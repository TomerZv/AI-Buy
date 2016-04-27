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
        public int ID { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; }
        public int MinimumPrice { get; set; }
        public string Name { get; set; }
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