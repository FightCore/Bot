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

namespace FightCore.Bot.EmbedCreators.Characters
{
    public class CharacterInfoEmbedCreator : BaseEmbedCreator
    {


        public CharacterInfoEmbedCreator(IOptions<EmbedSettings> embedSettings) : base(embedSettings)
        {
        }

        public Embed CreateInfoEmbed(Character character, Misc misc)
        {
            var embedBuilder = new EmbedBuilder {Title = character.Name};

            if (EmbedSettings.FightCoreInfo)
            {
                embedBuilder.AddField("General information", ShortenString(character.GeneralInformation, 250));

                if (character.NotablePlayers.Any())
                {
                    embedBuilder.AddField("Notable players",
                        string.Join(", ", character.NotablePlayers.Take(6).Select(player => player.Name))
                    );
                }
            }

            if (character.StockIcon != null)
                embedBuilder.WithThumbnailUrl(character.StockIcon.Url);

            if (character.CharacterImage != null)
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

            embedBuilder.WithUrl($"https://www.fightcore.gg/character/{character.Id}");
            embedBuilder = AddFooter(embedBuilder);
            return embedBuilder.Build();
        }

        public Embed CreateMoveEmbed(WrapperCharacter character, NormalizedEntity move, Character fightCoreCharacter)
        {
            return move switch
            {
                Attack attack => CreateAttackEmbed(character, attack, fightCoreCharacter),
                Dodge dodge => CreateDodgeEmbed(character, dodge, fightCoreCharacter),
                Grab grab => CreateGrabEmbed(character, grab, fightCoreCharacter),
                Throw @throw => CreateThrowEmbed(character, @throw, fightCoreCharacter),
                _ => throw new NotImplementedException()
            };
        }

        public Embed CreateMoveListEmbed(WrapperCharacter character, List<NormalizedEntity> moves, Character fightCoreCharacter)
        {
            var embedBuilder = new EmbedBuilder();
            embedBuilder.WithUrl($"http://meleeframedata.com/{character.NormalizedName}")
                .WithThumbnailUrl(fightCoreCharacter.StockIcon.Url.Replace(" ", "+"));
            embedBuilder.Title = $"{character.Name} - Moves";

            embedBuilder.AddField("Moves", ShortenField(
                string.Join(", ", moves.Select(move => move.Name))))
                .AddField("Help", "To check out a move use:\n`-character move {CHARACTER NAME} {MOVE NAME}`\n" +
                                          "For example: `-character move Fox u-smash`");

            embedBuilder = AddFooter(embedBuilder);
            return embedBuilder.Build();
        }

        public Embed CreateHelpEmbed()
        {
            var embedBuilder = new EmbedBuilder();
            embedBuilder.WithTitle("Help");
            embedBuilder.AddField("Character statistics",
                "`-c {{NAME}}`\n" +
                "Use this command to get information about a character " +
                "Note that character names with spaces need to have quotes, " +
                "`-c \"Ice climbers\"`. Most of these names can shortened (Ice Climbers = ics)\n" +
                "Example: `-c Kirby`");
            embedBuilder.AddField("Move list",
                "`-c moves {{NAME}}`\n" +
                "Use this command to get a list of moves that are available for that character. " +
                "Note that the special move names can also be shortened to their input (Fox Blaster = neutral b).\n" +
                "Example: `-c moves g&w`");
            embedBuilder.AddField("Move frame data",
                "`-c m {{CHARACTER}} {{MOVE}}`\n" +
                "Use this command to get the frame data about a move, note that character can only be one word here!" +
                " Shorten your character names to make them fit (Ice Climbers = ics)\n" +
                "Example: `-c m falcon u-tilt`");
            embedBuilder = AddFooter(embedBuilder);

            return embedBuilder.Build();
        }

        private Embed CreateAttackEmbed(WrapperCharacter character, Attack move, Character fightCoreCharacter)
        {
            var embedBuilder = CreateDefaultFrameDataEmbed(character, move, fightCoreCharacter);

            var frameDataBuilder = new StringBuilder();
            AddIfPossible("Total frames", move.Total, frameDataBuilder);
            AddIfPossible("Hit start", move.Start, frameDataBuilder);
            AddIfPossible("Hit end", move.End, frameDataBuilder);
            AddIfPossible("Shield stun", move.Stun, frameDataBuilder);
            AddIfPossible("Percent", move.Percent, frameDataBuilder);
            AddIfPossible("Percent (weak hit)", move.PercentWeak, frameDataBuilder);
            AddIfPossible("IASA", move.Iasa, frameDataBuilder);

            if (move.AutoCancelStart.HasValue && move.AutoCancelEnd.HasValue && move.AutoCancelStart > -1 && move.AutoCancelEnd > -1)
            {
                frameDataBuilder.AppendLine($"**Won't Auto Cancel:** {move.AutoCancelStart}-{move.AutoCancelEnd}");
            }

            AddIfPossible("Land lag", move.LandingLag, frameDataBuilder);
            AddIfPossible("L-Canceled", move.LCanceledLandingLag, frameDataBuilder);

            if (!string.IsNullOrWhiteSpace(move.Notes))
            {
                frameDataBuilder.AppendLine($"**Notes:** {move.Notes}");
            }

            embedBuilder.AddField("Frame Data", frameDataBuilder.ToString(), true);


            embedBuilder = AddMeleeFrameDataInfo(embedBuilder);

            return embedBuilder.Build();
        }

        private Embed CreateDodgeEmbed(WrapperCharacter character, Dodge dodge, Character fightCoreCharacter)
        {
            var embedBuilder = CreateDefaultFrameDataEmbed(character, dodge, fightCoreCharacter);
            var frameDataBuilder = new StringBuilder();
            AddIfPossible("Start", dodge.Start, frameDataBuilder);
            AddIfPossible("Invulnerable ends", dodge.EndInvulnerable, frameDataBuilder);
            AddIfPossible("Total", dodge.Total, frameDataBuilder);

            embedBuilder.AddField("Frame data", frameDataBuilder.ToString());

            embedBuilder = AddMeleeFrameDataInfo(embedBuilder);
            return embedBuilder.Build();
        }

        private Embed CreateGrabEmbed(WrapperCharacter character, Grab grab, Character fightCoreCharacter)
        {
            var embedBuilder = CreateDefaultFrameDataEmbed(character, grab, fightCoreCharacter);


            var frameDataBuilder = new StringBuilder();
            AddIfPossible("Start", grab.Start, frameDataBuilder);
            AddIfPossible("End", grab.End, frameDataBuilder);
            AddIfPossible("Total", grab.Total, frameDataBuilder);
            embedBuilder.AddField("Frame data", frameDataBuilder.ToString());

            if (!string.IsNullOrWhiteSpace(grab.Notes))
            {
                embedBuilder.AddField("Notes", grab.Notes, true);
            }

            embedBuilder = AddMeleeFrameDataInfo(embedBuilder);
            return embedBuilder.Build();
        }

        private Embed CreateThrowEmbed(WrapperCharacter character, Throw throwMove, Character fightCoreCharacter)
        {
            var embedBuilder = CreateDefaultFrameDataEmbed(character, throwMove, fightCoreCharacter);


            var frameDataBuilder = new StringBuilder();
            AddIfPossible("Start", throwMove.Start, frameDataBuilder);
            AddIfPossible("End", throwMove.End, frameDataBuilder);
            AddIfPossible("Total", throwMove.Total, frameDataBuilder);
            AddIfPossible("Percent", throwMove.Percent, frameDataBuilder);
            embedBuilder.AddField("Frame data", frameDataBuilder.ToString());

            if (!string.IsNullOrWhiteSpace(throwMove.Notes))
            {
                embedBuilder.AddField("Notes", throwMove.Notes, true);
            }

            embedBuilder = AddMeleeFrameDataInfo(embedBuilder);
            return embedBuilder.Build();
        }

        private EmbedBuilder CreateDefaultFrameDataEmbed(WrapperCharacter character, NormalizedEntity move,
            Character fightCoreCharacter)
        {
            var characterName = SearchHelper.Normalize(move.Character);
            var moveName = SearchHelper.Normalize(move.NormalizedType);
            var embedBuilder = new EmbedBuilder()
                .WithUrl($"http://meleeframedata.com/{move.Character}")
                .WithThumbnailUrl(fightCoreCharacter.StockIcon.Url.Replace(" ", "+"))
                .WithImageUrl($"https://i.fightcore.gg/melee/moves/{characterName}/{moveName}.gif");

            embedBuilder.Title = $"{character.Name} - {move.Name}";
            return AddFooter(embedBuilder);
        }

        private static EmbedBuilder AddMeleeFrameDataInfo(EmbedBuilder builder)
        {
            return builder.AddField("Melee Frame Data",
                "All of this data is provided by http://meleeframedata.com.");
        }

        private static void AddIfPossible(string key, int? value, StringBuilder stringBuilder)
        {
            if (!value.HasValue || value <= 0)
            {
                return;
            }

            stringBuilder.Append($"**{key}:** {value}\n");
        }
    }
}
