using System;
using System.Collections.Generic;
using System.Text;

namespace FightCore.SlippiStatsOnline.Models
{
    public class Player
    {
        public PlayerData PlayerData { get; set; }

        public OverallStats Overall { get; set; }

        public AverageStats Average { get; set; }
    }
}
