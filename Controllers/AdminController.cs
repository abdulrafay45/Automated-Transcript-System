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

namespace FYPWeb.Controllers
{
    public class AdminController : Controller
    {
        NUMLAutomatedTranscriptEntities db =new NUMLAutomatedTranscriptEntities();
        // GET: Admin
        public ActionResult AdminDashboard()
        {
            
            
            if (Session["Admin"] != null)
            {
                StudentUser Std = new StudentUser();
                Std.Admin = Session["Admin"] as AdminStaff;
                Std.Course = db.Courses.ToList();
                return View(Std);
            }
            return RedirectToAction("Index", "Student");
        }
        public ActionResult Test()
        {
            if (Session["Admin"] != null)
            {
                StudentUser Std = new StudentUser();
                Std.Admin = Session["Admin"] as AdminStaff;
                Std.Programmes = db.Programmes.ToList();
                return View(Std);
            }
            return RedirectToAction("Index", "Student");
        }
        
        public ActionResult CreateProfile()
        {
            
            if (Session["Admin"] != null)
            {
                try
                {
                    StudentUser studentUser = new StudentUser();
                    studentUser.Campus = db.Campuses.ToList();
                    studentUser.Programmes = db.Programmes.ToList();
                    studentUser.Batches = db.Batches.OrderByDescending(b => b.Bid).ToList();
                    studentUser.Student = new Student();
                    studentUser.Coordinator = new Coordinator();
                    studentUser.examBranch = new ExamBranch();
                    studentUser.Admin = new AdminStaff();
                    studentUser.User = new User();
                    var data = Session["Admin"];
                    studentUser.Admin = data as AdminStaff;
                    string message = TempData["Profile"] as string; // Retrieve message from TempData
                    if (!string.IsNullOrEmpty(message))
                    {
                        ViewBag.Profile = message;
                    }
                    return View(studentUser);
                }
                catch (Exception e)
                {

                }
            }
            return RedirectToAction("Index", "Student");
        }

        [HttpPost]
        public ActionResult CreateAdmin(StudentUser studentUser)
        {
            try
            {
                // Check if the student already exists
                var existingStudent = db.Users.FirstOrDefault(x => x.Name == studentUser.User.Name);
                
                if (existingStudent == null)
                {
                    // Generate salt and hash password
                    string salt = GenerateSalt();
                    string hashedPassword = HashPassword(studentUser.User.Password, salt);

                    // Assign salt and hashed password to the user
                    studentUser.User.salt = salt;
                    studentUser.User.Password = hashedPassword;

                    // Set the role ID
                    studentUser.User.RoleId = 1;
                    studentUser.User.Name = studentUser.Admin.Name; 
                    int maxuserId = db.Users.Max(s => (int?)s.UserId) ?? 0;

                    //// Increment the maximum StudentId by 1
                    int newuserId = maxuserId +1;
                    studentUser.User.UserId = newuserId;
                    studentUser.Admin.UserId = newuserId;
                    db.Users.Add(studentUser.User);
                    db.SaveChanges();
                    int maxStudentId = db.Students.Max(s => (int?)s.StudentId) ?? 0;

                    // Increment the maximum StudentId by 1
                    int newStudentId = maxStudentId + 1;
                    studentUser.Admin.AdminStaffId = newStudentId;
                    
                    

                    // Add the student and user to the database
                    db.AdminStaffs.Add(studentUser.Admin);
                    

                    // Save changes to the database
                    db.SaveChanges();

                    // Redirect to CreateProfile action upon successful save
                    return RedirectToAction("CreateProfile", "Admin");
                }
                else
                {
                    // Handle case where student already exists
                    ModelState.AddModelError("", "A student with the same name already exists.");
                    return RedirectToAction("CreateProfile", "Admin");
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
                return RedirectToAction("CreateProfile", "Admin");
            }
            catch (Exception exp)
            {
                // Handle other exceptions
                ModelState.AddModelError("", "An error occurred while creating the student. Please try again later.");
                Console.WriteLine(exp.Message);
                return RedirectToAction("CreateProfile", "Admin");
            }
        }
        [HttpPost]
        public ActionResult CreateStudent(StudentUser studentUser)
        {
            try
            {
                // Check if the student already exists
                var existingUser = db.Students.FirstOrDefault(x => x.SystemID == studentUser.Student.SystemID);
                var existingStudent2 = db.Students.FirstOrDefault(x => x.RollNumber == studentUser.Student.RollNumber && x.RegistrationNumber == studentUser.Student.RegistrationNumber);
                
                
                if (existingUser == null && existingStudent2== null)
                {
                    studentUser.User = new User();
                    studentUser.User.Name = studentUser.Student.Name;
                    studentUser.User.Username = studentUser.Student.SystemID;
                    
                    string Name = studentUser.Student.SystemID;
                    studentUser.User.Password = Name;
                    // Generate salt and hash password
                    string salt = GenerateSalt();
                    string hashedPassword = HashPassword(studentUser.User.Password, salt);

                    // Assign salt and hashed password to the user
                    studentUser.User.salt = salt;
                    studentUser.User.Password = hashedPassword;
                    
                    // Set the role ID
                    studentUser.User.RoleId = 2;
                    
                    int maxuserId = db.Users.Max(s => (int?)s.UserId) ?? 0;

                    //// Increment the maximum StudentId by 1
                    int newuserId = maxuserId +1;
                    studentUser.User.UserId = newuserId;
                    studentUser.Student.UserId = newuserId;
                    string Batch = studentUser.Student.Batch;
                    db.Users.Add(studentUser.User);
                    db.SaveChanges();
                    int maxStudentId = db.Students.Max(s => (int?)s.StudentId) ?? 0;

                    // Increment the maximum StudentId by 1
                    int newStudentId = maxStudentId + 1;
                    studentUser.Student.StudentId = newStudentId;
                    studentUser.Batch = db.Batches
                    .Where(s => s.BNumber == studentUser.Student.Batch)                    
                    .FirstOrDefault();
                    studentUser.Student.Batch = studentUser.Batch.Bid.ToString();


                    // Add the student and user to the database
                    db.Students.Add(studentUser.Student);
                    

                    // Save changes to the database
                    db.SaveChanges();
                    studentUser.Scheme = db.Schemes
                    .Where(s => s.BatchNo == Batch)
                    .FirstOrDefault();

                    int studentId = db.Students
                    .Where(s => s.SystemID == studentUser.Student.SystemID)
                    .Select(s => s.StudentId)
                    .FirstOrDefault();
                    if (studentUser.Scheme != null)
                    {
                        // Step 2: Retrieve course IDs
                        var courseIds = db.StudentsCourses
                            .Where(ac => ac.Scheme == studentUser.Scheme.SchemeNo && ac.SemesterNo == studentUser.Student.CurrentSemester)
                            .Select(ac => ac.CourseId)
                            .ToList();

                        // Step 3: Insert into Enroll
                        foreach (var courseId in courseIds)
                        {
                            db.Enrolls.Add(new Enroll
                            {
                                StudentId = studentId,
                                CourseId = courseId,
                                SemesterNo = studentUser.Student.CurrentSemester
                            });
                        }
                        db.SaveChanges();
                    }
                    else
                    {
                        throw new Exception("Scheme not found");
                    }
                    ViewBag.Error = "Student Add Successfully";
                    TempData["User"] = ViewBag.Error;
                    // Redirect to CreateProfile action upon successful save
                    return RedirectToAction("CreateProfile", "Admin");
                }
                else
                {
                    // Handle case where student already exists
                    ViewBag.Error = "A student with the same name already exists.";
                    TempData["Profile"] = ViewBag.Error;
                    return RedirectToAction("CreateProfile", "Admin");
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
                return RedirectToAction("CreateProfile", "Admin");
            }
            catch (Exception exp)
            {
                // Handle other exceptions
                ModelState.AddModelError("", "An error occurred while creating the student. Please try again later.");
                Console.WriteLine(exp.Message);
                return RedirectToAction("CreateProfile", "Admin");
            }
        }
        [HttpPost]
        public ActionResult CreateCoordinator(StudentUser studentUser)
        {
            try
            {
                // Check if the student already exists
                var existingStudent = db.Users.FirstOrDefault(x => x.Name == studentUser.User.Name);
                var existingStudent2 = db.Coordinators.FirstOrDefault(x => x.EmpId == studentUser.Coordinator.EmpId);
                if (existingStudent == null && existingStudent2 == null)
                {
                    // Generate salt and hash password
                    string salt = GenerateSalt();
                    string hashedPassword = HashPassword(studentUser.User.Password, salt);

                    // Assign salt and hashed password to the user
                    studentUser.User.salt = salt;
                    studentUser.User.Password = hashedPassword;

                    // Set the role ID
                    studentUser.User.RoleId = 3;
                    studentUser.User.Name = studentUser.Coordinator.Name; 
                    int maxuserId = db.Users.Max(s => (int?)s.UserId) ?? 0;

                    //// Increment the maximum StudentId by 1
                    int newuserId = maxuserId +1;
                    studentUser.User.UserId = newuserId;
                    studentUser.Coordinator.UserId = newuserId;
                    db.Users.Add(studentUser.User);
                    db.SaveChanges();

                    // Add the student and user to the database
                    db.Coordinators.Add(studentUser.Coordinator);
                    

                    // Save changes to the database
                    db.SaveChanges();

                    // Redirect to CreateProfile action upon successful save
                    return RedirectToAction("CreateProfile", "Admin");
                }
                else
                {
                    // Handle case where student already exists
                    ViewBag.Error = "A  Coordinators with Same EmpID already exists.";
                    TempData["Profile"] = ViewBag.Error;
                    return RedirectToAction("CreateProfile", "Admin");
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
                return RedirectToAction("CreateProfile", "Admin");
            }
            catch (Exception exp)
            {
                // Handle other exceptions
                ModelState.AddModelError("", "An error occurred while creating the student. Please try again later.");
                Console.WriteLine(exp.Message);
                return RedirectToAction("CreateProfile", "Admin");
            }
        }
        [HttpPost]
        public ActionResult CreateExamBranch(StudentUser studentUser)
        {
            try
            {
                // Check if the student already exists
                var existingStudent = db.Users.FirstOrDefault(x => x.Name == studentUser.User.Name);
                var existingStudent2 = db.ExamBranches.FirstOrDefault(x => x.EmpId == studentUser.examBranch.EmpId);
                if (existingStudent == null && existingStudent2 == null)
                {
                    // Generate salt and hash password
                    string salt = GenerateSalt();
                    string hashedPassword = HashPassword(studentUser.User.Password, salt);

                    // Assign salt and hashed password to the user
                    studentUser.User.salt = salt;
                    studentUser.User.Password = hashedPassword;

                    // Set the role ID
                    studentUser.User.RoleId = 4;
                    studentUser.User.Name = studentUser.examBranch.ExamBranchName; 
                    int maxuserId = db.Users.Max(s => (int?)s.UserId) ?? 0;

                    //// Increment the maximum StudentId by 1
                    int newuserId = maxuserId +1;
                    studentUser.User.UserId = newuserId;
                    studentUser.examBranch.UserId = newuserId;
                    db.Users.Add(studentUser.User);
                    db.SaveChanges();
                    
                    
                    

                    // Add the student and user to the database
                    db.ExamBranches.Add(studentUser.examBranch);
                    

                    // Save changes to the database
                    db.SaveChanges();

                    // Redirect to CreateProfile action upon successful save
                    return RedirectToAction("CreateProfile", "Admin");
                }
                else
                {
                    // Handle case where student already exists
                    ViewBag.Error = "A Exam Brach Emp with Same EmpID already exists.";
                    TempData["Profile"] = ViewBag.Error;
                    return RedirectToAction("CreateProfile", "Admin");
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
                return RedirectToAction("CreateProfile", "Admin");
            }
            catch (Exception exp)
            {
                // Handle other exceptions
                ModelState.AddModelError("", "An error occurred while creating the student. Please try again later.");
                Console.WriteLine(exp.Message);
                return RedirectToAction("CreateProfile", "Admin");
            }
        }

        protected string GenerateSalt()
        {
            // Generate a random salt (you can use a cryptographically secure random number generator)
            // For simplicity, we are using a simple random string generator here
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var saltChars = new char[16];
            for (int i = 0; i < saltChars.Length; i++)
            {
                saltChars[i] = chars[random.Next(chars.Length)];
            }
            return new string(saltChars);
        }

        protected string HashPassword(string password, string salt)
        {
            // Combine the password and salt
            string combinedPassword = password + salt;

            // Choose the hash algorithm (SHA-256 or SHA-512)
            using (var sha256 = SHA256.Create())
            {
                // Convert the combined password string to a byte array
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(combinedPassword);

                // Compute the hash value of the byte array
                byte[] hash = sha256.ComputeHash(bytes);

                // Convert the byte array to a hexadecimal string
                System.Text.StringBuilder result = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    result.Append(hash[i].ToString("x2"));
                }

                return result.ToString();
            }
        }

        public ActionResult ManageCourse()
        {
            if (Session["Admin"] != null)
            {
                StudentUser Std = new StudentUser();
                Std.Admin = Session["Admin"] as AdminStaff;
                Std.Course = db.Courses.ToList();
                Std.Programmes = new List<Programme>();
                for(int i = 0; i < Std.Course.Count; i++)
                {
                    var courseId = Std.Course[i].ProgrammeId; // Retrieve the ProgrammeId from Std.Course[i] outside of the LINQ query

                    Programme program = (from c in db.Courses
                                         join p in db.Programmes on c.ProgrammeId equals p.ProgrammeId
                                         where p.ProgrammeId == courseId // Use the retrieved ProgrammeId in the query
                                         select p).FirstOrDefault();


                    Std.Programmes.Add(program);
                }
                return View(Std);
            }
            return RedirectToAction("Index", "Student");
        }

        public ActionResult AddCourse()
        {
            if (Session["Admin"] != null)
            {
                StudentUser Std = new StudentUser();
                Std.Admin = Session["Admin"] as AdminStaff;
                Std.SingleCourse = new Cours();
                Std.Programmes = db.Programmes.ToList();
                Std.degrees = db.Degrees.ToList();
                return View(Std);
            }
            return RedirectToAction("Index", "Student");
        }

        [HttpPost]
        public ActionResult AddNewCourse(StudentUser studentUser)
        {
            try
            {
                // Check if the student already exists
                var existingCourse = db.Courses.FirstOrDefault(x => x.CourseCode == studentUser.SingleCourse.CourseCode);

                if (existingCourse == null)
                {
                    int maxStudentId = db.Courses.Max(s => (int?)s.CourseId) ?? 0;

                    // Increment the maximum StudentId by 1
                    int newStudentId = maxStudentId + 1;
                    studentUser.SingleCourse.CourseId = newStudentId;
                    // Add the student and user to the database
                    db.Courses.Add(studentUser.SingleCourse);


                    // Save changes to the database
                    db.SaveChanges();

                    // Redirect to CreateProfile action upon successful save
                    return RedirectToAction("ManageCourse", "Admin");
                }
                else
                {
                    // Handle case where student already exists
                    ModelState.AddModelError("", "A student with the same name already exists.");
                    return RedirectToAction("CreateProfile", "Admin");
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
                return RedirectToAction("CreateProfile", "Admin");
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
                return RedirectToAction("CreateProfile", "Admin");
            }
        }
        public ActionResult ManageProgram()
        {
            if (Session["Admin"] != null)
            {
                StudentUser Std = new StudentUser();
                Std.Admin = Session["Admin"] as AdminStaff;
                Std.Programmes = db.Programmes.ToList();
                return View(Std);
            }
            return RedirectToAction("Index", "Student");
        }

        public ActionResult AddProgram()
        {
            if (Session["Admin"] != null)
            {
                StudentUser Std = new StudentUser();
                Std.Admin = Session["Admin"] as AdminStaff;
                Std.Programme = new Programme();
                Std.Programmes = db.Programmes.ToList();
                return View(Std);
            }
            return RedirectToAction("Index", "Student");
        }

        [HttpPost]
        public ActionResult AddNewProgram(StudentUser studentUser)
        {
            try
            {
                // Check if the student already exists
                var existingCourse = db.Programmes.FirstOrDefault(x => x.ProgrammeName == studentUser.Programme.ProgrammeName);

                if (existingCourse == null)
                {
                    int maxStudentId = db.Programmes.Max(s => (int?)s.ProgrammeId) ?? 0;

                    // Increment the maximum StudentId by 1
                    int newStudentId = maxStudentId + 1;
                    studentUser.Programme.ProgrammeId = newStudentId;
                    // Add the student and user to the database
                    db.Programmes.Add(studentUser.Programme);


                    // Save changes to the database
                    db.SaveChanges();

                    // Redirect to CreateProfile action upon successful save
                    return RedirectToAction("ManageProgram", "Admin");
                }
                else
                {
                    // Handle case where student already exists
                    ModelState.AddModelError("", "A student with the same name already exists.");
                    return RedirectToAction("CreateProfile", "Admin");
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
                return RedirectToAction("CreateProfile", "Admin");
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
                return RedirectToAction("CreateProfile", "Admin");
            }
        }
        public ActionResult AddScheme()
        {
            if (Session["Admin"] != null)
            {
                StudentUser Std = new StudentUser();
                Std.Admin = Session["Admin"] as AdminStaff;
                Std.Scheme = new Scheme();
                Std.Batches = db.Batches.OrderByDescending(b => b.Bid).ToList();
                Std.Assigncourse = db.StudentsCourses
                    .Select(r => r.Scheme)
                    .Distinct()
                    .ToList();
                ViewBag.SchemeList = new SelectList(Std.Assigncourse.Select(s => new { Value = s, Text = "Seheme " + s }).ToList(), "Value", "Text");
                return View(Std);
            }
            return RedirectToAction("Index", "Student");
        }

        [HttpPost]
        public ActionResult AddScheme(StudentUser studentUser)
        {
            try
            {
                // Check if the student already exists
                var existingCourse = db.Schemes.FirstOrDefault(x => x.BatchNo == studentUser.Scheme.BatchNo && x.SchemeNo==studentUser.Scheme.SchemeNo);

                if (existingCourse == null)
                {
                    // Add the student and user to the database
                    db.Schemes.Add(studentUser.Scheme);


                    // Save changes to the database
                    db.SaveChanges();

                    // Redirect to CreateProfile action upon successful save
                    return RedirectToAction("AddScheme", "Admin");
                }
                else
                {
                    // Handle case where student already exists
                    ModelState.AddModelError("", "A student with the same name already exists.");
                    return RedirectToAction("CreateProfile", "Admin");
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
                return RedirectToAction("CreateProfile", "Admin");
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
                return RedirectToAction("CreateProfile", "Admin");
            }
        }
        public ActionResult AddBatch()
        {
            if (Session["Admin"] != null)
            {
                StudentUser Std = new StudentUser();
                Std.Admin = Session["Admin"] as AdminStaff;
                Std.Batch = new Batch();
                return View(Std);
            }
            return RedirectToAction("Index", "Student");
        }

        [HttpPost]
        public ActionResult AddNewBatch(StudentUser studentUser)
        {
            try
            {
                // Check if the student already exists
                var existingCourse = db.Batches.FirstOrDefault(x => x.BNumber == studentUser.Batch.BNumber);

                if (existingCourse == null)
                {
                    db.Batches.Add(studentUser.Batch);


                    // Save changes to the database
                    db.SaveChanges();

                    // Redirect to CreateProfile action upon successful save
                    return RedirectToAction("AddBatch", "Admin");
                }
                else
                {
                    // Handle case where student already exists
                    ModelState.AddModelError("", "A student with the same name already exists.");
                    return RedirectToAction("CreateProfile", "Admin");
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
                return RedirectToAction("CreateProfile", "Admin");
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
                return RedirectToAction("CreateProfile", "Admin");
            }
        }
        public ActionResult AssignCourse()
        {
            if (Session["Admin"] != null)
            {
                ManageCourse Std = new ManageCourse();
                Std.Admin = Session["Admin"] as AdminStaff;
                Std.Courses = db.Courses.ToList();
                Std.studentsCours = new StudentsCours();
                Std.programmes = db.Programmes.ToList();
                Std.Batches = db.Batches.OrderByDescending(b => b.Bid).ToList();
                return View(Std);
            }
            return RedirectToAction("Index", "Student");
        }

        [HttpPost]
        public ActionResult AssignCourse(ManageCourse studentUser)
        {
            try
            {
                // Check if the student already exists
                var existingCourse = db.StudentsCourses.FirstOrDefault(x => x.CourseId == studentUser.studentsCours.CourseId && x.SemesterNo == studentUser.studentsCours.SemesterNo);


                if (existingCourse == null)
                {
                    int maxStudentId = db.StudentsCourses.Max(s => (int?)s.StudentCourseId) ?? 0;

                    // Increment the maximum StudentId by 1
                    int newStudentId = maxStudentId + 1;
                    studentUser.studentsCours.StudentCourseId = newStudentId;
                    // Add the student and user to the database
                    db.StudentsCourses.Add(studentUser.studentsCours);


                    // Save changes to the database
                    db.SaveChanges();

                    // Redirect to CreateProfile action upon successful save
                    return RedirectToAction("AssignCourse", "Admin");
                }
                else
                {
                    // Handle case where student already exists
                    ModelState.AddModelError("", "A student with the same name already exists.");
                    return RedirectToAction("AssignCourse", "Admin");
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
                return RedirectToAction("AssignCourse", "Admin");
            }
        }
        public JsonResult GetSemestersForBatch(string batchName)
        {
            var batches = db.Batches.ToList();
            var latestBatch = batches
            .OrderByDescending(b => int.Parse(b.BNumber.Split('-')[2])) // Order by year descending
            .ThenByDescending(b => b.BNumber.Split('-')[1] == "Fall" ? 1 : 0) // Order by term within the same year
            .FirstOrDefault();

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
                semesterDifference = latestBatchTerm == selectedBatchTerm ? 1 : 1;
            }
            else if (yearDifference == 1)
            {
                semesterDifference = 3;
            }
            else if (yearDifference == 2)
            {
                semesterDifference = 5;
            }
            else if (yearDifference == 3)
            {
                semesterDifference = 7;
            }
            else if (yearDifference >= 4)
            {
                semesterDifference = 8;
            }

            // Adjust semester difference based on terms
            if (latestBatchTerm == selectedBatchTerm)
            {
               
            }
            else if (latestBatchTerm != selectedBatchTerm)
            {
                if(semesterDifference!=8)
                {
                    semesterDifference++;
                }
                
            }

            // Create SelectListItems for the dropdown
            var semesters = new List<SelectListItem>();
            for (int i = 1; i <= semesterDifference; i++)
            {
                semesters.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            }

            return Json(semesters, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteProgram(int id)
        {
            Programme Prog = db.Programmes.Find(id);
            if (Prog != null)
            {
                db.Programmes.Remove(Prog);
                db.SaveChanges();
            }

            return RedirectToAction("ManageCourse"); // Redirect back to the course management page
        }
        public ActionResult DeleteCourse(int id)
        {
            Cours course = db.Courses.Find(id);
            if (course != null)
            {
                db.Courses.Remove(course);
                db.SaveChanges();
            }

            return RedirectToAction("ManageCourse"); // Redirect back to the course management page
        }
    }
}