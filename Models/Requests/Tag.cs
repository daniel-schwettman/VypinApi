using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VypinApi.DataAccessLayer;

namespace VypinApi.Models.Requests
{
    public class Tag
    {
        public int RssiSampleCount { get; set; }
        [JsonProperty("last-event")]
        public LastEvent LastEvent { get; set; }
        public string Mzone1Rssi { get; set; }
    }
}