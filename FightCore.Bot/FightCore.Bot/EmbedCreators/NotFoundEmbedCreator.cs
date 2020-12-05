using System.Collections.Generic;
using System.Text;
using Discord;
using FightCore.Bot.Configuration;
using FightCore.Bot.EmbedCreators.Base;
using FightCore.Logic.Search;
using Microsoft.Extensions.Options;

namespace FightCore.Bot.EmbedCreators
{
    public class NotFoundEmbedCreator : BaseEmbedCreator
    {
        public NotFoundEmbedCreator(IOptions<EmbedSettings> embedSettings) : base(embedSettings)
        {
        }

        public Embed Create(Dictionary<string, string> values)
        {
            var embedBuilder = new EmbedBuilder();
            AddFooter(embedBuilder);
            embedBuilder.WithTitle("Not found");
            var stringBuilder = new StringBuilder("Nothing was found for the following parameters:\n");
            foreach (var (key, value) in values)
            {
                stringBuilder.AppendLine($"{key}: `{SearchHelper.Normalize(value)}`");
            }

            embedBuilder.WithDescription(stringBuilder.ToString());

            return embedBuilder.Build();
        }


    }
}
