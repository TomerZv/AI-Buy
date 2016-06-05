using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public interface IAgent
    {
        Auction Auction { get; set; }
        string Name { get; set; }

        Task Initialize(HttpClient client);

        Task ParticipateAuction();
    }
}
