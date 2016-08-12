using AuctionManager.Classes;
using Models;
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

        [Route("api/ManageAuctions/PrintResults")]
        [HttpGet()]
        public bool PrintResults()
        {
            return BusinessLogic.Instance.PrintResults();
        }

        [Route("api/ManageAuctions/IsStarted")]
        [HttpGet()]
        public bool IsStarted()
        {
            return BusinessLogic.Instance.IsStarted();
        }

        [Route("api/ManageAuctions/InitAuctions")]
        [HttpGet()]
        public string InitAuctions()
        {
            BusinessLogic.Instance.InitAuctions();

            return "success";
        }

        [Route("api/ManageAuctions/GetAuctions")]
        [HttpGet()]
        public List<Auction> GetAuctions()
        {
            return BusinessLogic.Instance.GetAllAuction();
        }

        [Route("api/ManageAuctions/GetAuctionsFromFile")]
        [HttpGet()]
        public bool GetAuctionsFromFile()
        {
            BusinessLogic.Instance.ReadAuctions();
            return true;
        }

        [Route("api/ManageAuctions/ChooseAuctionForAgent")]
        [HttpGet()]
        public int ChooseAuctionForAgent(string agent)
        {
            return BusinessLogic.Instance.ChooseAuctionForAgent(agent);
        }
    }
}
