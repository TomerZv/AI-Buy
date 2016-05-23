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

namespace FormAgent
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();


        }

        void timer1_Tick(object sender, EventArgs e)
        {
            TimeSpan TimeRemaining1 = new DateTime(2016,5,27,13,23,10) - DateTime.Now;
            TimeSpan TimeRemaining2 = new DateTime(2016, 5, 28, 6, 42, 19) - DateTime.Now;
            textBox8.Text = TimeRemaining1.Hours + " : " + TimeRemaining1.Minutes + " : " + TimeRemaining1.Seconds;
            textBox9.Text = TimeRemaining2.Hours + " : " + TimeRemaining2.Minutes + " : " + TimeRemaining2.Seconds;
            textBox8.Refresh();
            textBox9.Refresh();

        }

        void timer2_Tick(object sender, EventArgs e)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(dataGridView1);
            row.Cells[0].Value = "Agent4";
            row.Cells[1].Value = "580 $";
            row.Cells[2].Value = DateTime.Now;
            dataGridView1.Rows.Insert(0, row);

            textBox7.Text = "580 $";

            timer2.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*string resourceAddress = @"http://localhost:8670/api/manageauctions";
            string postBody = @"{""ID"":2,""AuctionID"":1,""Price"":200,""Username"":""Tomer"",""Date"":""2015-12-17T03:24:00""}";        
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage x = await httpClient.PostAsync(resourceAddress, new StringContent(postBody, Encoding.UTF8, "application/json"));

            MessageBox.Show(x.IsSuccessStatusCode.ToString());*/
            Thread.Sleep(1200);

            timer1.Tick += timer1_Tick;
            timer1.Interval = 1000;
            timer1.Enabled = true;
            timer1.Start();

            textBox7.Text = "560 $";
            textBox10.Text = "650 $";
            textBox5.Text = "Apple iPhone 6S 64GB Silver";
            textBox6.Text = "Apple iPhone 6S 64GB Black";


            textBox3.Text = "http://www.ebay.com/itm/Apple-iPhone-6-64GB-Space-Gray-Sprint/";
            textBox4.Text = "http://www.ebay.com/itm/Apple-iPhone-6S-Latest-Model-64GB-Silver-AT-T-now-Unlocked-Smartphone-/";


            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(dataGridView1);  
            row.Cells[0].Value = "Agent1";
            row.Cells[1].Value = "560 $";
            row.Cells[2].Value = new DateTime(2016, 5, 22, 13, 22, 10);
            dataGridView1.Rows.Add(row);

            DataGridViewRow row2 = new DataGridViewRow();
            row2.CreateCells(dataGridView1);
            row2.Cells[0].Value = "Agent3";
            row2.Cells[1].Value = "540 $";
            row2.Cells[2].Value = new DateTime(2016, 5, 22, 10, 15, 14);
            dataGridView1.Rows.Add(row2);

            DataGridViewRow row3 = new DataGridViewRow();
            row3.CreateCells(dataGridView1);
            row3.Cells[0].Value = "Agent1";
            row3.Cells[1].Value = "520 $";
            row3.Cells[2].Value = new DateTime(2016, 5, 21, 22, 10, 52);
            dataGridView1.Rows.Add(row3);

            DataGridViewRow row4 = new DataGridViewRow();
            row4.CreateCells(dataGridView2);
            row4.Cells[0].Value = "Agent3";
            row4.Cells[1].Value = "650 $";
            row4.Cells[2].Value = new DateTime(2016, 5, 24, 22, 10, 12);
            dataGridView2.Rows.Add(row4);

            DataGridViewRow row5 = new DataGridViewRow();
            row5.CreateCells(dataGridView2);
            row5.Cells[0].Value = "Agent1";
            row5.Cells[1].Value = "630 $";
            row5.Cells[2].Value = new DateTime(2016, 5, 24, 14, 10, 02);
            dataGridView2.Rows.Add(row5);

            timer2.Tick += timer2_Tick;
            timer2.Interval = 10000;
            timer2.Enabled = true;
            timer2.Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Thread.Sleep(1500);
            listBox1.Items.Add("Apple iPhone 6S 64GB (2 Auction found)");
            listBox1.Items.Add("Apple iPhone 5S 32GB (4 Auction found)");
            listBox1.Items.Add("Apple iPhone 6 16 (1 Auction found)");            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Thread.Sleep(450);
            MessageBox.Show("Authentication succeeded!");
        }
    }
}
