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

        protected EmbedBuilder AddFightCoreFooter(EmbedBuilder embedBuilder)
        {
            return embedBuilder.WithFooter("Visit us at www.FightCore.gg")
                .WithCurrentTimestamp()
                .WithColor(Color.Red);
        }


        protected EmbedBuilder AddFooter(EmbedBuilder builder)
        {
            switch (EmbedSettings.FooterType)
            {
                case FooterType.FightCore:
                    builder = AddFightCoreFooter(builder);
                    break;
                case FooterType.MeleeOnline:
                    builder.WithFooter("Melee Online Frame Data", "https://cdn.discordapp.com/icons/724998978113896508/a_a765306c32c21eca27349539154983a9.webp?size=128")
                        .WithColor(Color.Green)
                        .WithCurrentTimestamp();
                    break;
                case FooterType.DutchNetplay:
                    builder.WithFooter("Dutch Melee Discord", "https://cdn.discordapp.com/icons/283580261520769026/df9cab8218c661ebd2ad0c4550969504.webp?size=128")
                        .WithColor(Color.Red)
                        .WithCurrentTimestamp();
                    break;
                case FooterType.MeleeNewbieNetplay:
                    builder.WithFooter("Newbie Netplay Frame Data", "https://i.fightcore.gg/clients/newbienetplaysmash.png")
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
