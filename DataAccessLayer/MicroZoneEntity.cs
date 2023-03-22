using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VypinApi.DataAccessLayer
{
    public partial class MicroZoneEntity
    {
        [Key]
        public int MicroZoneId { get; set; }
        public string RawId { get; set; }
        public string MicroZoneName { get; set; }
        public string MicroZoneNumber { get; set; }
        public string TagAssociationNumber { get; set; }
        public int DepartmentId { get; set; }
        public double MicroZoneX { get; set; }
        public double MicroZoneY { get; set; }
        public double MicroZoneWidth { get; set; }
        public double MicroZoneHeight { get; set; }
        public bool IsLocked { get; set; }
    }
}