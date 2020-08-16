using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FightCore.MeleeFrameData
{
    public class Misc : NormalizedEntity
    {
        public int Weight { get; set; }

        public double Gravity { get; set; }

        [Column("walk_speed")]
        public double WalkSpeed { get; set; }

        [Column("run_speed")]
        public double RunSpeed { get; set; }

        [Column("wd_length")]
        public int WaveDashLengthRank { get; set; }

        [Column("wd_frames")]
        public int PLAIntangibilityFrames { get; set; }

        [Column("jump_squat")]
        public int JumpSquat { get; set; }

        [Column("wall_jump")]
        public bool CanWallJump { get; set; }

        public string Notes { get; set; }
    }
}
