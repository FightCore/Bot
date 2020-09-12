using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;
using FightCore.Api.Models;
using FightCore.Bot.Configuration;
using FightCore.Bot.EmbedCreators.Base;
using FightCore.Bot.Helpers;
using FightCore.Bot.Models.FrameData;
using FightCore.MeleeFrameData;
using Microsoft.Extensions.Options;
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

        public Embed CreateInfoEmbed(WrapperCharacter wrapperCharacter, Character character, Misc misc)
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

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"**Weight:** {misc.Weight}");
            stringBuilder.AppendLine($"**Gravity:** {misc.Gravity}");
            stringBuilder.AppendLine($"**Walk speed:** {misc.WalkSpeed}");
            stringBuilder.AppendLine($"**Run speed:** {misc.RunSpeed}");
            stringBuilder.AppendLine($"**Wave dash length (rank):** {misc.WaveDashLengthRank}");
            stringBuilder.AppendLine($"**PLA Intangibility Frames:** {misc.PLAIntangibilityFrames}");
            stringBuilder.AppendLine($"**Can wall jump:** {misc.CanWallJump}");
            stringBuilder.AppendLine($"**Jump squat:** {misc.JumpSquat}");
            embedBuilder.AddField("Frame data", stringBuilder.ToString());

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
            frameDataBuilder.AppendLine("Data by https://www.meleeframedata.com");

            if (!string.IsNullOrWhiteSpace(frameDataBuilder.ToString()))
            {
                embedBuilder.AddField("Frame Data", frameDataBuilder.ToString(), true);
            }

            var hitboxStringBuilder = AddHitboxDataToEmbedBuilder(move);
            if (!string.IsNullOrWhiteSpace(hitboxStringBuilder.ToString()))
            {
                hitboxStringBuilder.AppendLine();
                hitboxStringBuilder.AppendLine("Hitbox data by https://www.ikneedata.com");
                hitboxStringBuilder.AppendLine("Colored hitboxes follow the following rules:");
                hitboxStringBuilder.AppendLine("id0=Red, id1=Green, id2=Purple, id3=Orange");
                hitboxStringBuilder.AppendLine("Credits to 20XX for the code and MWStage for the colored GIFs");
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
            AddString("Kockback growth", string.Join('/', move.Hitboxes.Select(hitbox => hitbox.KnockbackGrowth)), hitboxSummary);
            AddString("Set knockback", string.Join('/', move.Hitboxes.Select(hitbox => hitbox.SetKnockback)), hitboxSummary);

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

            embedBuilder.AddField("Moves", ShortenField(
                string.Join(", ", moves.Select(move => move.Name))))
                .AddField("Help", "To check out a move use:\n`" + _prefix + "c {CHARACTER NAME} {MOVE NAME}`\n" +
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

            if (fightCoreCharacter != null)
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
