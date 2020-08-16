using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FightCore.MeleeFrameData
{
    public class NormalizedEntity
    {
        [NotMapped]
        public string NormalizedName { get; set; }

        [NotMapped]
        public string NormalizedType { get; set; }

        [NotMapped]
        public string NormalizedCharacter { get; set; }

        public string Name { get; set; }

        [Column("char")]
        public string Character { get; set; }
    }
}
