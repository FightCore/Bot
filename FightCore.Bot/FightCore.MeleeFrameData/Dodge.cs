using System.ComponentModel.DataAnnotations.Schema;

namespace FightCore.MeleeFrameData
{
    public class Dodge : NormalizedEntity
    {
        public string Type { get; set; }

        public int? Start { get; set; }

        [Column("inv_end")]
        public int? EndInvulnerable { get; set; }

        public int? Total { get; set; }
    }
}
