using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VypinApi.Models
{
    public class TagLocation
    {
        public string TagId { get; set; }

        public string LastUpdatedOnServer { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }
    }
}