using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using FightCore.Bot.Configuration;
using FightCore.Bot.Services;
using Microsoft.Extensions.Options;

namespace FightCore.Bot.Modules
{
    [Group("admin")]
    [Alias("a")]
    public class AdminModule : ModuleBase<SocketCommandContext>
    {
        private readonly FrameDataService _frameDataService;
        private readonly UsersConfiguration _usersConfiguration;
        private readonly DiscordSocketClient _client;
        public AdminModule(
            FrameDataService frameDataService,
            DiscordSocketClient client,
            IOptions<UsersConfiguration> usersConfiguration)
        {
            _frameDataService = frameDataService;
            _usersConfiguration = usersConfiguration.Value;
            _client = client;
        }

        [Command("refresh")]
        [Alias("reload")]
        public async Task Refresh()
        {
            if (_usersConfiguration.Admins.Contains(Context.User.Id))
            {
                _frameDataService.ReloadData();
                await ReplyAsync(":white_check_mark: Refreshed and all services are back online.");
            }
        }

        [Command("servers")]
        public async Task Servers()
        {
            if (_usersConfiguration.Admins.Contains(Context.User.Id))
            {
                await ReplyAsync($"Active on {_client.Guilds.Count} servers");
            }
        }

        [Command("game")]
        public async Task SetGame([Remainder] string gameText)
        {
            if (_usersConfiguration.Admins.Contains(Context.User.Id))
            {
                if (gameText == "clear")
                {
                    await _client.SetGameAsync(null, null, ActivityType.Playing);
                    await ReplyAsync(":white_check_mark: Cleared the custom game.");
                }
                else
                {
                    await _client.SetGameAsync(gameText);
                    await ReplyAsync(":white_check_mark: Set the custom game.");
                }
            }
        }
    }

}
