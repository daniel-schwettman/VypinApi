using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VypinApi.Models.Requests
{
    public class TagLocationRequest
    {
        public List<string> TagIds { get; set; }
    }
}