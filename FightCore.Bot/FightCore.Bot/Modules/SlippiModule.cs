﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using FightCore.Bot.Configuration;
using FightCore.Bot.EmbedCreators;
using FightCore.Bot.EmbedCreators.Slippi;
using FightCore.SlippiStatsOnline;
using Microsoft.Extensions.Options;

namespace FightCore.Bot.Modules
{
    [Group("slippi")]
    public class SlippiModule : ModuleBase<SocketCommandContext>
    {
        private readonly ISlippiPlayerService _slippiPlayerService;
        private readonly NotFoundEmbedCreator _notFoundEmbedCreator;
        private readonly PlayerEmbedCreator _playerEmbedCreator;
        private readonly bool _enabled;

        public SlippiModule(ISlippiPlayerService slippiPlayerService,
            NotFoundEmbedCreator notFoundEmbedCreator,
            PlayerEmbedCreator playerEmbedCreator,
            IOptions<ModuleSettings> moduleSettings)
        {
            _slippiPlayerService = slippiPlayerService;
            _notFoundEmbedCreator = notFoundEmbedCreator;
            _playerEmbedCreator = playerEmbedCreator;
            _enabled = moduleSettings.Value.SlippiStats;
        }

        [Command]
        public async Task GetPlayerInfo([Remainder] string tags)
        {
            if (!_enabled)
            {
                return;
            }

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
