using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AuctionManager.Models
{
    public class Auction
    {
        [Key]
        public int Id { get; set; }
        public string ItemType { get; set; }
        public int StartDate { get; set; }
        public int Duration { get; set; }
        public int MinimumPrice { get; set; }
        public int AvgPrice { get; set; }
        public int MinBid { get; set; }
        public int CurrentPrice { get; set; }

        public List<Bid> Biddings { get; set; }

        public AuctionStatus Status { get; set; }
    }
    
    public enum AuctionStatus
    {
        Future,
        Open,
        Close
    }
}