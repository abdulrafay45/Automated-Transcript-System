using System;
using System.Collections.Generic;
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

    public class ExamBranchController : Controller
    {
        NUMLAutomatedTranscriptEntities db = new NUMLAutomatedTranscriptEntities();
        // GET: ExamBranch
        public ActionResult Dashboard()
        {
            if (Session["ExamBranch"] != null)
            {
                Exambranch Exb = new Exambranch
                {
                    exambranch = Session["ExamBranch"] as ExamBranch
                };
                Exb.result = new Result();
                return View(Exb);
            }
            return RedirectToAction("Index", "Student");
        }
        public ActionResult Other(Exambranch Exb)
        {
            if (Session["ExamBranch"] != null)
            {
                // If Exb is null, create a new instance and set the exambranch property from session
                if (Exb.student == null && Exb.resultss == null && Exb.FormSem == 0 && Exb.ToSem ==0)
                {
                    Exb = new Exambranch
                    {
                        exambranch = Session["ExamBranch"] as ExamBranch
                    };
                    Exb.student = new Student();
                }
                else
                {
                    // Check if the student exists in the database
                    Exb.student = db.Students.FirstOrDefault(x => x.SystemID == Exb.student.SystemID);
                    Exb.exambranch = Session["ExamBranch"] as ExamBranch;
                    if (Exb.student != null)
                    {
                        // Fetch distinct SemesterIds for the existing student
                        Exb.resultss = db.Results
                            .Where(r => r.StudentId == Exb.student.StudentId)
                            .Select(r => r.SemesterId)
                            .Distinct()
                            .ToList();
                        if (Exb.resultss != null && Exb.resultss.Any())
                        {
                            // Create a SelectList for the dropdown
                            ViewBag.SemesterList = new SelectList(Exb.resultss.Select(s => new { Value = s, Text = "Semester " + s }).ToList(), "Value", "Text");
                        }
                        else
                        {
                            // Handle the case when no results are found
                            ViewBag.SemesterList = new SelectList(Enumerable.Empty<SelectListItem>());
                        }
                    }
                }

                // Return the view with the Exb model
                return View(Exb);
            }

            // Redirect to the Index action of the Student controller if the session is null
            return RedirectToAction("Index", "Student");
        }
        public ActionResult DashboardTest()
        {
            if (Session["ExamBranch"] != null)
            {
                Exambranch Exb = new Exambranch
                {
                    exambranch = Session["ExamBranch"] as ExamBranch
                };
                
                return View(Exb);
            }
            return RedirectToAction("Index", "Student");
        }
        public ActionResult PrintAll1(Exambranch Exb)
        {
            if (Session["ExamBranch"] != null)
            {
                var studentId = Exb.result.StudentId;
                return new ActionAsPdf("PrintTranscript", new { studentId });
            }
            return RedirectToAction("Index", "Student");

        }
        public ActionResult PrintTranscript(String studentId)
            {
            if (studentId != null)
            {
                Exambranch exambranch = new Exambranch();
                exambranch.student = db.Students.Where(x => x.SystemID == studentId).FirstOrDefault();
                exambranch.results = db.Results.Where(x => x.StudentId == exambranch.student.StudentId).OrderBy(x => x.SemesterId).ToList();

                exambranch.Courses = new List<Cours>();
                for (int i = 0; i < exambranch.results.Count; i++)
                {
                    var courseId = exambranch.results[i].CourseId; // Retrieve the ProgrammeId from Std.Course[i] outside of the LINQ query

                    Cours Course = (from c in db.Results
                                    join r in db.Courses on c.CourseId equals r.CourseId
                                    where r.CourseId == courseId // Use the retrieved ProgrammeId in the query
                                    select r).FirstOrDefault();


                    exambranch.Courses.Add(Course);
                }
                return View(exambranch);
            }
            return RedirectToAction("Index", "Student");
        }
        public ActionResult PrintAll2(Exambranch Exb)
        {
            if (Session["ExamBranch"] != null)
            {
                var studentId = Exb.result.StudentId;
                return new ActionAsPdf("PrintTranscript2", new { studentId });
            }
            return RedirectToAction("Index", "Student");

        }
        public ActionResult PrintTranscript2(String studentId)
            {
            if (studentId != null)
            {
                Exambranch exambranch = new Exambranch();
                exambranch.student = db.Students.Where(x => x.SystemID == studentId).FirstOrDefault();
                exambranch.results = db.Results.Where(x => x.StudentId == exambranch.student.StudentId).OrderBy(x => x.SemesterId).ToList();

                exambranch.Courses = new List<Cours>();
                for (int i = 0; i < exambranch.results.Count; i++)
                {
                    var courseId = exambranch.results[i].CourseId; // Retrieve the ProgrammeId from Std.Course[i] outside of the LINQ query

                    Cours Course = (from c in db.Results
                                    join r in db.Courses on c.CourseId equals r.CourseId
                                    where r.CourseId == courseId // Use the retrieved ProgrammeId in the query
                                    select r).FirstOrDefault();


                    exambranch.Courses.Add(Course);
                }
                return View(exambranch);
            }
            return RedirectToAction("Index", "Student");
        }
        public ActionResult PrintAll3(Exambranch Exb)
        {
            if (Session["ExamBranch"] != null)
            {
                var studentId = Exb.result.StudentId;
                return new ActionAsPdf("PrintTranscript3", new { studentId });
            }
            return RedirectToAction("Index", "Student");

        }
        public ActionResult PrintTranscript3(String studentId)
            {
            if (studentId != null)
            {
                Exambranch exambranch = new Exambranch();
                exambranch.student = db.Students.Where(x => x.SystemID == studentId).FirstOrDefault();
                exambranch.results = db.Results.Where(x => x.StudentId == exambranch.student.StudentId).OrderBy(x => x.SemesterId).ToList();

                exambranch.Courses = new List<Cours>();
                for (int i = 0; i < exambranch.results.Count; i++)
                {
                    var courseId = exambranch.results[i].CourseId; // Retrieve the ProgrammeId from Std.Course[i] outside of the LINQ query

                    Cours Course = (from c in db.Results
                                    join r in db.Courses on c.CourseId equals r.CourseId
                                    where r.CourseId == courseId // Use the retrieved ProgrammeId in the query
                                    select r).FirstOrDefault();


                    exambranch.Courses.Add(Course);
                }
                return View(exambranch);
            }
            return RedirectToAction("Index", "Student");
        }
        public ActionResult PrintSelected(Exambranch Exb)
        {
            if (Session["ExamBranch"] != null)
            {
                Exb.student = db.Students.FirstOrDefault(x => x.SystemID == Exb.student.SystemID);
                var studentId = Exb.student.StudentId;
                var FormSem = Exb.FormSem;
                var ToSem = Exb.ToSem;
                return new ActionAsPdf("PrintSelctedTranscript", new { studentId , FormSem , ToSem });
            }
            return RedirectToAction("Index", "Student");
        }
        public ActionResult PrintSelctedTranscript(int studentId , int FormSem , int ToSem)
        {
            if (studentId != 0)
            {
                Exambranch exambranch = new Exambranch();
                exambranch.student = db.Students.Where(x => x.StudentId == studentId).FirstOrDefault();
                exambranch.results = db.Results
                .Where(r => r.StudentId == exambranch.student.StudentId && r.SemesterId >= FormSem && r.SemesterId <= ToSem)
                .ToList();

                exambranch.Courses = new List<Cours>();
                for (int i = 0; i < exambranch.results.Count; i++)
                {
                    var courseId = exambranch.results[i].CourseId; // Retrieve the ProgrammeId from Std.Course[i] outside of the LINQ query

                    Cours Course = (from c in db.Results
                                    join r in db.Courses on c.CourseId equals r.CourseId
                                    where r.CourseId == courseId // Use the retrieved ProgrammeId in the query
                                    select r).FirstOrDefault();
                    exambranch.Courses.Add(Course);
                }
                return View(exambranch);
            }
            return RedirectToAction("Index", "Student");
        }
    }
}