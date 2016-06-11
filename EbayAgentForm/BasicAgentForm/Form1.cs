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
        public IAgent Agent { get; set; }

        public Form1()
        {
            InitializeComponent();

            string[] args = Environment.GetCommandLineArgs();

            if (args.Length > 1)
            {
                if (args[1] == "Init")
                {
                    button2.PerformClick();
                }
                else
                {
                    comboBox1.Text = args[1];
                    button1.PerformClick();
                }
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "Naive")
            {
                Agent = new NaiveAgent();            
            }
            else
            {
                Agent = new MultipleAuctionAgent();
            }
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

                await Agent.Initialize(client);

                if (!Agent.FailedInit)
                {
                    await Agent.ParticipateAuction();
                }
            }
           
        }

        private async void InitializeServer(object sender, EventArgs e)
        {
            var client = new HttpClient();
            {
                client.BaseAddress = new Uri(@"http://localhost:8670/api/manageauctions/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                await client.GetAsync("GetAuctionsFromFile");
                await client.GetAsync("InitAuctions");

                MessageBox.Show("Finished initlization");
            }
        }
    }
}
