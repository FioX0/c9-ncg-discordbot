using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace C9_NCG_DiscordBot
{
    public struct ConfigJson
    {
        [JsonProperty("Token")]
        public string Token { get; private set; }
        [JsonProperty("prefix")]
        public string Prefix { get; private set; }
    }
}
