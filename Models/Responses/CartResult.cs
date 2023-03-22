using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VypinApi.Models.Responses
{
    public class CartResult
    {
        public int Id { get; set; }
        public string TagId { get; set; }
        public string CartNumber { get; set; }
    }
}