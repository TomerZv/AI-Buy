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
        private static Dictionary<int, List<string>> AuctionsToAgents { get; set; }

        private static bool IsAuctionsStarted { get; set; }

        private static object FileLocker = new object();

        private static int fileCounter = 1;

        #endregion

        private BusinessLogic()
        {
            Auctions = new Dictionary<int, Tuple<Auction, object>>();
            AuctionsToAgents = new Dictionary<int, List<string>>();
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
            File.AppendAllText(@"c:\log\final.txt","Auc ID, Agent, Price" + Environment.NewLine);
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
                    auc.StartDate = DateTime.Now;
                    auc.Duration = int.Parse(values[3]);
                    auc.EndDate = auc.StartDate.AddMinutes(auc.Duration);
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

        public Auction GetAuction(int id)
        {
            return Auctions[id].Item1;
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

            bool didEnter = Monitor.TryEnter(locker);
            Auction auction;

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

                    lock (FileLocker)
                    {
                        File.AppendAllText(@"c:\log\log"+fileCounter+".txt", string.Format("User {0}, pays {1}, for auction id {2}, at {3}" + Environment.NewLine, bid.Username, bid.Price, bid.AuctionID, bid.Date));
                    }
                }

                Monitor.Exit(locker);
            }
            else
            {
                //Monitor.Wait(locker);
                auction = Auctions[bid.AuctionID].Item1;
            }

            result.DidSucceed = didEnter;
            return result;
        }

        public int ChooseAuctionForAgent(string agentName)
        {
            int selectedAuctionId;

            lock(AuctionsToAgents)
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

            return selectedAuctionId;
        }
    }
}