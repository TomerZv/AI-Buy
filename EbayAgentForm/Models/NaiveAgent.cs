using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Models
{
    public enum Behavior
    {
        Average,
        BelowAverage,
        AboveAverage
    }

    public class NaiveAgent : IAgent
    {
        #region Consts
        
        private const int BELOW_AVERAGE_PRECENTAGE = 15;
        private const int ABOVE_AVERAGE_PRECENTAGE = 85;

        private const double BELOW_AVERAGE_MIN = 0.5;
        private const double BELOW_AVERAGE_MAX = 0.8;
        private const double AVERAGE_MIN = 0.8;
        private const double AVERAGE_MAX = 1.2;
        private const double ABOVE_AVERAGE_MIN = 1.2;
        private const double ABOVE_AVERAGE_MAX = 2; 

        #endregion

        #region Data Members

        public Auction Auction { get; set; }

        public string Name { get; set; }

        public int Price { get; set; }

        public Behavior Behavior { get; set; }

        private int FailedCount { get; set; }

        private HttpClient Client { get; set; } 

        #endregion

        public async Task ParticipateAuction()
        {
            // רצים על המכירה כל עוד היא לא הסתיימה ולא נצברו הרבה שגיאות 
            while (Auction.EndDate > DateTime.Now && FailedCount < 10)
            {
                HttpResponseMessage response = await Client.GetAsync("/GetAuction/" + Auction.Id);

                if (response.IsSuccessStatusCode)
                {
                    this.Auction = await response.Content.ReadAsAsync<Auction>();
                }
                else
                {
                    FailedCount++;
                }

                // בדיקה שההצעה האחרונה היא לא של הסוכן הנוכחי
                if (Auction.CurrentBid.Username != Name)
                {
                    // בדיקה שהסכום שהסוכן מוכן לתת גבוה מהמחיר המינמלי להצעה הבאה
                    if (Auction.CurrentPrice + Auction.MinBid < Price)
                    {
                        string postBody = string.Format(@"{""AuctionID"":{1},""Price"":{2},""Username"":""{3}"",""Date"":{4}""}",
                                                        Auction.Id,
                                                        Auction.CurrentPrice + Auction.MinBid,
                                                        Name,
                                                        DateTime.Now);

                        HttpResponseMessage postresponse = await Client.PostAsync("PlaceBidOnAuction", new StringContent(postBody, Encoding.UTF8, "application/json"));

                        if (!postresponse.IsSuccessStatusCode)
                        {
                            FailedCount++;
                        }
                    }
                    // נפסיק להציע במידה וסכום המכירה גבוה מהסכום שאנו מוכנים לשלם
                    else
                    {
                        break;
                    }
                }

                Thread.Sleep(1000);
            }
        }

        public async Task Initialize(HttpClient client)
        {
            this.Client = client;
            this.FailedCount = 0;

            // TODO : Make this real.
            this.Name = "Agent" + new Random().Next(1, 10000).ToString();
            var auctions = await GetAuctions(client);

            this.Auction = auctions[ChooseAuction(auctions.Count)];

            ChooseBehavior();

            ChoosePrice(this.Behavior, this.Auction.AvgPrice);
        }

        private async Task<List<Auction>> GetAuctions(HttpClient client)
        {
            var task = Task.Factory.StartNew(() => client.GetAsync("GetAuctions"));
            await task.Result;
            var temp = await task.Result.Result.Content.ReadAsAsync<List<Auction>>();
            return temp;
        }

        private void ChooseBehavior()
        {
            Random r = new Random();
            int n = r.Next(1, 101);
            this.Behavior = Behavior.Average;

            if (n <= BELOW_AVERAGE_PRECENTAGE)
            {
                this.Behavior = Behavior.BelowAverage;
            }
            else if (n >= ABOVE_AVERAGE_PRECENTAGE)
            {
                this.Behavior = Behavior.AboveAverage;
            }
        }

        private void ChoosePrice(Behavior b, int avg)
        {
            Random r = new Random();
            this.Price = 0;

            switch (b)
            {
                case Behavior.BelowAverage:
                    {
                        this.Price = r.Next((int)Math.Round(avg * BELOW_AVERAGE_MIN), (int)Math.Round(avg * BELOW_AVERAGE_MAX));

                        break;
                    }
                case Behavior.Average:
                    {
                        this.Price = r.Next((int)Math.Round(avg * AVERAGE_MIN), (int)Math.Round(avg * AVERAGE_MAX));

                        break;
                    }
                case Behavior.AboveAverage:
                    {
                        this.Price = r.Next((int)Math.Round(avg * ABOVE_AVERAGE_MIN), (int)Math.Round(avg * ABOVE_AVERAGE_MAX));

                        break;
                    }
            }
        }

        private int ChooseAuction(int count)
        {
            return new Random().Next(1, count + 1);
        }
    }
}
