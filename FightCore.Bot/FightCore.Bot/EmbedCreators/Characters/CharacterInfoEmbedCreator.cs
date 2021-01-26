using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;
using FightCore.Bot.Configuration;
using FightCore.Bot.EmbedCreators.Base;
using FightCore.FrameData.Models;
using FightCore.Logic.Aliasses.FrameData;
using FightCore.Logic.Formatting;
using FightCore.Logic.Search;
using Microsoft.Extensions.Options;
using Character = FightCore.Api.Models.Character;
using Move = FightCore.FrameData.Models.Move;

namespace FightCore.Bot.EmbedCreators.Characters
{
    public class CharacterInfoEmbedCreator : BaseEmbedCreator
    {

        private readonly char _prefix;
        private readonly CharacterFormatting _formatter;

        public CharacterInfoEmbedCreator(IOptions<EmbedSettings> embedSettings,
            IOptions<CommandSettings> commandSetting) : base(embedSettings)
        {
            _prefix = commandSetting.Value.Prefix;
            _formatter = new CharacterFormatting("**{0}:**");
        }

        public Embed CreateInfoEmbed(WrapperCharacter wrapperCharacter, Character character, CharacterStatistics statistics, CharacterInfo info)
        {
            var embedBuilder = new EmbedBuilder {Title = wrapperCharacter.Name};

            if (EmbedSettings.FightCoreInfo && character != null)
            {
                embedBuilder.AddField("General information", ShortenString(character.GeneralInformation, 250));

                if (character.NotablePlayers.Any())
                {
                    embedBuilder.AddField("Notable players",
                        string.Join(", ", character.NotablePlayers.Take(6).Select(player => player.Name))
                    );
                }
            }

            if (character?.StockIcon != null)
                embedBuilder.WithThumbnailUrl(character.StockIcon.Url);

            if (character?.CharacterImage != null)
                embedBuilder.WithImageUrl(character.CharacterImage.Url);

            var movementStringBuilder = _formatter.CreateMovementData(statistics);
            embedBuilder.AddField("Ground movement", movementStringBuilder.ToString());

            var frameDataStringBuilder = _formatter.CreateCharacterFrameData(statistics);
            embedBuilder.AddField("Frame data", frameDataStringBuilder.ToString());

            var miscStringBuilder = _formatter.CreateCharacterMiscData(info, wrapperCharacter);

            embedBuilder.AddField("Misc data", miscStringBuilder.ToString());

            embedBuilder.WithUrl($"https://www.fightcore.gg/character/{wrapperCharacter.FightCoreId}");
            embedBuilder = AddFooter(embedBuilder);
            return embedBuilder.Build();
        }

        public Embed CreateMoveEmbed(WrapperCharacter character, Move move, Character fightCoreCharacter)
        {
            var embedBuilder = CreateDefaultFrameDataEmbed(character, move, fightCoreCharacter);
            embedBuilder.Description = $"Technical name: **{move.NormalizedName}**";

            var frameDataBuilder = _formatter.CreateMoveData(move);

            if (!string.IsNullOrWhiteSpace(frameDataBuilder.ToString()))
            {
                embedBuilder.AddField("Frame Data", frameDataBuilder.ToString(), true);
            }

            var hitboxStringBuilder = _formatter.CreateHitboxData(move);
            if (!string.IsNullOrWhiteSpace(hitboxStringBuilder.ToString()))
            {
                AddString("Source", "https://www.ikneedata.com", hitboxStringBuilder);
                hitboxStringBuilder.AppendLine("id0=Red, id1=Green, id2=Purple, id3=Orange");
                // Temporary take out the credit to prepare for update.
                //hitboxStringBuilder.AppendLine("Credits to MWStage for the colored GIFs");
                embedBuilder.AddField("Hitbox summary", hitboxStringBuilder.ToString());
            }

            return embedBuilder.Build();
        }



        public Embed CreateMoveListEmbed(WrapperCharacter character, List<Move> moves, Character fightCoreCharacter)
        {
            var embedBuilder = new EmbedBuilder();
            embedBuilder.WithUrl($"http://meleeframedata.com/{character.NormalizedName}");
            if (fightCoreCharacter != null)
            {
                embedBuilder.WithThumbnailUrl(fightCoreCharacter.StockIcon.Url.Replace(" ", "+"));
            }
            embedBuilder.Title = $"{character.Name} - Moves";
            var groupedMoves = moves.GroupBy(move => move.Type)
                .OrderBy(move => move.Key);

            foreach (var type in groupedMoves)
            {
                embedBuilder.AddField(type.Key.ToString(),
                    ShortenField(string.Join(", ", type.Select(move => move.Name))), type.Key != MoveType.Unknown);
            }
            embedBuilder.AddField("Help", "To check out a move use:\n`" + _prefix + "c {CHARACTER NAME} {MOVE NAME}`\n" +
                                          "For example: `" + _prefix + "c Fox u-smash`");

            embedBuilder = AddFooter(embedBuilder);
            return embedBuilder.Build();
        }

        public Embed CreateHelpEmbed()
        {
            var embedBuilder = new EmbedBuilder();
            embedBuilder.WithTitle("Help");
            embedBuilder.AddField("Character statistics",
                "`" + _prefix + "c {{NAME}}`\n" +
                "Use this command to get information about a character.\n" +
                "Example: `" + _prefix + "c Kirby`");
            embedBuilder.AddField("Move list",
                "`" + _prefix + "c {{NAME}} moves`\n" +
                "Use this command to get a list of moves that are available for that character. " +
                "Note that the special move names can also be shortened to their input (Fox Blaster = neutral b).\n" +
                "Example: `" + _prefix + "c g&w moves`");
            embedBuilder.AddField("Move frame data",
                "`" + _prefix + "c {{CHARACTER}} {{MOVE}}`\n" +
                "Gets the frame and hitbox data from a specific character and move.\n" +
                "Example: `" + _prefix + "c Captain Falcon u-tilt`");
            embedBuilder.AddField("Discord",
                "For further help and reporting bugs, visit our discord at http://discord.fightcore.gg");
            embedBuilder = AddFooter(embedBuilder);

            return embedBuilder.Build();
        }

        private EmbedBuilder CreateDefaultFrameDataEmbed(WrapperCharacter character, Move move,
            Character fightCoreCharacter)
        {
            var characterName = SearchHelper.Normalize(character.NormalizedName);
            var moveName = SearchHelper.Normalize(move.NormalizedName);
            var embedBuilder = new EmbedBuilder()
                .WithUrl($"http://meleeframedata.com/{characterName}")
                .WithImageUrl($"https://i.fightcore.gg/melee/moves/{characterName}/{moveName}.gif");

            if (fightCoreCharacter?.StockIcon != null)
            {
                embedBuilder.WithThumbnailUrl(fightCoreCharacter.StockIcon.Url.Replace(" ", "+"));
            }

            embedBuilder.Title = $"{character.Name} - {move.Name}";
            return AddFooter(embedBuilder);
        }


        private static void AddString(string key, string value, StringBuilder stringBuilder)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            stringBuilder.AppendLine($"**{key}:** {value}");
        }
    }
}
