using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Models;
using System.Data.Entity;
using System.Threading;
using System.IO;

namespace BasicAgentForm
{
    public partial class Form1 : Form
    {
        private const int BELOW_AVERAGE_PRECENTAGE = 15;
        private const int ABOVE_AVERAGE_PRECENTAGE = 85;

        private const double BELOW_AVERAGE_MIN = 0.5;
        private const double BELOW_AVERAGE_MAX = 0.8;
        private const double AVERAGE_MIN = 0.8;
        private const double AVERAGE_MAX = 1.2;
        private const double ABOVE_AVERAGE_MIN = 1.2;
        private const double ABOVE_AVERAGE_MAX = 2;

        public Auction Auction { get; set; }
        public Behavior Behavior { get; set; }
        public int Price { get; set; }

        public int FailedCount  {get; set; }

        public string AgentName { get; set; }

        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var client = new HttpClient();
            {
                client.BaseAddress = new Uri(@"http://localhost:8670/api/manageauctions/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                bool isStarted = false;

                while (!isStarted)
                {
                    HttpResponseMessage response = await client.GetAsync("IsStarted");

                    if (response.IsSuccessStatusCode)
                    {
                        isStarted = await response.Content.ReadAsAsync<bool>();
                    }
                }

                await InitializeAgent(client);

                //string postBody = @"{""ID"":2,""AuctionID"":1,""Price"":200,""Username"":""Tomer"",""Date"":""2015-12-17T03:24:00""}";
                //HttpResponseMessage x = await client.PostAsync(resourceAddress, new StringContent(postBody, Encoding.UTF8, "application/json"));
                //MessageBox.Show(x.IsSuccessStatusCode.ToString());
            }
            MessageBox.Show(this.Behavior.ToString() + " " + this.Price + " " + this.Auction.Id); 
        }

        private async Task InitializeAgent(HttpClient client)
        {
            FailedCount = 0;
            AgentName = "Agent" + new Random().Next(1, 10000).ToString();
            var auctions = await GetAuctions(client);

            this.Auction = auctions[ChooseAuction(auctions.Count)];

            ChooseBehavior();

            ChoosePrice(this.Behavior, this.Auction.AvgPrice);

            try
            {
                while (this.Auction.EndDate > DateTime.Now && FailedCount < 10)
                {
                    HttpResponseMessage response = await client.GetAsync("/GetAuction/" + this.Auction.Id);

                    if (response.IsSuccessStatusCode)
                    {
                        this.Auction = await response.Content.ReadAsAsync<Auction>();
                    }
                    else
                    {
                        FailedCount++;
                    }

                    if (this.Auction.CurrentPrice + this.Auction.MinBid < this.Price)
                    {
                        string postBody = string.Format(@"{""AuctionID"":{1},""Price"":{2},""Username"":""{3}"",""Date"":{4}""}",
                                                        this.Auction.Id,
                                                        this.Auction.CurrentPrice + this.Auction.MinBid,
                                                        this.AgentName,
                                                        DateTime.Now);

                        HttpResponseMessage postresponse = await client.PostAsync("PlaceBidOnAuction", new StringContent(postBody, Encoding.UTF8, "application/json"));

                        if (!postresponse.IsSuccessStatusCode)
                        {
                            FailedCount++;
                        }
                    }
                    else
                    {
                        break;
                    }

                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText("errors.txt", ex.Message.ToString());
            }
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

    public enum Behavior{
        Average,
        BelowAverage,
        AboveAverage
    }
}
