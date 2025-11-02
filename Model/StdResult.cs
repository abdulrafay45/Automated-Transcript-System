using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FYPWeb.DataModel;

namespace FYPWeb.Model
{
    public class StdResult
    {
        public int Totalsemester { get; set; }
        public Student student { get; set; }
        public Programme Programme { get; set; }
        public Cours Cours { get; set; }
        public List<Result> results { get; set; }
        public List<Cours> Course { get; set; }

    }
}