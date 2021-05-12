using System;
using System.Collections.Generic;
using System.Text;

namespace C9_NCG_DiscordBot.Models
{
    public class PaymentModel
    {
            public ulong Id { get; set; }
            public int ProfileId { get; set; }
            public ulong DiscordUserId { get; set; }
            public string Key { get; set; }
            public float Amount { get; set; }
            public int Authrised { get; set; }
    }
}
