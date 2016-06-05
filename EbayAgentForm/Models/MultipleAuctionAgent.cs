using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class MultipleAuctionAgent : IAgent
    {
        public Auction Auction { get; set; }

        public string Name { get; set; }

        private HttpClient Client { get; set; }

        public async Task Initialize(HttpClient client)
        {
            this.Client = client;

        }

        public async Task ParticipateAuction()
        {
            throw new NotImplementedException();
        }
    }
}
