using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class BidResult
    {
        public Auction Auction { get; set; }
        public bool DidSucceed { get; set; }
    }
}