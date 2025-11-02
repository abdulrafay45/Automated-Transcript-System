using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FYPWeb.DataModel;
using FYPWeb.Model;

namespace FYPWeb.Model
{
    public class ManageCourse
    {
        public Coordinator coordinator { get; set; }
        public StudentsCours studentsCours { get; set; }
        public Result result { get; set; }
        public Student student { get; set; }
        public List<Student> students { get; set; }
        public Cours Course { get; set; }
        public Batch Batch { get; set; }
        public AdminStaff Admin { get; set; }
        public List<Batch> Batches { get; set; }
        public List<Enroll> Enrolls { get; set; }
        public Enroll Enroll { get; set; }
        public RepeatedCours RepeatedCours { get; set; }
        
        public List<Cours> Courses { get; set; }
        public List<Programme> programmes { get; set; }
        public string SystemID { get; set; }
        public List<Studentslist> Studentslist { get; set; }
        public long BatchID { get; set; }
        public int ProgramId { get; set; }

    }
}