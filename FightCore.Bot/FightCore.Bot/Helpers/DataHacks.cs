using System;
using System.Collections.Generic;
using System.Text;

namespace FightCore.Bot.Helpers
{
    public static class DataHacks
    {
        /// <summary>
        /// Fixes the throws that are noted as "direction_throw" to be in the same format
        /// as the other normalized resources.
        /// </summary>
        /// <param name="oldThrow">The old value to normalize.</param>
        /// <returns>The new normalized value.</returns>
        public static string FixThrowType(string oldThrow)
        {
            switch (oldThrow)
            {
                case "forward_throw":
                    return "fthrow";
                case "back_throw":
                    return "bthrow";
                case "up_throw":
                    return "uthrow";
                case "down_throw":
                    return "dthrow";
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
