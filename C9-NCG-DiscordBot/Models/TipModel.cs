using System;
using System.Collections.Generic;
using System.Text;

namespace C9_NCG_DiscordBot.Models
{
    public class TipModel
    {
        public ulong Id { get; set; }
        public ulong DiscordId { get; }
        public float Balance { get; set; }
        public string Role { get; set; }
    }
}
