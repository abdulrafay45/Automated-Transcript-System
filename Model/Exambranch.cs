using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FYPWeb.DataModel;
using FYPWeb.Model;

namespace FYPWeb.Model
{
    public class Exambranch
    {
        public Student student { get; set; }
        public ExamBranch exambranch { get; set; }
        public StudentsCours studentsCours { get; set; }
        public Result result { get; set; }
        public List<Result> results { get; set; }
        public List<int?> resultss { get; set; }
        public List<Cours> Courses { get; set; }
        public int FormSem { get; set; }
        public int ToSem { get; set; }
    }
}