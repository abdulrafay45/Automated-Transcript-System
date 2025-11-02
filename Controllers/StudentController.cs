using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using FYPWeb.DataModel;
using FYPWeb.Model;
using Rotativa;

namespace FYPWeb.Controllers
{
    public class StudentController : Controller
    {
        NUMLAutomatedTranscriptEntities db = new NUMLAutomatedTranscriptEntities();
        // GET: Student
        public ActionResult Index()
        {
            string message = TempData["User"] as string; // Retrieve message from TempData
            if (!string.IsNullOrEmpty(message))
            {
                ViewBag.Error = message;
            }
            User user = new User();
            return View(user);
        }
        public ActionResult Dashboard()
        {
          
            if (Session["Student"] != null)
            {
                StdResult stdResult = new StdResult
                {
                    student = Session["Student"] as Student
                };
                int Pid = int.Parse(stdResult.student.Department);
                stdResult.Programme = db.Programmes.FirstOrDefault(x=>x.ProgrammeId == Pid);

                if (stdResult.student != null)
                {
                    // Get distinct SemesterIds for the student
                    var semesterIds = db.Results
                        .Where(x => x.StudentId == stdResult.student.StudentId)
                        .Select(r => r.SemesterId)
                        .Distinct()
                        .ToList();

                    if (semesterIds.Count != 0)
                    {
                        // Fetch the first result for each distinct semester
                        stdResult.results = db.Results
                            .Where(r => semesterIds.Contains(r.SemesterId) && r.StudentId == stdResult.student.StudentId)
                            .GroupBy(r => r.SemesterId)
                            .Select(g => g.FirstOrDefault())
                            .ToList();

                        // Get the most recent semester ID
                        var latestSemesterId = semesterIds.OrderByDescending(x => x).FirstOrDefault();
                        stdResult.Totalsemester = latestSemesterId ?? 0;
                    }
                    else
                    {
                        stdResult.results = new List<Result>();
                        stdResult.Totalsemester = 0;
                    }

                    return View(stdResult);
                }
            }
            return RedirectToAction("Index", "Student");


            
        }
        public ActionResult Test(int ID)
        {
            if (Session["Student"] != null)
            {
                StdResult stdResult = new StdResult();
                stdResult.student = Session["Student"] as Student;
                stdResult.results = db.Results.Where(x => x.SemesterId == ID && x.StudentId == stdResult.student.StudentId).ToList();
                stdResult.Course = new List<Cours>();
                for (int i = 0; i < stdResult.results.Count; i++)
                {
                    var courseId = stdResult.results[i].CourseId; // Retrieve the ProgrammeId from Std.Course[i] outside of the LINQ query

                    Cours Course = (from c in db.Results
                                    join r in db.Courses on c.CourseId equals r.CourseId
                                    where r.CourseId == courseId // Use the retrieved ProgrammeId in the query
                                    select r).FirstOrDefault();


                    stdResult.Course.Add(Course);
                }
                ViewBag.Semester = ID;
                return View(stdResult);
            }
            return RedirectToAction("Index", "Student");
        }
        public ActionResult STranscript()
        {
            return View();
        }
        public ActionResult Transcript()
        {
            if (Session["Student"] != null)
            {
                StdResult stdResult = new StdResult();
                stdResult.student = Session["Student"] as Student;
                stdResult.results = db.Results.Where(x=> x.StudentId == stdResult.student.StudentId).OrderBy(X=>X.SemesterId).ToList();
                stdResult.Course = new List<Cours>();
                for (int i = 0; i < stdResult.results.Count; i++)
                {
                    var courseId = stdResult.results[i].CourseId; // Retrieve the ProgrammeId from Std.Course[i] outside of the LINQ query

                    Cours Course = (from c in db.Results
                                    join r in db.Courses on c.CourseId equals r.CourseId
                                    where r.CourseId == courseId // Use the retrieved ProgrammeId in the query
                                    select r).FirstOrDefault();


                    stdResult.Course.Add(Course);
                }
                return View(stdResult);
            }
            return RedirectToAction("Index", "Student");
        }
        public ActionResult PrintTranscripts()
        {
            if (Session["Student"] != null)
            {
                StdResult stdResult = new StdResult();
                stdResult.student = Session["Student"] as Student;
                stdResult.results = db.Results.Where(x=> x.StudentId == stdResult.student.StudentId).OrderBy(X=>X.SemesterId).ToList();
                stdResult.Course = new List<Cours>();
                for (int i = 0; i < stdResult.results.Count; i++)
                {
                    var courseId = stdResult.results[i].CourseId; // Retrieve the ProgrammeId from Std.Course[i] outside of the LINQ query

                    Cours Course = (from c in db.Results
                                    join r in db.Courses on c.CourseId equals r.CourseId
                                    where r.CourseId == courseId // Use the retrieved ProgrammeId in the query
                                    select r).FirstOrDefault();


                    stdResult.Course.Add(Course);
                }
                return View(stdResult);
            }
            return RedirectToAction("Index", "Student");
        }
        public ActionResult PrintTranscript(int studentId)
        {
            if (studentId != 0)
            {
                StdResult stdResult = new StdResult();
                stdResult.student = db.Students.Where(x => x.StudentId == studentId).FirstOrDefault();
                stdResult.results = db.Results.Where(x => x.StudentId == stdResult.student.StudentId).OrderBy(X => X.SemesterId).ToList();
                stdResult.Course = new List<Cours>();
                for (int i = 0; i < stdResult.results.Count; i++)
                {
                    var courseId = stdResult.results[i].CourseId; // Retrieve the ProgrammeId from Std.Course[i] outside of the LINQ query

                    Cours Course = (from c in db.Results
                                    join r in db.Courses on c.CourseId equals r.CourseId
                                    where r.CourseId == courseId // Use the retrieved ProgrammeId in the query
                                    select r).FirstOrDefault();


                    stdResult.Course.Add(Course);
                }
                return View(stdResult);
            }
            return RedirectToAction("Index", "Student");
        }
        public ActionResult PrintAll()
        {
                if (Session["Student"] != null)
                {
                    var studentId = (Session["Student"] as Student).StudentId;
                    return new ActionAsPdf("PrintTranscript", new { studentId });
                }
                return RedirectToAction("Index", "Student");
            
        }

        public ActionResult Result(int ID)
        {
            if (Session["Student"] != null)
            {
                StdResult stdResult = new StdResult();
                stdResult.student = Session["Student"] as Student;
                stdResult.results = db.Results.Where(x => x.SemesterId == ID && x.StudentId == stdResult.student.StudentId).ToList();
                stdResult.Course = new List<Cours>();
                for (int i = 0; i < stdResult.results.Count; i++)
                {
                    var courseId = stdResult.results[i].CourseId; // Retrieve the ProgrammeId from Std.Course[i] outside of the LINQ query

                    Cours Course = (from c in db.Results
                                     join r in db.Courses on c.CourseId equals r.CourseId
                                         where r.CourseId == courseId // Use the retrieved ProgrammeId in the query
                                         select r).FirstOrDefault();


                    stdResult.Course.Add(Course);
                }
                ViewBag.Semester = ID;
                return View(stdResult);
            }
            return RedirectToAction("Index", "Student");
        }
        public ActionResult PrintSemResut(int ID, int studentId)
        {
            if (studentId!=0)
            {
                StdResult stdResult = new StdResult();
                stdResult.student = db.Students.Where(x => x.StudentId == studentId).FirstOrDefault();
                stdResult.results = db.Results.Where(x => x.SemesterId == ID && x.StudentId == stdResult.student.StudentId).ToList();
                stdResult.Course = new List<Cours>();
                for (int i = 0; i < stdResult.results.Count; i++)
                {
                    var courseId = stdResult.results[i].CourseId; // Retrieve the ProgrammeId from Std.Course[i] outside of the LINQ query

                    Cours Course = (from c in db.Results
                                     join r in db.Courses on c.CourseId equals r.CourseId
                                         where r.CourseId == courseId // Use the retrieved ProgrammeId in the query
                                         select r).FirstOrDefault();


                    stdResult.Course.Add(Course);
                }
                ViewBag.Semester = ID;
                return View(stdResult);
            }
            return RedirectToAction("Index", "Student");
        }
        public ActionResult SemResult(int ID)
        {
            if (Session["Student"] != null)
            {
                var studentId = (Session["Student"] as Student).StudentId;
                return new ActionAsPdf("PrintSemResut", new { ID , studentId });
            }
            return RedirectToAction("Index", "Student");

        }
        [HttpPost]
        public ActionResult Login(User user)
        {
            try
            {
                User u = db.Users.Where((x) => x.Username == user.Username).FirstOrDefault();
                if (u != null)
                {
                    string salt = u.salt;
                    string hashedPassword = HashPassword(user.Password, salt);
                    if (hashedPassword == u.Password)
                    {
                        dynamic data;
                        if (u.RoleId == 1)
                        {
                            data = db.AdminStaffs.Where((X) => X.UserId == u.UserId).FirstOrDefault();
                            Session["Admin"] = data;
                            return RedirectToAction("AdminDashboard", "Admin");
                        }
                        else if (u.RoleId == 2)
                        {
                            data = db.Students.Where((X) => X.UserId == u.UserId).FirstOrDefault();
                            Session["Student"] = data;
                            return RedirectToAction("Dashboard", "Student");
                        }
                        else if (u.RoleId == 3)
                        {
                            data = db.Coordinators.Where((X) => X.UserId == u.UserId).FirstOrDefault();
                            Session["Coordinators"] = data;
                            return RedirectToAction("CoordinatorsDashboard", "Coordinators");
                        }
                        else if (u.RoleId == 4)
                        {
                            data = db.ExamBranches.Where((X) => X.UserId == u.UserId).FirstOrDefault();
                            Session["ExamBranch"] = data;
                            return RedirectToAction("Dashboard", "ExamBranch");
                        }
                        return RedirectToAction("Dashboard", "Student");
                    }
                    else
                    {
                        ViewBag.Error = "User Not Found Enter Correct Username Or Password";
                        TempData["User"] = ViewBag.Error;
                        return RedirectToAction("index", "Student");
                    }
                }
                else
                {
                    ViewBag.Error = "User Not Found Enter Correct Username Or Password";
                    TempData["User"] = ViewBag.Error;
                    return RedirectToAction("index", "Student");
                }
            }
            
            catch (Exception exp)
            {
                return RedirectToAction("index", "Student");
            }
        }
        [HttpPost]
        public ActionResult ChangePassowrd(StudentUser user)
        {
            try
            {
                if (Session["Student"] == null)
                {
                    ViewBag.Error = "Session expired. Please log in again.";
                    return RedirectToAction("Index", "Student");
                }

                user.Student = Session["Student"] as Student;

                
                if (user.Student == null || string.IsNullOrEmpty(user.Student.SystemID))
                {
                    ViewBag.Error = "Invalid System ID.";
                    return RedirectToAction("Index", "Student");
                }

                User u = db.Users.FirstOrDefault(x => x.Username == user.Student.SystemID);
                if (u == null)
                {
                    ViewBag.Error = "User not found. Enter correct System ID.";
                    return RedirectToAction("Index", "Student");
                }

                string salt = u.salt;
                string hashedOldPassword = HashPassword(user.Oldpassword, salt);
                if (hashedOldPassword != u.Password)
                {
                    ViewBag.Error = "Incorrect old password.";
                    return RedirectToAction("Index", "Student");
                }

                string newSalt = GenerateSalt();
                string hashedNewPassword = HashPassword(user.newpassword, newSalt);

                u.salt = newSalt;
                u.Password = hashedNewPassword;

                db.Entry(u).State = EntityState.Modified;
                db.SaveChanges();

                ViewBag.Message = "Password updated successfully.";
                return View(user);
            }
            catch (Exception exp)
            {
                ViewBag.Error = "An error occurred while updating the password. Please try again later.";
                Console.WriteLine(exp.Message);
                return View();
            }
        }

        public ActionResult ChangePassowrd()
        {
            if (Session["Student"] != null)
            {
                StudentUser stdResult = new StudentUser();
                stdResult.Student = Session["Student"] as Student;
                int Pid = int.Parse(stdResult.Student.Department);
                stdResult.Programme = db.Programmes.FirstOrDefault(x => x.ProgrammeId == Pid);
                stdResult.User = new User();
                return View(stdResult);
            }
            return RedirectToAction("Index", "Student");
        }
        
        public ActionResult Logout()
        {
            // Clear the user's session data
            Session.Clear();

            // Redirect the user to the login page
            return RedirectToAction("Index", "Student");
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
    }
}