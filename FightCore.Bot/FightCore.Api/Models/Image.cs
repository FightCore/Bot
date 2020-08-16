using System;
using System.Collections.Generic;
using System.Text;

namespace FightCore.Api.Models
{
    public class Image
    {
        /// <summary>
        /// The url of the image.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The name of the image.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// An description of the image.
        /// </summary>
        public string Description { get; set; }
    }
}
