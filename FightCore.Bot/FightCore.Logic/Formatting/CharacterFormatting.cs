using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using FightCore.FrameData.Models;
using FightCore.Logic.Aliasses.FrameData;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Move = FightCore.FrameData.Models.Move;

namespace FightCore.Logic.Formatting
{
    public class CharacterFormatting
    {
        private readonly string _headerFormat;

        public CharacterFormatting(string headerFormat = "{0}")
        {
            _headerFormat = headerFormat;
        }

        public StringBuilder CreateCharacterMiscData(CharacterInfo info, WrapperCharacter wrapperCharacter)
        {
            var miscStringBuilder = new StringBuilder();
            AddString("Discord", info?.Discord, miscStringBuilder);
            AddString("MeleeFrameData", info?.MeleeFrameData, miscStringBuilder);
            AddString("SSB Wiki", info?.SsbWiki, miscStringBuilder);
            AddString("FightCore", $"https://www.fightcore.gg/character/{wrapperCharacter.FightCoreId}", miscStringBuilder);
            return miscStringBuilder;
        }

        public StringBuilder CreateCharacterFrameData(CharacterStatistics statistics)
        {
            var frameDataStringBuilder = new StringBuilder();
            frameDataStringBuilder.AppendLine($"{string.Format(_headerFormat, "Weight")} {statistics.Weight}");
            frameDataStringBuilder.AppendLine($"{string.Format(_headerFormat, "Gravity")} {statistics.Gravity}");
            frameDataStringBuilder.AppendLine($"{string.Format(_headerFormat, "Can wall jump")} {statistics.CanWallJump}");
            frameDataStringBuilder.AppendLine($"{string.Format(_headerFormat, "Jump squat")} {statistics.JumpSquat}");
            return frameDataStringBuilder;
        }

        public StringBuilder CreateMovementData(CharacterStatistics statistics)
        {
            var movementStringBuilder = new StringBuilder();
            AddIfPossible("Walk speed", statistics.WalkSpeed, movementStringBuilder);
            AddIfPossible("Initial Dash speed", statistics.InitialDash, movementStringBuilder);
            AddIfPossible("Initial dash frames", statistics.DashFrames, movementStringBuilder);
            AddIfPossible("Run speed", statistics.RunSpeed, movementStringBuilder);
            AddIfPossible("Wave dash length (rank)", statistics.WaveDashLengthRank, movementStringBuilder);
            AddIfPossible("Perfect wave dash length", statistics.WaveDashLength, movementStringBuilder);
            AddIfPossible("PLA Intangibility Frames", statistics.PLAIntangibilityFrames, movementStringBuilder);
            AddString("Source", "https://smashboards.com/threads/ultimate-ground-movement-analysis-turbo-edition.392367/", movementStringBuilder);
            return movementStringBuilder;
        }

        public StringBuilder CreateMoveData(Move move)
        {
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
            return frameDataBuilder;
        }

        public StringBuilder CreateHitboxData(Move move)
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

        private void AddIfPossible(string key, int? value, StringBuilder stringBuilder)
        {
            if (!value.HasValue || value <= 0)
            {
                return;
            }

            stringBuilder.Append($"{string.Format(_headerFormat, key)} {value}\n");
        }

        private void AddIfPossible(string key, double? value, StringBuilder stringBuilder)
        {
            if (!value.HasValue || value <= 0)
            {
                return;
            }

            stringBuilder.Append($"{string.Format(_headerFormat, key)} {value}\n");
        }

        private void AddString(string key, string value, StringBuilder stringBuilder)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            stringBuilder.AppendLine($"{string.Format(_headerFormat, key)} {value}");
        }
    }
}
