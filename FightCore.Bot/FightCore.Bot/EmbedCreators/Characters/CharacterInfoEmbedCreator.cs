using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;
using FightCore.Bot.Configuration;
using FightCore.Bot.EmbedCreators.Base;
using FightCore.FrameData.Models;
using FightCore.Logic.Aliasses.FrameData;
using FightCore.Logic.Search;
using Microsoft.Extensions.Options;
using Character = FightCore.Api.Models.Character;
using Move = FightCore.FrameData.Models.Move;

namespace FightCore.Bot.EmbedCreators.Characters
{
    public class CharacterInfoEmbedCreator : BaseEmbedCreator
    {

        private readonly char _prefix;

        public CharacterInfoEmbedCreator(IOptions<EmbedSettings> embedSettings,
            IOptions<CommandSettings> commandSetting) : base(embedSettings)
        {
            _prefix = commandSetting.Value.Prefix;
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

            var movementStringBuilder = new StringBuilder();
            AddIfPossible("Walk speed", statistics.WalkSpeed, movementStringBuilder);
            AddIfPossible("Initial Dash speed", statistics.InitialDash, movementStringBuilder);
            AddIfPossible("Initial dash frames", statistics.DashFrames, movementStringBuilder);
            AddIfPossible("Run speed", statistics.RunSpeed, movementStringBuilder);
            AddIfPossible("Wave dash length (rank)", statistics.WaveDashLengthRank, movementStringBuilder);
            AddIfPossible("Perfect wave dash length", statistics.WaveDashLength, movementStringBuilder);
            AddIfPossible("PLA Intangibility Frames", statistics.PLAIntangibilityFrames, movementStringBuilder);
            AddString("Source", "https://smashboards.com/threads/ultimate-ground-movement-analysis-turbo-edition.392367/", movementStringBuilder);
            embedBuilder.AddField("Ground movement", movementStringBuilder.ToString());


            var frameDataStringBuilder = new StringBuilder();
            frameDataStringBuilder.AppendLine($"**Weight:** {statistics.Weight}");
            frameDataStringBuilder.AppendLine($"**Gravity:** {statistics.Gravity}");
            frameDataStringBuilder.AppendLine($"**Can wall jump:** {statistics.CanWallJump}");
            frameDataStringBuilder.AppendLine($"**Jump squat:** {statistics.JumpSquat}");
            embedBuilder.AddField("Frame data", frameDataStringBuilder.ToString());

            var miscStringBuilder = new StringBuilder();
            AddString("Discord", info?.Discord, miscStringBuilder);
            AddString("MeleeFrameData", info?.MeleeFrameData, miscStringBuilder);
            AddString("SSB Wiki", info?.SsbWiki, miscStringBuilder);
            AddString("FightCore", $"https://www.fightcore.gg/character/{wrapperCharacter.FightCoreId}", miscStringBuilder);
            embedBuilder.AddField("Misc data", miscStringBuilder.ToString());

            embedBuilder.WithUrl($"https://www.fightcore.gg/character/{wrapperCharacter.FightCoreId}");
            embedBuilder = AddFooter(embedBuilder);
            return embedBuilder.Build();
        }

        public Embed CreateMoveEmbed(WrapperCharacter character, Move move, Character fightCoreCharacter)
        {
            var embedBuilder = CreateDefaultFrameDataEmbed(character, move, fightCoreCharacter);

            var frameDataBuilder = new StringBuilder();
            AddIfPossible("Total frames", move.TotalFrames, frameDataBuilder);
            if (move.Start.HasValue && move.End.HasValue)
            {
                AddString("Hit", $"{move.Start} - {move.End}", frameDataBuilder);
            }
            else if (move.Start.HasValue)
            {
                AddIfPossible("Hit start", move.Start, frameDataBuilder);
            }
            else if (move.End.HasValue)
            {
                AddIfPossible("Hit end", move.End, frameDataBuilder);
            }

            AddIfPossible("IASA", move.IASA, frameDataBuilder);
            AddIfPossible("Land lag", move.LandLag, frameDataBuilder);
            AddIfPossible("L-Canceled", move.LCanceledLandLag, frameDataBuilder);

            if (move.AutoCancelBefore.HasValue && move.AutoCancelAfter.HasValue && move.AutoCancelBefore > -1 && move.AutoCancelAfter > -1)
            {
                frameDataBuilder.AppendLine($"**Won't Auto Cancel:** {move.AutoCancelBefore}-{move.AutoCancelAfter}");
            }

            AddString("Notes", move.Notes, frameDataBuilder);
            AddString("Source", move.Source, frameDataBuilder);

            if (!string.IsNullOrWhiteSpace(frameDataBuilder.ToString()))
            {
                embedBuilder.AddField("Frame Data", frameDataBuilder.ToString(), true);
            }

            var hitboxStringBuilder = AddHitboxDataToEmbedBuilder(move);
            if (!string.IsNullOrWhiteSpace(hitboxStringBuilder.ToString()))
            {
                AddString("Source", "https://www.ikneedata.com", hitboxStringBuilder);
                hitboxStringBuilder.AppendLine("id0=Red, id1=Green, id2=Purple, id3=Orange");
                hitboxStringBuilder.AppendLine("Credits to MWStage for the colored GIFs");
                embedBuilder.AddField("Hitbox summary", hitboxStringBuilder.ToString());
            }

            return embedBuilder.Build();
        }

        private StringBuilder AddHitboxDataToEmbedBuilder(Move move)
        {
            var hitboxSummary = new StringBuilder();
            AddString("Name", string.Join('/', move.Hitboxes.Select(hitbox => hitbox.Name)), hitboxSummary);
            AddString("Damage", string.Join('/', move.Hitboxes.Select(hitbox => hitbox.Damage)), hitboxSummary);
            AddString("Effect", string.Join('/', move.Hitboxes.Select(hitbox => hitbox.Effect)), hitboxSummary);
            AddString("Angle", string.Join('/', move.Hitboxes.Select(hitbox => hitbox.Angle)), hitboxSummary);
            AddString("Base knockback", string.Join('/', move.Hitboxes.Select(hitbox => hitbox.BaseKnockback)), hitboxSummary);
            AddString("Knockback growth", string.Join('/', move.Hitboxes.Select(hitbox => hitbox.KnockbackGrowth)), hitboxSummary);
            AddString("Set knockback", string.Join('/', move.Hitboxes.Select(hitbox => hitbox.SetKnockback)), hitboxSummary);
            AddString("Shieldstun", string.Join('/', move.Hitboxes.Select(hitbox => hitbox.Shieldstun)), hitboxSummary);

            return hitboxSummary;
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

        private static void AddIfPossible(string key, int? value, StringBuilder stringBuilder)
        {
            if (!value.HasValue || value <= 0)
            {
                return;
            }

            stringBuilder.Append($"**{key}:** {value}\n");
        }

        private static void AddIfPossible(string key, double? value, StringBuilder stringBuilder)
        {
            if (!value.HasValue || value <= 0)
            {
                return;
            }

            stringBuilder.Append($"**{key}:** {value}\n");
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
