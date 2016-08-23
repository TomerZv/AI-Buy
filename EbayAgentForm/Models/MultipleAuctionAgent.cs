using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Models
{
    public class MultipleAuctionAgent : IAgent
    {
        const double PERCENT_FROM_AVG = 0.8;

        public Auction Auction { get; set; }

        public string Name { get; set; }

        public int FailedCount { get; set; }

        private HttpClient Client { get; set; }

        static Random random = new Random();

        public bool IsWinAuction { get; set; }

        public int AuctionsFinishedCount { get; set; }

        public List<Auction> AuctionToParticipate {get; set;}

        public List<Auction> RelevantAuctions { get; set; }

        public int Price { get; set; }

        public bool FailedInit { get; set; }

        public async Task Initialize(HttpClient client)
        {
            this.Client = client;
            this.FailedCount = 0;

            this.Name = "Multi Agent " + random.Next(1, 10000).ToString();
            var auctions = await GetAuctions(client);

            int amountOfAuctionToParticipate = (auctions.Count / 2) > 10 ? 10 : (auctions.Count / 2);
            AuctionToParticipate = auctions.OrderBy(x => x.EndDate).Take(amountOfAuctionToParticipate).ToList();

            this.Price = (int)Math.Round(auctions.First().AvgPrice * PERCENT_FROM_AVG);
        }

        public async Task ParticipateAuction()
        {
            List<int> leadingAuctions = new List<int>();
            List<Auction> allAuctions = await this.GetAuctions(Client);
            List<Auction> relevantAuctions = allAuctions.Where(a => a.Status != AuctionStatus.Close).ToList();

            // Runs through only if didn't win any auction yet and there are still open auctions that the agent is able to participate in.
            while (!IsWinAuction && relevantAuctions.Count != 0) 
            {
                relevantAuctions = relevantAuctions.OrderBy(x => x.EndDate).ToList();
                relevantAuctions = relevantAuctions.Where(a => a.EndDate == relevantAuctions.First().EndDate).ToList();
                relevantAuctions = relevantAuctions.OrderBy(a => a.CurrentPrice).ToList();

                // Gets the auction that has the lowest bid and ends ASAP.
                var auctionToParticipate = relevantAuctions.First();

                bool isLead = false;

                List<int> auctionsToRemove = new List<int>();

                // Validates on which auction this agent is still on the lead
                foreach (var curr in leadingAuctions)
                {
                    var currentLeadingAgent = allAuctions.Where(a => a.Id == curr).First().CurrentBid.Username;

                    if (currentLeadingAgent == this.Name)
                    {
                        isLead = true;
                    }
                    else {
                        auctionsToRemove.Add(curr);
                    }
                }

                auctionsToRemove.ForEach(a => leadingAuctions.Remove(a));

                // Only if the agent isn't on the lead of any auction, participates in the lowest bid auction.
                if (!isLead) {
                    // Checks that the agent is willing to pay for this auction.
                    if ((auctionToParticipate.CurrentPrice + auctionToParticipate.MinBid < Price) && (auctionToParticipate.Status == AuctionStatus.Open))
                    {
                        string postBody = string.Format(@"{{""AuctionID"":{0},""Price"":{1},""Username"":""{2}"",""Date"":""{3}""}}",
                                                        auctionToParticipate.Id,
                                                        auctionToParticipate.CurrentPrice + auctionToParticipate.MinBid,
                                                        Name,
                                                        DateTime.Now);

                        HttpResponseMessage postresponse = await Client.PostAsync("PlaceBidOnAuction", new StringContent(postBody, Encoding.UTF8, "application/json"));

                        if (!postresponse.IsSuccessStatusCode)
                        {
                            FailedCount++;
                        }
                        else 
                        {
                            // Participated in the auction - adds it to the list of leadingAuctions.
                            leadingAuctions.Add(auctionToParticipate.Id);
                        }
                    }
                }

                allAuctions = await this.GetAuctions(Client);
                relevantAuctions = allAuctions.Where(a => a.Status != AuctionStatus.Close).ToList();
            }
        }

        private async void CheckWinAuctions()
        {
            foreach (Auction auc in AuctionToParticipate)
            {
                HttpResponseMessage response = await Client.GetAsync("GetAuction?id=" + auc.Id);

                if (response.IsSuccessStatusCode)
                {
                    Auction auc2 = await response.Content.ReadAsAsync<Auction>();

                    if (auc2.Status == AuctionStatus.Close && auc2.CurrentBid != null && auc2.CurrentBid.Username == this.Name)
                    {
                        IsWinAuction = true;
                        break;
                    }
                }
            }
        }

        private async Task<List<Auction>> GetAuctions(HttpClient client)
        {
            var task = Task.Factory.StartNew(() => client.GetAsync("GetAuctions"));
            await task.Result;
            var temp = await task.Result.Result.Content.ReadAsAsync<List<Auction>>();

            return temp;
        }
    }
}
