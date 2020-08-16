using System;
using System.Threading.Tasks;
using Discord.Commands;
using FightCore.Bot.Configuration;
using FightCore.Bot.EmbedCreators.Tournaments;
using Microsoft.Extensions.Options;
using Smashgg.Net.Logic.Client;

namespace FightCore.Bot.Modules
{
    [Group("tournament")]
    public class TournamentModule : ModuleBase<SocketCommandContext>
    {
        private readonly bool _isEnabled;

        public TournamentModule(IOptions<ModuleSettings> moduleSettings)
        {
            _isEnabled = moduleSettings.Value.Tournaments;
        }

        [Command]
        public async Task Info([Remainder] string slug)
        {
            if (!_isEnabled)
            {
                return;
            }

            var smashggNetClient = new SmashggNetClient(Environment.GetEnvironmentVariable("token"));
            var tournament = await smashggNetClient.TournamentEndpoint.GetTournamentWithNestedEntities(slug);

            var embed = new TournamentEmbedCreator(null).Create(tournament);
            await ReplyAsync("", embed: embed);
        }
    }
}
