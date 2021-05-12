using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace C9_NCG_DiscordBot.Models
{
    public class ItemModel
    {
        [JsonProperty("productId")]
        public string productId { get; set; }
        [JsonProperty("\"price\"")]
        public string price { get; set; }

    }
}
