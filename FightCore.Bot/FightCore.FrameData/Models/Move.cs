﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FightCore.FrameData.Models
{
    public class Move
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string NormalizedName { get; set; }

        public string NormalizedMoveName { get; set; }

        public List<Hitbox> Hitboxes { get; set; }

        #region Land lag
        public int? LandLag { get; set; }

        public int? LCanceledLandLag { get; set; }
        #endregion

        public int TotalFrames { get; set; }

        /// <summary>
        /// Interrupt-able As Soon As
        /// </summary>
        public int? IASA { get; set; }

        public int? AutoCancelBefore { get; set; }

        public int? AutoCancelAfter { get; set; }

        public int? Start { get; set; }

        public int? End { get; set; }

        public MoveType Type { get; set; }

        public string Notes { get; set; }

        public int? Percent { get; set; }

        public long CharacterId { get; set; }

        public string Source { get; set; }
    }
}
