using System;
using System.Collections;
using System.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using eBay.Services.Common;
using eBay.Services.Finding;
using eBay.Services.WcfExtension;
using eBay.Service.Core;
using eBay.Service.Core.Sdk;
using eBay.Service.Core.Soap;
using eBay.Service.Util;
using eBay.Service.Call;
using Slf;
using eBay.Services;


namespace EbayAgentForm
{
    public partial class Form1 : Form
    {
        private static ApiContext apiContext = null;
        private FindingServicePortTypeClient client;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoggerService.SetLogger(new TraceLogger());

            // Get AppID and ServerAddress from Web.config
            string appID = System.Configuration.ConfigurationManager.AppSettings["AppID"];
            string findingServerAddress = System.Configuration.ConfigurationManager.AppSettings["FindingServerAddress"];

            ClientConfig config = new ClientConfig();
            // Initialize service end-point configration
            config.EndPointAddress = findingServerAddress;

            // set eBay developer account AppID
            config.ApplicationId = appID;

            // Create a service client
            client = FindingServiceClientFactory.getServiceClient(config);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Create request object
            FindItemsAdvancedRequest request = new FindItemsAdvancedRequest();
            
            ItemFilter auctionFilter = new ItemFilter();
            auctionFilter.name = ItemFilterType.ListingType;
            auctionFilter.value = new string[] { "Auction", "AuctionWithBIN" };
            request.itemFilter = new ItemFilter[] { auctionFilter };
            
            // Set request parameters
            request.keywords = textBox1.Text;

            if (request.keywords == null)
            {
                request.keywords = "ipod";
            }
            PaginationInput pi = new PaginationInput();
            pi.entriesPerPage = 10;
            pi.entriesPerPageSpecified = true;
            request.paginationInput = pi;

            // Call the service
            FindItemsAdvancedResponse response = client.findItemsAdvanced(request);

            if (response.searchResult != null && response.searchResult.item != null)
            {
                SearchItem[] items = response.searchResult.item;

                foreach (SearchItem item in items)
                {
                    textBox2.Text += item.title + Environment.NewLine;
                }
            }

        }
    }
}
