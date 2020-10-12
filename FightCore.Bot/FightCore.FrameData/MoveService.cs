using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FightCore.FrameData.Models;
using RestSharp;

namespace FightCore.FrameData
{
    public interface IMoveService
    {
        Task<List<Character>> GetCharacters();
    }
    public class MoveService : IMoveService
    {
        private readonly IRestClient _client;

        public MoveService()
        {
            _client = new RestClient("https://localhost:5001");
        }

        public async Task<List<Character>> GetCharacters()
        {
            try
            {
                const string url = "/framedata/moves";

                var request = new RestRequest(url, Method.GET);
                return await _client.GetAsync<List<Character>>(request);
            }
            catch
            {
                return null;
            }
        }
    }
}
