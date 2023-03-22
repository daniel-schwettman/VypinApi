using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VypinApi.DataAccessLayer
{
    public partial class TagHistory
    {
        public int Id { get; set; }

        public string RawId { get; set; }

        public string Version { get; set; }

        public string Rssi { get; set; }

        public string mZone1Rssi { get; set; }

        public DateTime LastUpdatedOn { get; set; }

        public string StatusCode { get; set; }

        public DateTime ReceivedOn { get; set; }

        public string Battery { get; set; }

        public string Sensor1 { get; set; }

        public string TagType { get; set; }

        public string FirmwareVersion { get; set; }

        public string Raw { get; set; }

        public string MicroZoneCurrent { get; set; }

        public string MicroZonePrevious { get; set; }

        public string Name { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string Type { get; set; }

        public string Category { get; set; }

        public string ReaderId { get; set; }
        public string SequenceNumber { get; set; }
        public string Operation { get; set; }

        public int BeaconCount { get; set; }
        public string Temperature { get; set; }
        public string Humidity { get; set; }
    }
}