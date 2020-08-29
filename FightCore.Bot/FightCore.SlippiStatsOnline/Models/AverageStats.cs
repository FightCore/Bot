using System;
using System.Collections.Generic;
using System.Text;

namespace FightCore.SlippiStatsOnline.Models
{
    public class AverageStats
    {
        public string StocksTaken { get; set; }

        public string StockDifferential { get; set; }

        public string TotalDamage { get; set; }

        public string APM { get; set; }

        public string OpeningsPerKill { get; set; }

        public string NeutralWinRatio { get; set; }

        public string ConversionRatio { get; set; }

        public string BeneficialCounterHitRatio { get; set; }

        public string BeneficialTradeRatio { get; set; }

        public string KillPercent { get; set; }
    }
}
