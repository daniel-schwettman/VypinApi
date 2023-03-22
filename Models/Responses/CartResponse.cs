using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VypinApi.Models.Responses
{
    public class CartResponse
    {
        public List<CartResult> CartResults { get; set; }
    }
}