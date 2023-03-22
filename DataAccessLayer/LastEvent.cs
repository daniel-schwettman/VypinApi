using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VypinApi.DataAccessLayer
{
    public class LastEvent
    {
        public string TagId { get; set; }
        public string TagName { get; set; }
        public int Rssi { get; set; }
        public string StatusCode { get; set; }
        public string Battery { get; set; }
        public double Latitude { get; set; }
        public int Mzone1Rssi_Tmp { get; set; }
        public double Longitude { get; set; }
        public string TagType { get; set; }
        public string Raw { get; set; }
        public string Mzone1 { get; set; }
        public string Mzone2 { get; set; }
        public DateTime ReceivedOn { get; set; }
        public List<Asset> AssociatedAssets { get; set; }
        public string SequenceNumber { get; set; }
        public string Temperature { get; set; }
        public string Humidity { get; set; }
    }
}