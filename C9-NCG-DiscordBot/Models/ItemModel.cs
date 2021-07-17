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
        [JsonProperty("price")]
        public string price { get; set; }
        [JsonProperty("sellerAgentAddresss")]
        public string sellerAgentAddresss { get; set; }

        [JsonProperty("sellerAvatarAddress")]
        public string sellerAvatarAddress { get; set; }

        [JsonProperty("itemUsable/Id")]
        public string itemusableid { get; set; }

        [JsonProperty("itemUsable/itemId")]
        public string itemusableitemid { get; set; }

        [JsonProperty("itemUsable/grade")]
        public string itemusablegrade { get; set; }

        [JsonProperty("itemUsable/elementalType")]
        public string itemusableelementaltype { get; set; }

        [JsonProperty("itemusable/ItemType")]
        public string itemusableitemType { get; set; }

        [JsonProperty("itemUsable/ItemSubType")]
        public string itemusableItemSubType { get; set; }

    }
}
