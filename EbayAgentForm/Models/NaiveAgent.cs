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

        public bool FailedInit { get; set; }
        #endregion

        public async Task ParticipateAuction()
        {
            // Waiting untill the auction is open.
            while (Auction.Status != AuctionStatus.Open) ;

            // רצים על המכירה כל עוד היא לא הסתיימה ולא נצברו הרבה שגיאות 
            while (Auction.Status == AuctionStatus.Open && FailedCount < 10)
            {
                HttpResponseMessage response = await Client.GetAsync("GetAuction?id=" + Auction.Id);

                if (!response.IsSuccessStatusCode)
                {
                    FailedCount++;
                }
                else
                {
                    this.Auction = await response.Content.ReadAsAsync<Auction>();

                    // בדיקה שההצעה האחרונה היא לא של הסוכן הנוכחי
                    if (Auction.CurrentBid == null || Auction.CurrentBid.Username != Name)
                    {
                        // בדיקה שהסכום שהסוכן מוכן לתת גבוה מהמחיר המינמלי להצעה הבאה
                        if (Auction.CurrentPrice + Auction.MinBid < Price)
                        {
                            string postBody = string.Format(@"{{""AuctionID"":{0},""Price"":{1},""Username"":""{2}"",""Date"":""{3}""}}",
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
                }

                Thread.Sleep(1000);
            }
        }

        public async Task Initialize(HttpClient client)
        {
            this.Client = client;
            this.FailedCount = 0;
            this.FailedInit = true;

            // TODO : Make this real.
            this.Name = "Agent" + new Random().Next(1, 10000).ToString();
            List<Auction> auctions = await GetAuctions(client);

            // Check request succeed
            if (auctions.Count != 0)
            {
                int auctionId = await ChooseAuction();

                this.Auction = auctions.First(a => a.Id == auctionId);

                //ChooseBehavior();

                ChoosePrice(this.Auction.AvgPrice, 0.25);

                this.FailedInit = false;
            }
        }

        private async Task<List<Auction>> GetAuctions(HttpClient client)
        {
            HttpResponseMessage response = await Client.GetAsync("GetAuctions");
            List<Auction> lst = new List<Auction>();

            if (response.IsSuccessStatusCode)
            {
               lst = await response.Content.ReadAsAsync<List<Auction>>();
            }

            return lst;
        }

        //private void ChooseBehavior()
        //{
        //    Random r = new Random();
        //    int n = r.Next(1, 101);
        //    this.Behavior = Behavior.Average;

        //    if (n <= BELOW_AVERAGE_PRECENTAGE)
        //    {
        //        this.Behavior = Behavior.BelowAverage;
        //    }
        //    else if (n >= ABOVE_AVERAGE_PRECENTAGE)
        //    {
        //        this.Behavior = Behavior.AboveAverage;
        //    }
        //}

        //sigma = סטיית תקן
        private void ChoosePrice(int avg, double sigma)
        {
            Random r = new Random();
            
            this.Price = 0;
            double number = generateNumber(r, sigma);

            this.Price = (int)Math.Round(number * avg);

            //switch (b)
            //{
            //    case Behavior.BelowAverage:
            //        {
            //            this.Price = r.Next((int)Math.Round(avg * BELOW_AVERAGE_MIN), (int)Math.Round(avg * BELOW_AVERAGE_MAX));

            //            break;
            //        }
            //    case Behavior.Average:
            //        {
            //            this.Price = r.Next((int)Math.Round(avg * AVERAGE_MIN), (int)Math.Round(avg * AVERAGE_MAX));

            //            break;
            //        }
            //    case Behavior.AboveAverage:
            //        {
            //            this.Price = r.Next((int)Math.Round(avg * ABOVE_AVERAGE_MIN), (int)Math.Round(avg * ABOVE_AVERAGE_MAX));

            //            break;
            //        }
            //}
        }

        public static double generateNumber(Random r, double sigma)
        {
            var u1 = r.NextDouble();
            var u2 = r.NextDouble();

            var rand_std_normal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                                Math.Sin(2.0 * Math.PI * u2);

            var rand_normal = 1 + sigma * rand_std_normal;

            return rand_normal;
        }

        private async Task<int> ChooseAuction()
        {
            HttpResponseMessage response = await Client.GetAsync("ChooseAuctionForAgent?agent=" + this.Name);

            int auctionId = -1; 

            if (response.IsSuccessStatusCode)
            {
                auctionId = await response.Content.ReadAsAsync<int>();
            }

            return auctionId;
        }


        
        
    }
}
