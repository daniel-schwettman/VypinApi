using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VypinApi.Models.Responses
{
    public class Authentication
    {
        public string status { get; set; }

        public string authToken { get; set; }
    }
}