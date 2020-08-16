using System.Threading.Tasks;
using FightCore.Api.Models;
using RestSharp;

namespace FightCore.Api.Services
{
    public interface ICharacterService
    {
        Task<Character> GetByIdAsync(long id);
    }

    public class CharacterService : ICharacterService
    {

        private readonly IRestClient _client;

        public CharacterService()
        {
            _client = new RestClient("https://api.fightcore.gg");
        }

        public Task<Character> GetByIdAsync(long id)
        {
            var request = new RestRequest($"characters/{id}", Method.GET);
            return _client.GetAsync<Character>(request);
        }
    }
}
