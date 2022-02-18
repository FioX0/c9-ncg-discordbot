using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace C9_NCG_DiscordBot.Models
{
    public class WncgModel
    {
        [JsonProperty("hash")]
        public string hash { get; set; }
       
    }
}
