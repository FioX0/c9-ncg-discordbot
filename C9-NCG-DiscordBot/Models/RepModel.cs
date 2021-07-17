using System;
using System.Collections.Generic;
using System.Text;

namespace C9_NCG_DiscordBot.Models
{
    public class RepModel
    {
        public ulong Id { get; set; }
        public ulong UserId { get; set; }
        public string PublicKey { get; set; }
        public float Transactions { get; set; }
        public float ComRep { get; set; }
        public float TradeRep { get; set; }
    }
}
