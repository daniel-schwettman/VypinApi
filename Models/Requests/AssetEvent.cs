using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VypinApi.DataAccessLayer;

namespace VypinApi.Models.Requests
{
    public class AssetEvent
    {
        public List<Asset> Assets { get; set; }
    }
}