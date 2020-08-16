using System;
using System.Linq;
using System.Text;
using Discord;
using FightCore.Bot.Configuration;
using FightCore.Bot.EmbedCreators.Base;
using Microsoft.Extensions.Options;

namespace FightCore.Bot.EmbedCreators.Tournaments
{
    public class TournamentEmbedCreator : BaseEmbedCreator
    {
        public TournamentEmbedCreator(IOptions<EmbedSettings> embedSettings) : base(embedSettings)
        {
        }

        //public Embed Create(Tournament tournament)
        //{
        //    var embedBuilder = AddFooter(new EmbedBuilder());
        //    embedBuilder.Title = tournament.Name;
        //    embedBuilder.Url = $"https://www.smash.gg/{tournament.Slug}";
        //    if (tournament.Images.Any())
        //    {
        //        embedBuilder.WithThumbnailUrl(tournament.Images[0].Url);
        //    }

        //    var generalInformationBuilder = new StringBuilder();
        //    AddIfNotNull(tournament.StartAtDateTime?.ToLongDateString(), "Starts", generalInformationBuilder);
        //    AddIfNotNull(tournament.VenueName, "Venue", generalInformationBuilder);
            

        //    foreach (var grouping in tournament.Events.GroupBy(@event => @event.Videogame.Name))
        //    {
        //        var stringBuilder = new StringBuilder();
        //        foreach (var @event in grouping)
        //        {
        //            var isSpoiler = @event.StartAtDateTime.HasValue &&
        //                            TimeSpan.FromDays(8) > DateTime.Now - @event.StartAtDateTime.Value;

        //            if (!@event.StartAtDateTime.HasValue || @event.StartAtDateTime > DateTime.Now)
        //            {
        //                isSpoiler = false;
        //            }

        //            stringBuilder.AppendLine($"**{@event.Name}**");
        //            if (isSpoiler)
        //            {
        //                stringBuilder.AppendLine("Recent event! Adding spoiler tag\n||");
        //            }
        //            foreach (var standing in @event.Standings.Nodes)
        //            {
        //                stringBuilder.AppendLine($"{standing.Placement}: {standing.Entrant.Name}");
        //                    //: $"||{standing.Placement}: {standing.Entrant.Name}||");
        //            }
        //            if (isSpoiler)
        //            {
        //                stringBuilder.AppendLine("||");
        //            }
        //        }
        //        embedBuilder.AddField(grouping.Key, stringBuilder.ToString(), true);
        //    }

        //    return embedBuilder.Build();

        //}

        private void AddIfNotNull(string value, string key, StringBuilder builder)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            builder.AppendLine($"**{key}**: {value}");
        }
    }
}
