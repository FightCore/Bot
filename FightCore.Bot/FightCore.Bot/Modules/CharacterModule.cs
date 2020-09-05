using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using FightCore.Api.Services;
using FightCore.Bot.Configuration;
using FightCore.Bot.EmbedCreators;
using FightCore.Bot.EmbedCreators.Characters;
using FightCore.Bot.Helpers;
using FightCore.Bot.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FightCore.Bot.Modules
{
    [Group("character")]
    [Alias("c")]
    public class CharacterModule : ModuleBase<SocketCommandContext>
    {
        private readonly ICharacterService _characterService;
        private readonly FrameDataService _frameDataService;
        private readonly CharacterInfoEmbedCreator _characterInfoEmbedCreator;
        private readonly NotFoundEmbedCreator _notFoundEmbedCreator;
        private readonly bool _isEnabled;
        private readonly LoggingSettings _loggingSettings;
        private readonly LogService _logger;

        public CharacterModule(
            ICharacterService characterService,
            FrameDataService frameDataService,
            CharacterInfoEmbedCreator characterInfoEmbedCreator,
            NotFoundEmbedCreator notFoundEmbedCreator,
            IOptions<LoggingSettings> loggingSettings,
            LogService logger,
            IOptions<ModuleSettings> moduleSettings)
        {
            _characterService = characterService;
            _frameDataService = frameDataService;
            _characterInfoEmbedCreator = characterInfoEmbedCreator;
            _notFoundEmbedCreator = notFoundEmbedCreator;
            _isEnabled = moduleSettings.Value.Moves;
            _loggingSettings = loggingSettings.Value;
            _logger = logger;
        }


        [Command("help")]
        public async Task Help()
        {
            if (!_isEnabled)
            {
                return;
            }

            var embed = _characterInfoEmbedCreator.CreateHelpEmbed();
            await ReplyAsync(string.Empty, embed: embed);
        }

        [Command("move")]
        [Alias("m")]
        public async Task FrameDataTest(string character, [Remainder] string move)
        {
            if (!_isEnabled)
            {
                return;
            }

            using (Context.Channel.EnterTypingState())
            {
                var characterEntity = _frameDataService.GetCharacter(character);

                if (characterEntity == null)
                {
                    var notFoundEmbed = _notFoundEmbedCreator.Create(new Dictionary<string, string>()
                        {{"Character", character}});
                    await ReplyAsync("", embed: notFoundEmbed);

                    if (_loggingSettings.Moves)
                    {
                        _logger.LogMessage(LogLevel.Information, "NOT FOUND [Character]: {0}", character);
                    }
                    return;
                }

                var fightCoreCharacter = await _characterService.GetByIdAsync(characterEntity.FightCoreId);
                var attack = _frameDataService.GetMove(character, move);

                if (attack == null)
                {
                    if (_loggingSettings.Moves)
                    {
                        _logger.LogMessage(LogLevel.Warning, "NOT FOUND [Character]: {0}, [Move]: {1}", character, move);
                    }

                    var notFoundEmbed = _notFoundEmbedCreator.Create(new Dictionary<string, string>()
                        {{"Character", character}, {"Move", move}});
                    await ReplyAsync("", embed: notFoundEmbed);
                    return;
                }

                var embed = _characterInfoEmbedCreator.CreateMoveEmbed(characterEntity, attack, fightCoreCharacter);
                if (_loggingSettings.Moves)
                {
                    _logger.LogMessage(LogLevel.Information, "[Character]: {0}, [Move]: {1}", characterEntity.Name, attack.Name);
                }

                await ReplyAsync(string.Empty, embed: embed);
            }
        }

        [Command("moves")]
        public async Task ListMoves([Remainder] string character)
        {
            if (!_isEnabled)
            {
                return;
            }

            using (Context.Channel.EnterTypingState())
            {
                var characterEntity = _frameDataService.GetCharacter(character);

                if (characterEntity == null)
                {
                    var notFoundEmbed = _notFoundEmbedCreator.Create(new Dictionary<string, string>()
                        {{"Character", character}});
                    await ReplyAsync("", embed: notFoundEmbed);

                    if (_loggingSettings.Moves)
                    {
                        _logger.LogMessage(LogLevel.Warning, "NOT FOUND [Character]: {0}", character);
                    }

                    return;
                }

                var moves = _frameDataService.GetMoves(characterEntity.NormalizedName);

                var fightCoreCharacter = await _characterService.GetByIdAsync(characterEntity.FightCoreId);

                var embed = _characterInfoEmbedCreator.CreateMoveListEmbed(characterEntity, moves, fightCoreCharacter);
                await ReplyAsync(string.Empty, embed: embed);

                if (_loggingSettings.Characters)
                {
                    _logger.LogMessage(LogLevel.Information, "[Character moves]: {0}", characterEntity.Name);
                }
            }
        }

        [Command]
        public async Task Info(string character)
        {
            if (!_isEnabled)
            {
                return;
            }

            if (SearchHelper.Normalize(character) == "help")
            {
                await Help();
                return;
            }

            using (Context.Channel.EnterTypingState())
            {
                var characterEntity = _frameDataService.GetCharacter(character);
                if (characterEntity == null)
                {
                    var notFoundEmbed = _notFoundEmbedCreator.Create(new Dictionary<string, string>()
                        {{"Character", character}});
                    await ReplyAsync("", embed: notFoundEmbed);

                    if (_loggingSettings.Moves)
                    {
                        _logger.LogMessage(LogLevel.Warning, "NOT FOUND [Character]: {0}", character);
                    }

                    return;
                }

                var fightCoreCharacter = await _characterService.GetByIdAsync(characterEntity.FightCoreId);
                var misc = _frameDataService.GetMiscForCharacter(characterEntity.NormalizedName);
                var embed = _characterInfoEmbedCreator.CreateInfoEmbed(characterEntity, fightCoreCharacter, misc);

                await ReplyAsync(string.Empty, embed: embed);

                if (_loggingSettings.Characters)
                {
                    _logger.LogMessage(LogLevel.Information, "[Character]: {0}", characterEntity.Name);
                }
            }
        }
    }
}
