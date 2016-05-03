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

namespace FormAgent
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string resourceAddress = @"http://localhost:8670/api/manageauctions";
            string postBody = @"{""ID"":2,""AuctionID"":1,""Price"":200,""Username"":""Tomer"",""Date"":""2015-12-17T03:24:00""}";        
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage x = await httpClient.PostAsync(resourceAddress, new StringContent(postBody, Encoding.UTF8, "application/json"));

            MessageBox.Show(x.IsSuccessStatusCode.ToString());
        }
    }
}
