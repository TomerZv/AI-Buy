using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AuctionStatusReporter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;

            Thread refresh = new Thread(new ThreadStart(DoUpdate));

            refresh.Start();
        }

        private async void DoUpdate()
        {
            while (true)
            {
                var client = new HttpClient();
                {
                    client.BaseAddress = new Uri(@"http://localhost:8670/api/manageauctions/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = await client.GetAsync("GetAuctions");
                    List<Auction> lst = new List<Auction>();

                    if (response.IsSuccessStatusCode)
                    {
                        lst = await response.Content.ReadAsAsync<List<Auction>>();

                        string text = string.Empty;

                        lst = lst.OrderBy(item => item.Id).ToList();

                        foreach (var item in lst)
                        {
                            if (item.CurrentBid == null)
                            {
                                text += "Auction " + item.Id + " - No bids has been placed yet" + Environment.NewLine;
                            }
                            else
                            {
                                text += "Auction " + item.Id + " - Current bid : " + item.CurrentBid.Price + ", " + item.CurrentBid.Username + Environment.NewLine;
                            }
                        }

                        label1.Text = text;
                    }
                }

                Thread.Sleep(2500);
            }
        }
    }
}
