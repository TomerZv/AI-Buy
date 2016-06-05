using AuctionManager.Models;
using Models;
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

        private static Dictionary<int, Tuple<Auction, Object>> Auctions { get; set; }

        private static bool IsAuctionsStarted { get; set; }

        #endregion

        private BusinessLogic()
        {
            Auctions = new Dictionary<int, Tuple<Auction, object>>();
            IsAuctionsStarted = false;
        }

        public bool IsStarted()
        {
            return IsAuctionsStarted;
        }

        public void ReadAuctions()
        {
            Auctions.Clear();

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://localhost:8670/Content/AITrainingData.csv");
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

            using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
            {
                sr.ReadLine();

                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    var values = line.Split(',');

                    Auction auc = new Auction();
                    auc.Id = int.Parse(values[0]);
                    auc.ItemType = values[1];
                    auc.StartDate = DateTime.Now;
                    auc.Duration = int.Parse(values[3]);
                    auc.EndDate = auc.StartDate.AddDays(auc.Duration);
                    auc.MinimumPrice = int.Parse(values[4]);
                    auc.AvgPrice = int.Parse(values[5]);
                    auc.MinBid = int.Parse(values[6]);
                    auc.Biddings = new List<Bid>();

                    Auctions.Add(auc.Id, new Tuple<Auction, object>(auc, new Object()));       
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

        public void InitAuctions()
        {
            IsAuctionsStarted = true;
        }

        public Auction GetAuction(int auctionId)
        {
            return Auctions[auctionId].Item1;
        }

        public BidResult PlaceBidOnAuction(Bid bid)
        {
            BidResult result = new BidResult();

            if (!Auctions.ContainsKey(bid.AuctionID))
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
                    bid.ID = Guid.NewGuid();
                    auction.Biddings.Add(bid);
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