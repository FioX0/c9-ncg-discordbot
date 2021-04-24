using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace C9_NCG_DiscordBot.Models
{
    public class ProfileModel
    {
        public ulong Id { get; set; }
        public string PublicKey { get; set; }
        public float Value { get; set; }
        public string alias { get; set; }
    }
}
