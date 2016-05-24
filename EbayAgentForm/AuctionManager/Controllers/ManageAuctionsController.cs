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
        public BidResult PlaceBidOnAuction([FromBody] Bid bid)
        {
            return BusinessLogic.Instance.PlaceBidOnAuction(bid);
        }

        [HttpGet()]
        public Auction GetAuction(int id)
        {
            return BusinessLogic.Instance.GetAuction(id);
        }

        [HttpGet()]
        public List<Auction> GetAuctions()
        {
            return BusinessLogic.Instance.GetAllAuction();
        }

        [HttpGet()]
        public void ReadAuctions()
        {
            BusinessLogic.Instance.ReadAuctions();
        }
    }
}
