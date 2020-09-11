using System;
using System.Collections.Generic;
using System.Text;

namespace FightCore.FrameData.Models
{
    public class Hitbox
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public long Damage { get; set; }

        public long Angle { get; set; }

        // Ikneeddata = kg.
        public long KnockbackGrowth { get; set; }

        // Ikneeddata = wbk
        public long SetKnockback { get; set; }

        // Ikneeddata = bk
        public long BaseKnockback { get; set; }

        public string Effect { get; set; }

        public int HitlagAttacker { get; set; }

        public int HitlagDefender { get; set; }

        public int Shieldstun { get; set; }
    }
}
