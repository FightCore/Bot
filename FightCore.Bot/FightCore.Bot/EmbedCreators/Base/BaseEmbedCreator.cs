using System;
using Discord;
using FightCore.Bot.Configuration;
using Microsoft.Extensions.Options;

namespace FightCore.Bot.EmbedCreators.Base
{
    public abstract class BaseEmbedCreator
    {
        protected readonly EmbedSettings EmbedSettings;
        protected const int MaxFieldLength = 1024;
        protected const int MaxFieldCount = 25;
        protected const int MaxDescriptionLength = 2048;

        public BaseEmbedCreator(IOptions<EmbedSettings> embedSettings)
        {
            EmbedSettings = embedSettings.Value;
        }

        protected string ShortenString(string text, int length)
        {
            if (text.Length < length)
            {
                return text;
            }

            return text.Substring(0, length -3) + "...";
        }

        protected string ShortenDescription(string text)
        {
            return ShortenString(text, MaxDescriptionLength);
        }

        protected string ShortenField(string text)
        {
            return ShortenString(text, MaxDescriptionLength);
        }

        protected bool CheckString(string text)
        {
            if (text.Contains("@everyone") || text.Contains("@here"))
            {
                return false;
            }

            return true;
        }


        protected EmbedBuilder AddFooter(EmbedBuilder builder)
        {
            const string version = "v1.1.1b";
            switch (EmbedSettings.FooterType)
            {
                case FooterType.FightCore:
                    builder.WithFooter($"FightCore Bot Version {version}", "https://i.fightcore.gg/clients/fightcore.png")
                        .WithCurrentTimestamp()
                        .WithColor(Color.Red);
                    break;
                case FooterType.MeleeOnline:
                    builder.WithFooter($"Melee Online Frame Data Version {version}", "https://i.fightcore.gg/clients/meleeonline.webp")
                        .WithColor(Color.Green)
                        .WithCurrentTimestamp();
                    break;
                case FooterType.DutchNetplay:
                    builder.WithFooter($"Dutch Melee Discord Version {version}", "https://i.fightcore.gg/clients/dutchnetplaydiscord.webp")
                        .WithColor(Color.Red)
                        .WithCurrentTimestamp();
                    break;
                case FooterType.MeleeNewbieNetplay:
                    builder.WithFooter($"Newbie Netplay Frame Data Version {version}", "https://i.fightcore.gg/clients/newbienetplaysmash.png")
                        .WithColor(Color.Red)
                        .WithCurrentTimestamp();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return builder;
        }
    }
}
