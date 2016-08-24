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

        private static Dictionary<int, Auction> Auctions { get; set; }
        private static Dictionary<int, Object> Lockers { get; set; }
        private static Dictionary<int, List<string>> AuctionsToAgents { get; set; }
        private static List<string> AllAgents { get; set; }

        private static bool IsAuctionsStarted { get; set; }

        private static object FileLocker = new object();

        private static int fileCounter = 1;

        #endregion

        private BusinessLogic()
        {
            Auctions = new Dictionary<int, Auction>();
            Lockers = new Dictionary<int, Object>();
            AuctionsToAgents = new Dictionary<int, List<string>>();
            AllAgents = new List<string>();

            IsAuctionsStarted = false;

            while (File.Exists(@"c:\log\log" + fileCounter + ".txt"))
            {
                fileCounter++;
            }
        }

        public bool IsStarted()
        {
            return IsAuctionsStarted;
        }

        public bool PrintResults()
        {
            File.AppendAllText(@"c:\log\final.txt", "Auc ID, Agent, Price" + Environment.NewLine);



            int totalAuction = this.GetAllAuction().Count;
            double sumAllPrices = 0;
            int multiAgentWinsCount = 0;
            double minPriceMultiWin = double.MaxValue;

            foreach (Auction auc in this.GetAllAuction())
            {
                sumAllPrices += auc.CurrentPrice;

                if (auc.CurrentBid.Username.StartsWith("Multi"))
                {
                    multiAgentWinsCount++;
                    if (minPriceMultiWin > auc.CurrentBid.Price)
                    {
                        minPriceMultiWin = auc.CurrentBid.Price;
                    }
                }
            }

            double avgPrice = sumAllPrices / totalAuction;
            double midPrice = this.GetAllAuction().OrderBy(x => x.CurrentPrice).ToList().ElementAt((int)(totalAuction / 2)).CurrentPrice;

            if (!File.Exists(@"c:\log\Summary.txt"))
            {
                File.AppendAllText(@"c:\log\Summary.txt", "Total Naive Agents, Avg Price, Mid Price, Total Multi won, Multi min price won" + Environment.NewLine);
            }

            File.AppendAllText(@"c:\log\Summary.txt", string.Format("{0},{1},{2},{3},{4} {5}", AllAgents.Count, avgPrice, midPrice, multiAgentWinsCount, minPriceMultiWin, Environment.NewLine));

            foreach (Auction auc in this.GetAllAuction())
            {
                string userName = "No Winner";
                string price = "No Price";
                if (auc.CurrentBid != null)
                {
                    userName = auc.CurrentBid.Username;
                    price = auc.CurrentBid.Price.ToString();
                }

                File.AppendAllText(@"c:\log\final.txt", string.Format("{0},{1},{2},{3}", auc.Id, userName, price, Environment.NewLine));
            }

            return true;
        }

        public void ReadAuctions()
        {
            DateTime now = DateTime.Now;
            Auctions.Clear();

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://localhost:8670/Content/AITrainingDataSmall.csv");
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
                    auc.StartDate = now;
                    auc.Duration = int.Parse(values[3]);
                    auc.EndDate = auc.StartDate.AddMinutes(auc.Duration);
                    auc.MinimumPrice = int.Parse(values[4]);
                    auc.AvgPrice = int.Parse(values[5]);
                    auc.MinBid = int.Parse(values[6]);
                    auc.Biddings = new List<Bid>();

                    Auctions.Add(auc.Id, auc);
                    Lockers.Add(auc.Id, new Object());
                }
            }
        }

        public List<Auction> GetAllAuction()
        {
            List<Auction> allAuctions = new List<Auction>();
            List<int> keys = Auctions.Keys.ToList();


            foreach (int key in keys)
            {
                allAuctions.Add(Auctions[key]);
            }


            return allAuctions;
        }

        public void InitAuctions()
        {
            IsAuctionsStarted = true;
        }

        public Auction GetAuction(int id)
        {
            return Auctions[id];
        }

        public BidResult PlaceBidOnAuction(Bid bid)
        {
            BidResult result = new BidResult();

            if (!Auctions.ContainsKey(bid.AuctionID))
            {
                result.DidSucceed = false;
                return result;
            }
            Object locker = Lockers[bid.AuctionID];

            bool didEnter = Monitor.TryEnter(locker);
            Auction auction;

            if (didEnter)
            {
                auction = Auctions[bid.AuctionID];

                // Place bid only if the bid is higher than the current price of the auction and its still open
                if (auction.CurrentPrice >= bid.Price || auction.Status != AuctionStatus.Open)
                {
                    didEnter = false;
                }
                else
                {
                    bid.ID = Guid.NewGuid();
                    auction.Biddings.Add(bid);

                    lock (FileLocker)
                    {
                        File.AppendAllText(@"c:\log\log" + fileCounter + ".txt", string.Format("User {0}, pays {1}, for auction id {2}, at {3}" + Environment.NewLine, bid.Username, bid.Price, bid.AuctionID, bid.Date));
                    }
                }

                Monitor.Exit(locker);
            }
            else
            {
                //Monitor.Wait(locker);
                auction = Auctions[bid.AuctionID];
            }

            result.DidSucceed = didEnter;
            return result;
        }

        public int ChooseAuctionForAgent(string agentName)
        {
            int selectedAuctionId;

            lock (AuctionsToAgents)
            {
                List<int> emptyAuctions = Auctions.Keys.Where(auctionId => !AuctionsToAgents.Keys.Contains(auctionId)).ToList();

                if (emptyAuctions.Count != 0)
                {
                    int index = new Random().Next(0, emptyAuctions.Count);

                    selectedAuctionId = emptyAuctions[index];

                    AuctionsToAgents.Add(selectedAuctionId, new List<string>());
                }
                else
                {
                    int index = new Random().Next(0, Auctions.Count);

                    selectedAuctionId = Auctions.Keys.ElementAt(index);
                }

                AuctionsToAgents[selectedAuctionId].Add(agentName);
            }

            if (!AllAgents.Contains(agentName))
            {
                AllAgents.Add(agentName);
            }

            return selectedAuctionId;
        }
    }
}