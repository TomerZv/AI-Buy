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

        public int FailedCount { get; set; }

        private HttpClient Client { get; set; }

        static Random random = new Random();

        public async Task Initialize(HttpClient client)
        {
            this.Client = client;
            this.FailedCount = 0;

            // TODO : Make this real.
            this.Name = "Agent" + random.Next(1, 10000).ToString();
            var auctions = await GetAuctions(client);

            List<int> indexes = GenerateRandom(auctions.Count / 2, auctions.Count);

            List<Auction> auctionToParticipate = new List<Auction>();

            foreach (int index in indexes)
            {
                auctionToParticipate.Add(auctions.ElementAt(index));
            }
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
                    number = random.Next();
                } while (randomNumbers.Contains(number));

                randomNumbers.Add(number);
			}

            return randomNumbers;
     
        }

        public async Task ParticipateAuction()
        {
            throw new NotImplementedException();
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
