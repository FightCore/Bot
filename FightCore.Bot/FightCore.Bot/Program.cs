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
using FightCore.SlippiStatsOnline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ConfigurationBuilder = Microsoft.Extensions.Configuration.ConfigurationBuilder;

namespace FightCore.Bot
{
    public class Program
    {
        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private IConfiguration _config;

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _config = BuildConfig();

            var services = ConfigureServices();
            services.GetRequiredService<LogService>();
            await services.GetRequiredService<CommandHandlingService>().InitializeAsync(services);

            //_client.GuildAvailable += guild =>
            //{
            //    Console.WriteLine($"{guild.Name}: {guild.MemberCount}");
            //    return Task.CompletedTask;
            //};

            await _client.LoginAsync(TokenType.Bot, _config["token"]);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private IServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                // Base
                .AddSingleton(_client)
                .AddSingleton<CommandService>()
                // Logging
                .AddLogging()
                .AddSingleton<LogService>()
                .AddSingleton<FrameDataService>()
                // Extra
                .AddSingleton(_config)
                .Configure<EmbedSettings>(_config.GetSection("EmbedSettings"))
                .Configure<ModuleSettings>(_config.GetSection("Modules"))
                .Configure<CommandSettings>(_config.GetSection("CommandSettings"))
                .Configure<LoggingSettings>(_config.GetSection("Logging"))
                .AddScoped<ICharacterService, CharacterService>()
                .AddScoped<ISlippiPlayerService, SlippiPlayerService>()
                .AddScoped<CharacterInfoEmbedCreator>()
                .AddScoped<NotFoundEmbedCreator>()
                .AddScoped<PlayerEmbedCreator>()
                // Add additional services here...
                .AddSingleton<CommandHandlingService>()
                .BuildServiceProvider();
        }

        private IConfiguration BuildConfig()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .SetBasePath(Directory.GetCurrentDirectory())
                .Build();
        }
    }
}
