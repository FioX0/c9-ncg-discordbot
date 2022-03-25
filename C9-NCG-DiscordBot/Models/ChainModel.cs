using System;
using System.Collections.Generic;
using System.Text;

namespace C9_NCG_DiscordBot.Models
{
    public class ChainModel
    {
        public string address { get; set; }
        public string active { get; set; }
        public double index { get; set; }
        public double users { get; set; }
        public int nodeid { get; set; }
        public float duration { get; set; }
    }
}
