using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VypinApi.Models.Responses
{
    public class DepartmentResult
    {
        public int DepartmentId { get; set; }
        public string Name { get; set; }
        public string FilePath { get; set; }
        public bool IsLastLoaded { get; set; }
        public double ScreenWidth { get; set; }
        public double ScreenHeight { get; set; }
    }
}