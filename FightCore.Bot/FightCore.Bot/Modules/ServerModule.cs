using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using FightCore.Services;

namespace FightCore.Bot.Modules
{
    [Group("server")]
    [Alias("s")]
    public class ServerModule : ModuleBase<SocketCommandContext>
    {
        private readonly IServerSettingsService _serverSettingsService;

        public ServerModule(IServerSettingsService serverSettingsService)
        {
            _serverSettingsService = serverSettingsService;
        }

        [Command("prefix")]
        public async Task SetPrefix(string prefix)
        {
            return;
            if (!(Context.User is SocketGuildUser guidUser))
            {
                await ReplyAsync("Not in a server");
                return;
            }

            if (!guidUser.GuildPermissions.Administrator)
            {
                await ReplyAsync("Not an admin");
            }

            await _serverSettingsService.SetPrefix(Context.Guild.Id, prefix[0]);
            await ReplyAsync($"Your prefix has been set to {prefix[0]}");
        }
    }
}
