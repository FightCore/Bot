using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace FightCore.SlippiStatsOnline.Models
{
    public class PlayerResult
    {
        [JsonProperty("numOfGames")]
        public int NumberOfGames { get; set; }

        public int Timeouts { get; set; }

        public Player Player { get; set; }

        public Player Opponent { get; set; }

        public OverallStats Overall { get; set; }

        public AverageStats Average { get; set; }
    }
}
