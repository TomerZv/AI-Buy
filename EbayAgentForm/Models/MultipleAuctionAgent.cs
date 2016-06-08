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

        public async Task Initialize(HttpClient client)
        {
            this.Client = client;
            this.FailedCount = 0;

            // TODO : Make this real.
            this.Name = "Agent" + random.Next(1, 10000).ToString();
            var auctions = await GetAuctions(client);

            List<int> indexes = GenerateRandom(auctions.Count / 2, auctions.Count);

            AuctionToParticipate = new List<Auction>();

            foreach (int index in indexes)
            {
                AuctionToParticipate.Add(auctions.ElementAt(index));
            }

            AuctionToParticipate = AuctionToParticipate.OrderBy(x => x.EndDate).ToList();
            
            this.Price = (int)Math.Round(auctions.First().AvgPrice * PERCENT_FROM_AVG);
        }

        public static List<int> GenerateRandom(int count, int maxNum)
        {
            int number;

            // generate count random values.
            List<int> randomNumbers = new List<int>();
            for (int i = 0; i < maxNum / 2; i++)
			{
                do
                {
                    number = random.Next(0, maxNum);
                } while (randomNumbers.Contains(number));

                randomNumbers.Add(number);
			}

            return randomNumbers;
     
        }

        public async Task ParticipateAuction()
        {
            RelevantAuctions = new List<Auction>(AuctionToParticipate);
            
            int i = 0;
            Auction currentAuction;

            while (!IsWinAuction && RelevantAuctions.Count > 0)
            {
                currentAuction = RelevantAuctions.ElementAt(i);

                HttpResponseMessage response = await Client.GetAsync("GetAuction?id=" + currentAuction.Id);

                if (!response.IsSuccessStatusCode)
                {
                    FailedCount++;
                }
                else
                {
                    currentAuction = await response.Content.ReadAsAsync<Auction>();

                    if (currentAuction.Status != AuctionStatus.Open)
                    {
                        if (currentAuction.Status == AuctionStatus.Close)
                        {
                            RelevantAuctions.Remove(currentAuction);
                        }
                        else
                        {
                            i++;
                        }

                        continue;
                    }
                    else
                    {

                        // בדיקה שההצעה האחרונה היא לא של הסוכן הנוכחי
                        if (currentAuction.CurrentBid == null || currentAuction.CurrentBid.Username != Name)
                        {
                            // בדיקה שהסכום שהסוכן מוכן לתת גבוה מהמחיר המינמלי להצעה הבאה
                            if (currentAuction.CurrentPrice + currentAuction.MinBid < Price)
                            {
                                string postBody = string.Format(@"{{""AuctionID"":{0},""Price"":{1},""Username"":""{2}"",""Date"":""{3}""}}",
                                                                currentAuction.Id,
                                                                currentAuction.CurrentPrice + currentAuction.MinBid,
                                                                Name,
                                                                DateTime.Now);

                                HttpResponseMessage postresponse = await Client.PostAsync("PlaceBidOnAuction", new StringContent(postBody, Encoding.UTF8, "application/json"));

                                if (!postresponse.IsSuccessStatusCode)
                                {
                                    FailedCount++;
                                }

                                i++;
                            }
                            // נפסיק להציע במידה וסכום המכירה גבוה מהסכום שאנו מוכנים לשלם
                            else
                            {
                                RelevantAuctions.Remove(currentAuction);
                            }
                        }
                        else
                        {
                            i++;
                        }
                    }
                }

                CheckWinAuctions();
                Thread.Sleep(1000);
            }
        }

        private async void CheckWinAuctions()
        {
            foreach (Auction auc in AuctionToParticipate)
            {
                HttpResponseMessage response = await Client.GetAsync("GetAuction?id=" + auc.Id);
                Auction auc2 = await response.Content.ReadAsAsync<Auction>();

                if (auc2.Status == AuctionStatus.Close && auc2.CurrentBid.Username == this.Name)
                {
                    IsWinAuction = true;
                    break;
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
