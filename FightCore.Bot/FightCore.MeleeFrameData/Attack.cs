using System.ComponentModel.DataAnnotations.Schema;

namespace FightCore.MeleeFrameData
{
    public class Attack : NormalizedEntity
    {
        public string Move { get; set; }

        public int? Start { get; set; }

        public int? End { get; set; }

        public int? Total { get; set; }

        public int? Iasa { get; set; }

        public int? Stun { get; set; }

        public int? Percent { get; set; }

        [Column("percent_weak")]
        public int? PercentWeak { get; set; }

        public string Notes { get; set; }

        [Column("auto_cancel_s")]
        public int? AutoCancelStart { get; set; }

        [Column("auto_cancel_e")]
        public int? AutoCancelEnd { get; set; }

        [Column("land_lag")]
        public int? LandingLag { get; set; }

        [Column("cancel_lag")]
        public int? LCanceledLandingLag { get; set; }

        public string Id => Character + Move;
    }
}
