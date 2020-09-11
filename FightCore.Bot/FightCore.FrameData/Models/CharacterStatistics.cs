using System;
using System.Collections.Generic;
using System.Text;

namespace FightCore.FrameData.Models
{
    public class CharacterStatistics
    {
        public long Id { get; set; }

        public int Weight { get; set; }

        public double Gravity { get; set; }

        public double WalkSpeed { get; set; }

        public double RunSpeed { get; set; }

        public int WaveDashLengthRank { get; set; }

        public int PLAIntangibilityFrames { get; set; }

        public int JumpSquat { get; set; }

        public bool CanWallJump { get; set; }

        public string Notes { get; set; }
    }
}
