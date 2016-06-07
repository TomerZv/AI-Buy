using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Models
{
    public class Auction
    {
        [Key]
        public int Id { get; set; }
        public string ItemType { get; set; }
        public DateTime StartDate { get; set; }
        public int Duration { get; set; }
        public DateTime EndDate { get; set; }
        public int MinimumPrice { get; set; }
        public int AvgPrice { get; set; }
        public int MinBid { get; set; }
        public List<Bid> Biddings { get; set; }
        public AuctionStatus Status
        {
            get
            {
                AuctionStatus status = AuctionStatus.Close;

                if (DateTime.Now < this.StartDate) status = AuctionStatus.Future;
                if ((DateTime.Now >= this.StartDate) && (DateTime.Now <= this.EndDate)) status = AuctionStatus.Open;
                if (DateTime.Now > this.EndDate) status = AuctionStatus.Close;

                return status;
            }
        }

        public Bid CurrentBid
        {
            get
            {
                if (this.Biddings.Count == 0)
                {
                    return null;
                }

                return this.Biddings.Last();
            }
        }

        public int CurrentPrice
        {
            get
            {
                if (this.CurrentBid == null)
                {
                    return 0;
                }

                return CurrentBid.Price;
            }
        }
    }

    public enum AuctionStatus
    {
        Future,
        Open,
        Close
    }
}
