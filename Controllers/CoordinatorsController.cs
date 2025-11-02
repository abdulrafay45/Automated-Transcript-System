using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using FYPWeb.Model;
using FYPWeb.DataModel;
using System.Data.Entity.Validation;
using System.IO;
using System.Threading.Tasks;
using ExcelDataReader;

namespace FYPWeb.Controllers
{
    public class CoordinatorsController : Controller
    {
        NUMLAutomatedTranscriptEntities db = new NUMLAutomatedTranscriptEntities();
        // GET: Coordinators
        public ActionResult CoordinatorsDashboard()
        {
            if (Session["Coordinators"] != null)
            {
                ManageCourse Std = new ManageCourse();
                Std.coordinator = Session["Coordinators"] as Coordinator;
                return View(Std);
            }
            return RedirectToAction("Index", "Student");
        }
        public ActionResult CoordinatorsDashboardtest()
        {
            if (Session["Coordinators"] != null)
            {
                ManageCourse Std = new ManageCourse();
                Std.coordinator = Session["Coordinators"] as Coordinator;

                Std.Courses = db.Courses.ToList();
                Std.programmes = db.Programmes.ToList();
                Std.Batches = db.Batches.OrderByDescending(b => b.BNumber).ToList();
                return View(Std);
            }
            return RedirectToAction("Index", "Student");
        }

        public ActionResult AddCourse(int studentId)
        {
            if (Session["Coordinators"] != null)
            {
                if (TempData["Error"] != null)
                {
                    ModelState.AddModelError("", TempData["Error"].ToString());
                }
                ManageCourse Std = new ManageCourse();
                Std.student = new Student();
                Std.Course = new Cours();
                Std.coordinator = Session["Coordinators"] as Coordinator;
                Std.student = db.Students.FirstOrDefault(x => x.StudentId == studentId);
                Std.Courses = db.Courses.ToList();
                return View(Std);
            }
            return RedirectToAction("Index", "Student");
        }
        [HttpPost]
        public ActionResult AddNewCourse(ManageCourse studentUser)
        {
            try
            {
                // Check if the student already exists
                var existingCourse = db.Enrolls.FirstOrDefault(x => x.CourseId == studentUser.Course.CourseId && x.StudentId == studentUser.student.StudentId);
                

                if (existingCourse == null)
                {
                    ManageCourse Std = new ManageCourse();
                    Std.Enroll.StudentId = studentUser.student.StudentId;
                    Std.Enroll.CourseId = studentUser.Course.CourseId;
                    Std.Enroll.SemesterNo = studentUser.student.CurrentSemester;
                    // Add the student and user to the database
                    db.Enrolls.Add(studentUser.Enroll);


                    // Save changes to the database
                    db.SaveChanges();

                    // Redirect to CreateProfile action upon successful save
                    return RedirectToAction("StudentCourse", "Coordinators", new { studentId= studentUser.student.StudentId });
                }
                else
                {
                    // Handle case where student already exists
                    TempData["Error"] = "A student with the same Course already exists.";
                    return RedirectToAction("AddCourse", "Coordinators" , new { studentId  = studentUser.student.StudentId});
                }
            }
            catch (DbEntityValidationException ex)
            {
                // Handle validation errors
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        ModelState.AddModelError("", validationError.ErrorMessage);
                    }
                }
                return RedirectToAction("AssignCourse", "Coordinators");
            }
            catch (Exception exp)
            {
                // Handle other exceptions
                ModelState.AddModelError("", "An error occurred while creating the student. Please try again later.");
                Console.WriteLine(exp.Message);
                if (exp.InnerException != null)
                {
                    // Print the inner exception's message
                    Console.WriteLine("Inner Exception: " + exp.InnerException.Message);
                }
                return RedirectToAction("AssignCourse", "Coordinators");
            }
        }
        public ActionResult AddMarks()
        {
            if (Session["Coordinators"] != null)
            {
                ManageCourse Std = new ManageCourse();
                Std.coordinator = Session["Coordinators"] as Coordinator;
                Std.Courses = db.Courses.ToList();
                Std.result = new Result();
                Std.students = db.Students.ToList();
                string message = TempData["Marks"] as string; // Retrieve message from TempData
                if (!string.IsNullOrEmpty(message))
                {
                    ViewBag.Marks = message;
                }
                return View(Std);
            }
            return RedirectToAction("Index", "Student");
        }
        [HttpPost]
        public ActionResult AddNewMarks(ManageCourse studentUser)
        {
            try
            {
                // Check if the student already exists
                var existingStd = db.Students.FirstOrDefault(x => x.SystemID == studentUser.SystemID);

                if (existingStd != null)
                {
                    
                    // Karo Test ma nashta kar lo Mujy Ya File Baj d3ena database ki or Code wali b  ok

                    Result resultExit = db.Results.Where((x) => x.SemesterId == studentUser.result.SemesterId && x.CourseId == studentUser.result.CourseId && x.StudentId == existingStd.StudentId ).FirstOrDefault();
                    if (resultExit == null)
                    {
                        int maxResultId = db.Results.Max(s => (int?)s.ResultId) ?? 0;
                        int newResultId = maxResultId + 1;
                        studentUser.result.ResultId = newResultId;
                        studentUser.result.StudentId = existingStd.StudentId;
                        db.Results.Add(studentUser.result);
                        db.SaveChanges();
                    }
                    else
                    {
                        resultExit = studentUser.result;
                        db.SaveChanges();
                        return RedirectToAction("CoordinatorsDashboard", "Coordinators");
                    }

                    var allResult = db.Results.Where((x) => x.StudentId == existingStd.StudentId).ToList();
                    // Add the student and user to the database
                    decimal? TotalCGPA = 0;
                    if (allResult != null)
                    {

                        for (int i = 0; i < 11; i++)
                        {
                            decimal? GPA = 0;
                            int semesterNo = i + 1;
                            var semesterResult = allResult.Where((x) => x.StudentId == semesterNo).ToList();
                            if (semesterResult.Count == 0)
                            {
                                break;
                            }
                            else
                            {
                                decimal? totalObtainCredit = 0;
                                for (int j = 0; j < semesterResult.Count; j++)
                                {
                                    var currentCouseId = semesterResult[i].CourseId;
                                    Cours stdCourse = db.Courses.Where((x) => x.CourseId == currentCouseId).FirstOrDefault();
                                    totalObtainCredit += (semesterResult[j].ObtainMarks / stdCourse.TotalMarks) * stdCourse.TotalCreditHours;
                                }
                                GPA += totalObtainCredit / 4;
                            }
                            if (TotalCGPA != 0)
                            {
                                TotalCGPA = (TotalCGPA + GPA) / 2;
                            }
                            else
                            {
                                TotalCGPA = GPA;
                            }
                        }
                    }
                    existingStd.CGPA = (decimal)TotalCGPA;

                    // Save changes to the database
                    db.SaveChanges();
                    ViewBag.Successfully = "Add All Marks Successfully";
                    TempData["Marks"] = ViewBag.Successfully;
                    // Redirect to CreateProfile action upon successful save
                    return RedirectToAction("AddMarks", "Coordinators");
                }
                else
                {
                    // Handle case where student already exists
                    ModelState.AddModelError("", "A student with the same name already exists.");
                    return RedirectToAction("AddMarks", "Coordinators");
                }
            }
            catch (DbEntityValidationException ex)
            {
                // Handle validation errors
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        ModelState.AddModelError("", validationError.ErrorMessage);
                    }
                }
                return RedirectToAction("AssignCourse", "Coordinators");
            }
            catch (Exception exp)
            {
                // Handle other exceptions
                ModelState.AddModelError("", "An error occurred while creating the student. Please try again later.");
                Console.WriteLine(exp.Message);
                if (exp.InnerException != null)
                {
                    // Print the inner exception's message
                    Console.WriteLine("Inner Exception: " + exp.InnerException.Message);
                }
                return RedirectToAction("AssignCourse", "Coordinators");
            }
        }

        public JsonResult GetSemestersForBatch(string batchName)
        {
            var batches = db.Batches.ToList();
            var latestBatch = batches.OrderByDescending(b => b.BNumber).FirstOrDefault();

            if (latestBatch == null)
            {
                return Json(new List<SelectListItem>(), JsonRequestBehavior.AllowGet);
            }

            var latestBatchYear = int.Parse(latestBatch.BNumber.Split('-')[2]);
            var selectedBatchYear = int.Parse(batchName.Split('-')[2]);

            var latestBatchTerm = latestBatch.BNumber.Split('-')[1];
            var selectedBatchTerm = batchName.Split('-')[1];

            // Calculate the semester difference
            int semesterDifference = 0;

            // Calculate the year difference
            int yearDifference = latestBatchYear - selectedBatchYear;

            // If the selected batch is the latest, or has a higher year, calculate all semesters available
            if (yearDifference == 0)
            {
                semesterDifference = selectedBatchTerm == "Sp" ? 1 : 2;
            }
            else if (yearDifference == 1)
            {
                semesterDifference = 3;
            }
            else if (yearDifference == 2)
            {
                semesterDifference = 5;
            }
            else if (yearDifference >= 3)
            {
                semesterDifference = 8;
            }

            // Adjust semester difference based on terms
            if (latestBatchTerm == "Fall" && selectedBatchTerm == "Sp")
            {
                semesterDifference--;
            }
            else if (latestBatchTerm == "Sp" && selectedBatchTerm == "Fall")
            {
                semesterDifference++;
            }

            // Create SelectListItems for the dropdown
            var semesters = new List<SelectListItem>();
            for (int i = 1; i <= semesterDifference; i++)
            {
                semesters.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            }

            return Json(semesters, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCoursesBySemester(int semesterId)
        {
            var courses = (from c in db.Courses
                           join sc in db.StudentsCourses on c.CourseId equals sc.CourseId
                           where sc.SemesterNo == semesterId
                           select c).ToList();
            return Json(courses, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetStudentsBySemesterAndCourse(int semesterId, int courseId)
        {


            var students = (from c in db.Students
                            join sc in db.StudentsCourses on c.CurrentSemester equals sc.SemesterNo
                            where sc.CourseId == courseId && c.CurrentSemester == semesterId
                            select new
                            {
                                StudentId = c.StudentId,
                                SystemID = c.SystemID
                            }).ToList();
            return Json(students, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> Uploadexcelfile(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var uploadsFolder = Path.Combine(Server.MapPath("~/Upload"), fileName);

                using (var stream = new FileStream(uploadsFolder, FileMode.Create))
                {
                    await file.InputStream.CopyToAsync(stream);
                }

                using (var stream = System.IO.File.Open(uploadsFolder, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        // Assuming the first row is a header, so skip it
                        reader.Read();

                        while (reader.Read()) // Read each row in the Excel sheet
                        {
                            Result Rs = new Result();
                            ManageCourse mc = new ManageCourse();
                            int maxResultId = db.Results.Max(s => (int?)s.ResultId) ?? 0;
                            int newResultId = maxResultId + 1;
                            Rs.ResultId = newResultId;
                            Rs.ObtainMarks = Convert.ToInt32(reader.GetValue(1));
                            string rollNumber = reader.GetValue(2).ToString();
                            // Retrieve the student from the database using LINQ
                            mc.student = db.Students.FirstOrDefault(x => x.SystemID == rollNumber);
                            Rs.StudentId = mc.student.StudentId;
                            string courseCode = reader.GetValue(3).ToString();
                            mc.Course = db.Courses.FirstOrDefault(x => x.CourseCode == courseCode);
                            Rs.CourseId = mc.Course.CourseId;
                            Rs.SemesterId = Convert.ToInt32(reader.GetValue(4));
                            db.Results.Add(Rs);
                            db.SaveChanges();
                        }

                        // Save changes to the database after reading all rows from the Excel file
                        
                        ViewBag.Successfully = "Add All Marks Successfully";
                        TempData["Marks"] = ViewBag.Successfully;
                        return RedirectToAction("AddMarks");
                    }
                }
            }
            else
            {
                ViewBag.Message = "empty";
            }

            // Optionally, you can perform additional processing with the uploaded file here
            return RedirectToAction("Index");
        }
        public ActionResult Studentdata()
        {
            if (Session["Coordinators"] != null)
            {
                ManageCourse Std = new ManageCourse();
                Std.coordinator = Session["Coordinators"] as Coordinator;
                Std.student = new Student();
                Std.Courses = db.Courses.ToList();
                Std.programmes = db.Programmes.ToList();
                Std.Batches = db.Batches.OrderByDescending(b => b.BNumber).ToList();
                return View(Std);
            }
            return RedirectToAction("Index", "Student");
        }
        [HttpPost]
        public ActionResult Studentdata(ManageCourse mc)
        {
            if (Session["Coordinators"] == null)
            {
                // Handle the case where the session does not have a coordinator
                return RedirectToAction("Login", "Account"); // Redirect to login or appropriate action
            }

            // Initialize the ManageCourse object
            ManageCourse Std = new ManageCourse
            {
                coordinator = Session["Coordinators"] as Coordinator,
                students = db.Students
                    .Where(x => x.CurrentSemester == mc.student.CurrentSemester
                             && x.Department == mc.student.Department
                             && x.Batch == mc.student.Batch) // Ensure correct filtering based on batch
                    .ToList(),
                Courses = db.Courses.ToList(),
                programmes = db.Programmes.ToList(),
                Batches = db.Batches.OrderByDescending(b => b.BNumber).ToList()
            };
            long batchId = long.Parse(mc.student.Batch);
            long departmentId = long.Parse(mc.student.Department);
            // Prepare results to be displayed
            var results = Std.students.Select(s => new Studentslist
            {
                SystemID = s.SystemID,
                Name = s.Name,
                StudentId =s.StudentId,
                Batch = db.Batches.FirstOrDefault(x => x.Bid == batchId)?.BNumber,
                Semester = s.CurrentSemester,
                Program = db.Programmes.FirstOrDefault(x => x.ProgrammeId == departmentId)?.ProgrammeName,
            }).ToList();

            // Store results in ViewBag to pass to the view
            Std.Studentslist = results;

            return View(Std); // Pass the updated ManageCourse object to the view
        }
        
        public ActionResult StudentCourse(int studentId)
        {
            if (Session["Coordinators"] == null)
            {
                // Handle the case where the session does not have a coordinator
                return RedirectToAction("Login", "Account"); // Redirect to login or appropriate action
            }
            // Initialize the ManageCourse object
            ManageCourse Std = new ManageCourse
            {
                coordinator = Session["Coordinators"] as Coordinator,
                student = db.Students.FirstOrDefault(x => x.StudentId == studentId),
                Enrolls = db.Enrolls.Where(x => x.StudentId == studentId).ToList()
            };
            // Get the list of course IDs the student is enrolled in
            var enrolledCourseIds = Std.Enrolls.Select(e => e.CourseId).ToList();
            // Retrieve the courses the student is enrolled in
            Std.Courses = db.Courses.Where(c => enrolledCourseIds.Contains(c.CourseId)).ToList();
            return View(Std);
        }

    }
}