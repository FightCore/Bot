using System;
using System.Collections.Generic;
using System.Text;

namespace FightCore.Api.Models
{
    /// <summary>
    /// The ViewModel to be used to display a full character.
    /// </summary>
    public class Character
    {
        /// <summary>
        /// The id of the character.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The name of the character.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Some general information about the character.
        /// </summary>
        public string GeneralInformation { get; set; }

        /// <summary>
        /// Notable players who play this character.
        /// </summary>
        public List<NotablePlayer> NotablePlayers { get; set; }

        /// <summary>
        /// The image used as the stock icon for the character.
        /// </summary>
        public Image StockIcon { get; set; }

        /// <summary>
        /// A full body picture of the character.
        /// </summary>
        public Image CharacterImage { get; set; }
    }
}
