using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FYPWeb.Model
{
    public class Studentslist
    {
        public int StudentId { get; set; }
        public string SystemID { get; set; }
        public string Name { get; set; }
        public string Coursename { get; set; }
        public string Batch { get; set; }
        public string Program { get; set; }
        public int Semester { get; set; }
    }
}