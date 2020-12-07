using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using FightCore.Bot.Configuration;
using FightCore.Bot.Modules;
using FightCore.Models;
using FightCore.Services;
using Microsoft.Extensions.Options;

namespace FightCore.Bot.Services
{
    public class CommandHandlingService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private IServiceProvider _provider;
        private readonly CommandSettings _settings;
        private readonly ModuleSettings _moduleSettings;
        private readonly FailedMessageService _failedMessageService;
        private readonly IServerSettingsService _serverSettingsService;

        private readonly Dictionary<string, Type> _moduleDictionary = new Dictionary<string, Type>()
        {
            {nameof(ModuleSettings.Moves), typeof(CharacterModule)},
            {nameof(ModuleSettings.SlippiStats), typeof(SlippiModule)},
            {nameof(ModuleSettings.Tournaments), typeof(TournamentModule)}
        };

        public CommandHandlingService(
            IServiceProvider provider,
            DiscordSocketClient discord,
            CommandService commands,
            IOptions<ModuleSettings> moduleOptions,
            IOptions<CommandSettings> commandSettings,
            IServerSettingsService serverSettingsService,
            FailedMessageService failedMessageService)
        {
            _discord = discord;
            _commands = commands;
            _provider = provider;
            _settings = commandSettings.Value;
            _moduleSettings = moduleOptions.Value;
            _discord.MessageReceived += MessageReceived;
            _discord.MessageUpdated += DiscordOnMessageUpdated;
            _failedMessageService = failedMessageService;
            _serverSettingsService = serverSettingsService;
        }

        private async Task DiscordOnMessageUpdated(Cacheable<IMessage, ulong> originalMessage, SocketMessage socketMessage, ISocketMessageChannel channel)
        {
            var botMessage = _failedMessageService.GetMessageIdForEditMessage(originalMessage.Id);
            if (botMessage == null)
            {
                return;
            }

            if (!(socketMessage is SocketUserMessage message))
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

                await botMessage.ModifyAsync(editMessage => editMessage.Content = resultMessage);
            }
        }

        public async Task InitializeAsync(IServiceProvider provider)
        {
            _provider = provider;
            await _commands.AddModuleAsync(typeof(HelpModule), _provider);
            await _commands.AddModuleAsync(typeof(AdminModule), _provider);
            await _commands.AddModuleAsync(typeof(ServerModule), _provider);
            if (_moduleSettings.SlippiStats) await AddModule(nameof(_moduleSettings.SlippiStats));
            if (_moduleSettings.Moves) await AddModule(nameof(_moduleSettings.Moves));
            if (_moduleSettings.Tournaments) await AddModule(nameof(_moduleSettings.Tournaments));
        }

        private async Task AddModule(string module)
        {
            await _commands.AddModuleAsync(_moduleDictionary[module], _provider);
        }

        private async Task MessageReceived(SocketMessage rawMessage)
        {
            // Ignore system messages and messages from bots
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            var context = new SocketCommandContext(_discord, message);
            ServerSettings settings = null;
            if (context.Guild == null)
            {
                settings = _serverSettingsService.CreateDefaultSettings();
            }
            else
            {
                settings = await _serverSettingsService.GetForServerId(context.Guild.Id);
            }

            // Check if there is a channel id specified.
            // If there is, check if its the correct channel, else just return and ignore.
            if (_settings.ChannelId.HasValue && message.Channel.Id != _settings.ChannelId.Value)
            {
                return;
            }

            var argPos = 0;
            if (!message.HasMentionPrefix(_discord.CurrentUser, ref argPos) &&
                !message.HasCharPrefix(settings.Prefix[0], ref argPos))
            {
                return;
            }

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
