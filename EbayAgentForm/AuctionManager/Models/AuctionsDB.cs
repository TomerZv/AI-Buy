using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AuctionManager.Models
{
    public class AuctionsDB : System.Data.Entity.DbContext
    {
        public DbSet<Auction> auctions { get; set; }

        public AuctionsDB() : base("DefaultConnection")
        {

        }
    }
}