using AuctionManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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