using AuctionManager.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;

namespace AuctionManager.Classes
{
    public sealed class BusinessLogic
    {
        #region Singleton
        private static volatile BusinessLogic instance;
        private static object syncRoot = new Object();
        public static BusinessLogic Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new BusinessLogic();
                    }
                }

                return instance;
            }
        }
        #endregion

        #region Data Members

        public Dictionary<int, Tuple<Auction, Object>> Auctions { get; set; }

        #endregion

        public void ReadAuctions()
        {
            Auctions.Clear();

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("~/Content/AITrainingData.csv");
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

            using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    var values = line.Split(',');

                    Auction auc = new Auction();
                    auc.ID = int.Parse(values[0]);
                    auc.Name = values[1];
                    auc.StartDate = DateTime.Parse(values[2]);
                    auc.EndDate = DateTime.Parse(values[3]);
                    auc.MinimumPrice = int.Parse(values[4]);
                    auc.Biddings = new List<Bid>();
                    auc.CurrentPrice = 0;

                    Auctions.Add(auc.ID, new Tuple<Auction, object>(auc, new Object()));       
                }
            }
        }

        public List<Auction> GetAllAuction()
        {
            List<Auction> allAuctions = new List<Auction>();

            foreach(Tuple<Auction, Object> item in Auctions.Values)
            {

                allAuctions.Add(item.Item1);
            }

            return allAuctions;
        }


        public Auction GetAuction(int auctionId)
        {
            return Auctions[auctionId].Item1;
        }

        public BidResult PlaceBidOnAuction(Bid bid)
        {
            BidResult result = new BidResult();

            if (Auctions[bid.AuctionID] == null)
            {
                result.DidSucceed = false;
                return result;
            }
            Object locker = Auctions[bid.AuctionID].Item2;

            bool didEnter = true;
            Auction auction;
            Monitor.TryEnter(locker, ref didEnter);
            if (didEnter)
            {
                auction = Auctions[bid.AuctionID].Item1;

                // Place bid only if the bid is higher than the current price of the auction.
                if (auction.CurrentPrice >= bid.Price)
                {
                    didEnter = false;
                }
                else
                {
                    auction.Biddings.Add(bid);
                    auction.CurrentPrice = bid.Price;
                }
            }
            else
            {
                Monitor.Wait(locker);
                auction = Auctions[bid.AuctionID].Item1;
            }

            Monitor.Exit(locker);

            result.DidSucceed = didEnter;
            return result;
        }
    }
}