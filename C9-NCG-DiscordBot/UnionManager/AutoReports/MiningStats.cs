using C9_NCG_DiscordBot.Handlers;
using C9_NCG_DiscordBot.Models;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace C9_NCG_DiscordBot.UnionManager.AutoReports
{
    public class MiningStats
    {
        public static async Task InformMiningStatus(DiscordClient client2)
        {
            var db = new SqliteDataAccess();
            while (true)
            {
                var sw1 = Stopwatch.StartNew();
                for (int ix = 0; ix < 60; ++ix) Thread.Sleep(1000);
                sw1.Stop();
                //grab Union Miners
                ProfileModel[] array = await db.LoadALLUnionMiners();
                foreach (var miner in array)
                {
                    
                }
            }
        }
    }
}
