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
        private readonly CommandSettings _settings;

        public CommandHandlingService(
            IServiceProvider provider,
            DiscordSocketClient discord,
            CommandService commands,
            IOptions<CommandSettings> commandSettings)
        {
            _discord = discord;
            _commands = commands;
            _provider = provider;
            _settings = commandSettings.Value;

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

            // Check if there is a channel id specified.
            // If there is, check if its the correct channel, else just return and ignore.
            if (_settings.ChannelId.HasValue && message.Channel.Id != _settings.ChannelId.Value)
            {
                return;
            }

            var argPos = 0;
            if (!message.HasMentionPrefix(_discord.CurrentUser, ref argPos) &&
                !message.HasCharPrefix(_settings.Prefix, ref argPos))
            {
                return;
            }

            var context = new SocketCommandContext(_discord, message);

            var result = await _commands.ExecuteAsync(context, argPos, _provider);

            if (result.Error.HasValue)
            {
                var resultMessage = result.Error switch
                {
                    CommandError.UnknownCommand => null,
                    CommandError.BadArgCount => "**Bad number of arguments**\nDouble check if your command is correct.",
                    _ => "An unknown error occurred while trying to process the command."
                };

                if (string.IsNullOrWhiteSpace(resultMessage))
                {
                    return;
                }


                await context.Channel.SendMessageAsync(resultMessage);
            }
        }
    }
}
