using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FYPWeb.DataModel;

namespace FYPWeb.Model
{
    public class StudentUser
    {
        public Student Student { get; set; }
        public AdminStaff Admin { get; set; }
        public User User { get; set; }
        public UserRole UserRole { get; set; }
        public ExamBranch examBranch { get; set; }
        public StudentsCours studentsCours { get; set; }
        public Coordinator Coordinator { get; set; }
        public List<Campus> Campus { get; set; }
        public List<Cours> Course { get; set; }
        public Cours SingleCourse { get; set; }
        public Semester semester { get; set; }
        public Scheme Scheme { get; set; }
        public Batch Batch { get; set; }
        public List<Batch> Batches { get; set; }
        public List<Semester> semesters { get; set; }
        public List<Programme> Programmes { get; set; }
        public Programme Programme { get; set; }
        public List<Degree> degrees { get; set; }
        public List<string> Assigncourse { get; set; }
        public string newpassword { get; set; }
        public string Oldpassword { get; set; }
        

    }
}