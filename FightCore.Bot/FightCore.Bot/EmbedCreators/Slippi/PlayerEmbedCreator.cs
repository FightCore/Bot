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
            embedBuilder = AddFooter(embedBuilder);
            embedBuilder.AddField("Source", "Statistics by https://slippistats.online");
            embedBuilder = CreatePlayerStats(embedBuilder, player.Player);
            return embedBuilder.Build();
        }

        public Embed CreateVsEmbed(PlayerResult player)
        {
            var embedBuilder = new EmbedBuilder();
            embedBuilder = AddFooter(embedBuilder);
            embedBuilder = CreatePlayerStats(embedBuilder, player.Player);
            embedBuilder = CreatePlayerStats(embedBuilder, player.Opponent);
            embedBuilder.AddField("Source", "Statistics by https://slippistats.online");
            return embedBuilder.Build();
        }

        private EmbedBuilder CreatePlayerStats(EmbedBuilder embedBuilder, Player player)
        {
            if (string.IsNullOrWhiteSpace(player.PlayerData.Tag))
            {
                player.PlayerData.Tag = "Opponent";
            }

            var overallStatsBuilder = new StringBuilder();
            AddField(overallStatsBuilder, "Name", player.PlayerData.Tag);
            AddField(overallStatsBuilder, "Wins", player.Overall.Wins.ToString());
            AddField(overallStatsBuilder, "LRA + Starts", player.Overall.LRAStarts.ToString());
            AddField(overallStatsBuilder, "Stocks taken", player.Overall.StocksTaken.ToString());
            AddField(overallStatsBuilder, "Four stocks", player.Overall.FourStocks.ToString());
            overallStatsBuilder.AppendLine("\n**Average**\n");
            AddField(overallStatsBuilder, "Stocks taken", player.Average.StocksTaken);
            AddField(overallStatsBuilder, "Stocks Differential", player.Average.StockDifferential);
            AddField(overallStatsBuilder, "Inputs Per Minute", player.Average.APM);
            AddField(overallStatsBuilder, "Kill percentage", player.Average.KillPercent);
            AddField(overallStatsBuilder, "Neutral win ratio", player.Average.NeutralWinRatio);
            AddField(overallStatsBuilder, "Beneficial counter hit ratio", player.Average.BeneficialCounterHitRatio);
            AddField(overallStatsBuilder, "Beneficial trade ratio", player.Average.BeneficialTradeRatio);
            AddField(overallStatsBuilder, "Conversion ratio", player.Average.ConversionRatio);
            AddField(overallStatsBuilder, "Total Damage", player.Average.TotalDamage);
            AddField(overallStatsBuilder, "Openings per kill", player.Average.OpeningsPerKill);
            AddField(overallStatsBuilder, "Total Damage", player.Average.TotalDamage);
            embedBuilder.AddField($"Overall", overallStatsBuilder.ToString(), true);

            return embedBuilder;
        }

        private void AddField(StringBuilder builder, string name, string value)
        {
            builder.AppendLine($"**{name}:** {value}");
        }
    }
}
