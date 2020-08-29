using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FightCore.SlippiStatsOnline.Models;
using RestSharp;

namespace FightCore.SlippiStatsOnline
{
    public interface ISlippiPlayerService
    {
        Task<PlayerResult> GetForUser(string tag, string opponent = null);
    }

    public class SlippiPlayerService : ISlippiPlayerService
    {
        private readonly IRestClient _client;

        public SlippiPlayerService()
        {
            _client = new RestClient("https://slippistats.online");
        }

        public async Task<PlayerResult> GetForUser(string tag, string opponent = null)
        {
            try
            {
                tag = tag.Replace('#', '-');
                var url = $"api/stats/{tag}";
                if (opponent != null)
                {
                    url += $"?opponentCode={opponent.Replace("#", "-")}";
                }

                var request = new RestRequest(url, Method.GET);
                return await _client.GetAsync<PlayerResult>(request);
            }
            catch
            {
                return null;
            }
        }
    }
}
