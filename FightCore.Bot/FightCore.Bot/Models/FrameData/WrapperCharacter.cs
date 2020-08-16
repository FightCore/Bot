using System.Collections.Generic;

namespace FightCore.Bot.Models.FrameData
{
    public class WrapperCharacter
    {
        public string Name { get; set; }

        public string NormalizedName { get; set; }

        public List<string> Names { get; set; }

        public long FightCoreId { get; set; }

        public Dictionary<string, string> Moves { get; set; }
    }
}
