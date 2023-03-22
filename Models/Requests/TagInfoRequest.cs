using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VypinApi.Models.Requests
{
    public class TagInfoRequest
    {
        public string TagName { get; set; }

        public string TagId { get; set; }

        public string TagType { get; set; }

        public string TagCategory { get; set; }

		public bool InclusiveSearch { get; set; }

        public string SearchText { get; set;  }
    }
}