using System;
using System.Collections.Generic;
using System.Text;

namespace FightCore.Models
{
    public class ServerSettings
    {
        public long Id { get; set; }

        public ulong ServerId { get; set; }

        public string Prefix { get; set; }
    }
}
