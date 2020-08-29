using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using FightCore.Bot.EmbedCreators;
using FightCore.Bot.EmbedCreators.Slippi;
using FightCore.SlippiStatsOnline;

namespace FightCore.Bot.Modules
{
    [Group("slippi")]
    public class SlippiModule : ModuleBase<SocketCommandContext>
    {
        private readonly ISlippiPlayerService _slippiPlayerService;
        private readonly NotFoundEmbedCreator _notFoundEmbedCreator;
        private readonly PlayerEmbedCreator _playerEmbedCreator;

        public SlippiModule(ISlippiPlayerService slippiPlayerService,
            NotFoundEmbedCreator notFoundEmbedCreator,
            PlayerEmbedCreator playerEmbedCreator)
        {
            _slippiPlayerService = slippiPlayerService;
            _notFoundEmbedCreator = notFoundEmbedCreator;
            _playerEmbedCreator = playerEmbedCreator;
        }

        [Command]
        public async Task GetPlayerInfo([Remainder] string tags)
        {
            var splitTags = tags.Split(" vs ");
            string opponentCode = null;
            if (splitTags.Length == 2)
            {
                tags = splitTags[0];
                opponentCode = splitTags[1];
            }

            var playerData = await _slippiPlayerService.GetForUser(tags, opponentCode);

            if (playerData == null)
            {
                var notFoundEmbed = _notFoundEmbedCreator.Create(new Dictionary<string, string> {{nameof(playerData), tags } });
                await ReplyAsync(string.Empty, embed: notFoundEmbed);
                return;
            }
           
            var embed = playerData.Opponent != null
                ? _playerEmbedCreator.CreateVsEmbed(playerData)
                : _playerEmbedCreator.CreatePlayerEmbed(playerData);

            await ReplyAsync(string.Empty, embed: embed);
        }
    }
}
