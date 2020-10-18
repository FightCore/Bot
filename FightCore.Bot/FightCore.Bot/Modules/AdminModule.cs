using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
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
        public AdminModule(
            FrameDataService frameDataService,
            IOptions<UsersConfiguration> usersConfiguration)
        {
            _frameDataService = frameDataService;
            _usersConfiguration = usersConfiguration.Value;
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
    }

}
