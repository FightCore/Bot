using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using FightCore.Bot.Configuration;
using Microsoft.Extensions.Options;

namespace FightCore.Bot.Services
{
    public class CommandHandlingService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private IServiceProvider _provider;
        private readonly char _prefix;

        public CommandHandlingService(
            IServiceProvider provider,
            DiscordSocketClient discord,
            CommandService commands,
            IOptions<CommandSettings> commandSettings)
        {
            _discord = discord;
            _commands = commands;
            _provider = provider;
            _prefix = commandSettings.Value.Prefix;

            _discord.MessageReceived += MessageReceived;
        }

        public async Task InitializeAsync(IServiceProvider provider)
        {
            _provider = provider;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), provider);
            // Add additional initialization code here...
        }

        private async Task MessageReceived(SocketMessage rawMessage)
        {
            // Ignore system messages and messages from bots
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            var argPos = 0;
            if (!message.HasMentionPrefix(_discord.CurrentUser, ref argPos) && !message.HasCharPrefix(_prefix, ref argPos)) return;

            var context = new SocketCommandContext(_discord, message);

            var result = await _commands.ExecuteAsync(context, argPos, _provider);

            if (result.Error.HasValue &&
                result.Error.Value != CommandError.UnknownCommand)
                await context.Channel.SendMessageAsync(result.ToString());
        }
    }
}
