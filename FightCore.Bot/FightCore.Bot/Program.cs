using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using FightCore.Api.Services;
using FightCore.Bot.Configuration;
using FightCore.Bot.EmbedCreators;
using FightCore.Bot.EmbedCreators.Characters;
using FightCore.Bot.EmbedCreators.Slippi;
using FightCore.Bot.Services;
using FightCore.Data;
using FightCore.Services;
using FightCore.SlippiStatsOnline;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ConfigurationBuilder = Microsoft.Extensions.Configuration.ConfigurationBuilder;

namespace FightCore.Bot
{
    public class Program
    {
        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        private ulong _ownerId = 0;
        private DiscordSocketClient _client;
        private IConfiguration _config;

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _config = BuildConfig();

            var services = ConfigureServices();
            var admins = services.GetService<IOptions<UsersConfiguration>>().Value.Admins;
            if (admins.Count > 0)
            {
                _ownerId = admins[0];
            }

            services.GetRequiredService<LogService>();
            await services.GetRequiredService<CommandHandlingService>().InitializeAsync(services);

            //_client.GuildAvailable += guild =>
            //{
            //    Console.WriteLine($"{guild.Name}: {guild.MemberCount}");
            //    return Task.CompletedTask;
            //};

            _client.JoinedGuild += ClientOnJoinedGuild;

            await _client.LoginAsync(TokenType.Bot, _config["token"]);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task ClientOnJoinedGuild(SocketGuild arg)
        {
            if (_ownerId <= 0)
            {
                return;
            }

            var dmChannel = await _client.GetDMChannelAsync(_ownerId);
            await dmChannel.SendMessageAsync($"Joined {arg.Name}, Members: {arg.MemberCount}");
        }

        private IServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                // Base
                .AddSingleton(_client)
                .AddSingleton<FailedMessageService>()
                .AddSingleton<CommandService>()
                // Logging
                .AddLogging()
                .AddSingleton<LogService>()
                .AddSingleton<FrameDataService>()
                // Storage
                .AddDbContext<ServerContext>(options =>
                    options.UseSqlServer(_config.GetConnectionString("DefaultConnection")))
                // Extra
                .AddSingleton(_config)
                .Configure<UsersConfiguration>(_config.GetSection("Users"))
                .Configure<EmbedSettings>(_config.GetSection("EmbedSettings"))
                .Configure<ModuleSettings>(_config.GetSection("Modules"))
                .Configure<CommandSettings>(_config.GetSection("CommandSettings"))
                .Configure<LoggingSettings>(_config.GetSection("Logging"))
                .AddScoped<ICharacterService, CharacterService>()
                .AddScoped<ISlippiPlayerService, SlippiPlayerService>()
                .AddScoped<CharacterInfoEmbedCreator>()
                .AddScoped<NotFoundEmbedCreator>()
                .AddScoped<PlayerEmbedCreator>()
                .AddScoped<IServerSettingsService, ServerSettingsService>()
                // Add additional services here...
                .AddSingleton<CommandHandlingService>()
                .BuildServiceProvider();
        }

        private IConfiguration BuildConfig()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.local.json", optional: true)
                .SetBasePath(Directory.GetCurrentDirectory())
                .Build();
        }
    }
}
