using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VypinApi.DataAccessLayer;

namespace VypinApi.Models.Requests
{
    public class LegacyTag
    {
        public string TagName { get; set; }
        public string Longitude { get; set; }
        public string StatusCode { get; set; }
        public string TagId { get; set; }
        public string MfgHex { get; set; }
        public string TagType { get; set; }
        public string Battery { get; set; }
        public List<string> MZones { get; set; }
        public string Mzone1 { get; set; }
        public string Mzone2 { get; set; }
        public string Mzone3 { get; set; }
        public string ReaderId { get; set; }
        public string SequenceId { get; set; }
        public string Sensor1 { get; set; }
        public string Sensor2 { get; set; }
        public string Sensor3 { get; set; }
        public string Raw { get; set; }
        public string Version { get; set; }
        public string Latitude { get; set; }
        public string Mzone1Rssi { get; set; }
        public DateTime ReceivedOn { get; set; }
        public string Rssi { get; set; }
        public List<Asset> AssociatedAssets { get; set; }
        public string Temperature { get; set; }
        public string Humidity { get; set; }
    }
}