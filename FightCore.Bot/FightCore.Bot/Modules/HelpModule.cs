using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using FightCore.Bot.EmbedCreators.Characters;

namespace FightCore.Bot.Modules
{
    [Group("help")]
    [Alias("h")]
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        private readonly CharacterInfoEmbedCreator _characterInfoEmbedCreator;

        public HelpModule(CharacterInfoEmbedCreator characterInfoEmbedCreator)
        {
            _characterInfoEmbedCreator = characterInfoEmbedCreator;
        }

        [Command]
        public async Task ShowHelpEmbed()
        {
            await ReplyAsync("", embed: _characterInfoEmbedCreator.CreateHelpEmbed());
        }
    }
}
