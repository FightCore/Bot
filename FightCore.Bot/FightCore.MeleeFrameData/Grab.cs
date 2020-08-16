using System;
using System.Collections.Generic;
using System.Text;

namespace FightCore.MeleeFrameData
{
    public class Grab : NormalizedEntity
    {
        public string Type { get; set; }

        public int? Start { get; set; }

        public int? End { get; set; }

        public int? Total { get; set; }

        public string Notes { get; set; }
    }
}
