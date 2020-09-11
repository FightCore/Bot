using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using FightCore.Api.Services;
using FightCore.Bot.Configuration;
using FightCore.Bot.EmbedCreators;
using FightCore.Bot.EmbedCreators.Characters;
using FightCore.Bot.Models.FrameData;
using FightCore.Bot.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Character = FightCore.Api.Models.Character;

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

        [Command]
        public async Task FrameDataTest([Remainder] string query)
        {
            var sections = query.Split(' ');
            switch (sections[0])
            {
                case "m":
                case "move":
                    sections = sections[1..];
                    break;
                case "moves":
                {
                    var list = sections[1..].ToList();
                    list.Add("moves");
                    sections = list.ToArray();
                    break;
                }
            }

            using (Context.Channel.EnterTypingState())
            {
                WrapperCharacter characterEntity = null;
                string character = null;
                string move = null;
                string tempCharacter = null;
                var iterator = 1;
                foreach (var section in sections)
                {
                    if (tempCharacter == null)
                    {
                        character = query;
                        tempCharacter = section;
                    }
                    else
                    {
                        tempCharacter += " " + section;
                    }

                    characterEntity = _frameDataService.GetCharacter(tempCharacter);
                    if (characterEntity == null)
                    {
                        iterator++;
                        continue;
                    }

                    var split = iterator++;
                    character = tempCharacter;
                    move = string.Join(' ', sections[split..]);
                }

                if (characterEntity == null && !string.IsNullOrWhiteSpace(character))
                {
                    characterEntity = _frameDataService.GetCharacter(character);
                }

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

                if (string.IsNullOrWhiteSpace(move))
                {
                    await Info(characterEntity, fightCoreCharacter);
                    return;
                }

                // ReSharper disable once PossibleNullReferenceException
                switch (move.Split(' ')[0])
                {
                    case "moves":
                        await ListMoves(characterEntity, fightCoreCharacter);
                        return;
                }

                await GetMoveData(characterEntity, fightCoreCharacter, move);
            }
        }

        private async Task ListMoves(WrapperCharacter characterEntity, Character fightCoreCharacter)
        {
            var moves = _frameDataService.GetMoves(characterEntity.NormalizedName);

            var embed = _characterInfoEmbedCreator.CreateMoveListEmbed(characterEntity, moves, fightCoreCharacter);
            await ReplyAsync(string.Empty, embed: embed);

            if (_loggingSettings.Characters)
            {
                _logger.LogMessage(LogLevel.Information, "[Character moves]: {0}", characterEntity.Name);
            }
        }

        private async Task Info(WrapperCharacter characterEntity, Character fightCoreCharacter)
        {
            var misc = _frameDataService.GetMiscForCharacter(characterEntity.NormalizedName);
            var embed = _characterInfoEmbedCreator.CreateInfoEmbed(characterEntity, fightCoreCharacter, misc);

            await ReplyAsync(string.Empty, embed: embed);

            if (_loggingSettings.Characters)
            {
                _logger.LogMessage(LogLevel.Information, "[Character]: {0}", characterEntity.Name);
            }
        }

        private async Task GetMoveData(WrapperCharacter characterEntity, Character fightCoreCharacter, string move)
        {
            var result = _frameDataService.GetMove(characterEntity.NormalizedName, move, characterEntity);
            if (result == null)
            {
                if (_loggingSettings.Moves)
                {
                    _logger.LogMessage(LogLevel.Warning, "NOT FOUND [Character]: {0}, [Move]: {1}", characterEntity.NormalizedName, move);
                }

                var notFoundEmbed = _notFoundEmbedCreator.Create(new Dictionary<string, string>()
                    {{"Character", characterEntity.NormalizedName}, {"Move", move}});
                await ReplyAsync("", embed: notFoundEmbed);
                return;
            }

            result.Hitboxes.Sort((hitboxOne, hitboxTwo) => string.Compare(hitboxOne.Name, hitboxTwo.Name, StringComparison.Ordinal));
            var embed = _characterInfoEmbedCreator.CreateMoveEmbed(characterEntity, result, fightCoreCharacter);
            if (_loggingSettings.Moves)
            {
                _logger.LogMessage(LogLevel.Information, "[Character]: {0}, [Move]: {1}", characterEntity.Name, move);
            }

            await ReplyAsync(string.Empty, embed: embed);
        }
    }
}
