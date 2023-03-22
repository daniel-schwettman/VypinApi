using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VypinApi.Models.Requests
{
    public class LegacyTagEvent
    {
        public string ReaderId { get; set; }
        public List<LegacyTag> Tags { get; set; }
        public string SessionId { get; set; }
        public DateTime ReceivedOn { get; set; }
        public string Version { get; set; }
    }
}