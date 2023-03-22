using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VypinApi.Models.Responses
{
    public class AssetResult
    {
        public int AssetId { get; set; }

        public string Name { get; set; }

        public int? SlotId { get; set; }

        public int TagId { get; set; }

        public bool IsActive { get; set; }

        public string AssetIdentifier { get; set; }
    }
}