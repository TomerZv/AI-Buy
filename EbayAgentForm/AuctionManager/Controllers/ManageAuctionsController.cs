using AuctionManager.Classes;
using AuctionManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AuctionManager.Controllers
{
    public class ManageAuctionsController : ApiController
    {
        [HttpPost()]
        public BidResult PlaceBidOnAuction(Bid bid)
        {
            return BusinessLogic.Instance.PlaceBidOnAuction(bid);
        }

        [HttpGet()]
        public Auction GetAuction(int auctionId)
        {
            return BusinessLogic.Instance.GetAuction(auctionId);
        }

        public void ReadAuctions()
        {
            BusinessLogic.Instance.ReadAuctions();
        }
    }
}
