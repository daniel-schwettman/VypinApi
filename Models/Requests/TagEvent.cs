using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VypinApi.DataAccessLayer;

namespace VypinApi.Models.Requests
{
    public class TagEvent
    {
        public string ReaderId { get; set; }
        public List<Tag> Tags { get; set; }
        public string SessionId { get; set; }
        public string ReceivedOn { get; set; }
        public string Version { get; set; }
    }
}