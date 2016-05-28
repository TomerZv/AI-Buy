using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Models
{
    public class Bid
    {
        [Key]
        public int ID { get; set; }
        public int AuctionID { get; set; }
        public int Price { get; set; }
        public string Username { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }
    }
}