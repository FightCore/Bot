using System;
using System.Collections.Generic;
using System.Text;

namespace FightCore.FrameData.Models
{
    public enum MoveType
    {
        Unknown = 0,
        Tilt = 1,
        Grounded = 2,
        Air = 3,
        Special = 4,
        Dodge = 5,
        Throw = 6
    }
}
