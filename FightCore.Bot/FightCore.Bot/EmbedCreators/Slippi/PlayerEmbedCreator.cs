using Discord;
using FightCore.Bot.Configuration;
using FightCore.Bot.EmbedCreators.Base;
using FightCore.SlippiStatsOnline.Models;
using Microsoft.Extensions.Options;
using System.Text;

namespace FightCore.Bot.EmbedCreators.Slippi
{
    public class PlayerEmbedCreator : BaseEmbedCreator
    {
        public PlayerEmbedCreator(IOptions<EmbedSettings> embedSettings) : base(embedSettings)
        {
        }

        public Embed CreatePlayerEmbed(PlayerResult player)
        {
            var embedBuilder = new EmbedBuilder();
            embedBuilder = AddFightCoreFooter(embedBuilder);
            embedBuilder.AddField("Source", "Statistics by https://slippistats.online");
            embedBuilder = CreatePlayerStats(embedBuilder, player.Player);
            return embedBuilder.Build();
        }

        public Embed CreateVsEmbed(PlayerResult player)
        {
            var embedBuilder = new EmbedBuilder();
            embedBuilder = AddFightCoreFooter(embedBuilder);
            embedBuilder = CreatePlayerStats(embedBuilder, player.Player);
            embedBuilder = CreatePlayerStats(embedBuilder, player.Opponent);
            embedBuilder.AddField("Source", "Statistics by https://slippistats.online");
            return embedBuilder.Build();
        }

        private EmbedBuilder CreatePlayerStats(EmbedBuilder embedBuilder, Player player, string playerName = null)
        {
            if (string.IsNullOrWhiteSpace(player.PlayerData.Tag))
            {
                player.PlayerData.Tag = "Opponent";
            }

            embedBuilder.AddField("Player", $"**First known name**: {player.PlayerData.Tag}", true);
            var overallStatsBuilder = new StringBuilder();
            AddField(overallStatsBuilder, "Wins", player.Overall.Wins.ToString());
            AddField(overallStatsBuilder, "LRA + Starts", player.Overall.LRAStarts.ToString());
            AddField(overallStatsBuilder, "Stocks taken", player.Overall.StocksTaken.ToString());
            AddField(overallStatsBuilder, "Four stocks", player.Overall.FourStocks.ToString());
            embedBuilder.AddField($"Overall {playerName}", overallStatsBuilder.ToString(), true);

            var averageStatsBuilder = new StringBuilder();
            AddField(averageStatsBuilder, "Stocks taken", player.Average.StocksTaken);
            AddField(averageStatsBuilder, "Stocks Differential", player.Average.StockDifferential);
            AddField(averageStatsBuilder, "Inputs Per Minute", player.Average.APM);
            AddField(averageStatsBuilder, "Kill percentage", player.Average.KillPercent);
            AddField(averageStatsBuilder, "Neutral win ratio", player.Average.NeutralWinRatio);
            AddField(averageStatsBuilder, "Beneficial counter hit ratio", player.Average.BeneficialCounterHitRatio);
            AddField(averageStatsBuilder, "Beneficial trade ratio", player.Average.BeneficialTradeRatio);
            AddField(averageStatsBuilder, "Conversion ratio", player.Average.ConversionRatio);
            AddField(averageStatsBuilder, "Total Damage", player.Average.TotalDamage);
            AddField(averageStatsBuilder, "Openings per kill", player.Average.OpeningsPerKill);
            AddField(averageStatsBuilder, "Total Damage", player.Average.TotalDamage);
            embedBuilder.AddField($"Average {playerName}", averageStatsBuilder.ToString(), true);

            return embedBuilder;
        }

        private void AddField(StringBuilder builder, string name, string value)
        {
            builder.AppendLine($"**{name}:** {value}");
        }
    }
}
