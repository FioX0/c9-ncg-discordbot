using C9_NCG_DiscordBot.blockchain;
using C9_NCG_DiscordBot.Handlers;
using C9_NCG_DiscordBot.Models;
using CsvHelper;
using Dropbox.Api;
using Dropbox.Api.Files;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static C9_NCG_DiscordBot.Enums.TipEnums;
using static Google.Apis.Drive.v3.DriveService;

namespace C9_NCG_DiscordBot
{

    class extras
    {

        public static List<ulong> ReadMessages = new List<ulong>();
        public sealed class DiscordActivity
        {
            public string Name { get; internal set; }
        }

        public static async Task<bool> GetMeMyShit(string content)
        {
            var client = new RestClient("https://9c-main-full-state.planetarium.dev/graphql");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\"query\":\"query {\\r\\n  stateQuery {\\r\\n    shop {\\r\\n      products(itemSubType:" + content + ") { \\r\\n        productId\\r\\n        price\\r\\n        sellerAgentAddress\\r\\n        sellerAvatarAddress\\r\\n        itemUsable {\\r\\n          id\\r\\n          itemId\\r\\n          grade\\r\\n          elementalType\\r\\n          itemType\\r\\n          itemSubType\\r\\n        }\\r\\n        costume {\\r\\n          id\\r\\n          itemId\\r\\n          grade\\r\\n          elementalType\\r\\n          itemType\\r\\n          itemSubType\\r\\n        }\\r\\n      }\\r\\n    }\\r\\n  }\\r\\n}\",\"variables\":{}}",
                       ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
            var resultObjects = AllChildren(JObject.Parse(response.Content))
               .First(c => c.Type == JTokenType.Array && c.Path.Contains("products"))
               .Children<JObject>();

            List<ItemModel> ItemList = new List<ItemModel>();

            if (content == "TITLE" || content == "FULL_COSTUME")
            {

                foreach (JObject result in resultObjects)
                {
                    ItemModel item = new ItemModel();
                    item.productId = (string)result["productId"];
                    item.price = (string)result["price"];
                    item.sellerAgentAddresss = (string)result["sellerAgentAddress"];
                    item.sellerAvatarAddress = (string)result["sellerAvatarAddress"];
                    item.itemusableid = (string)result["costume"]["id"];
                    // item.itemusablecustomid = (string)result["costume"]["id"];
                    item.itemusableitemid = (string)result["costume"]["itemId"];
                    item.itemusablegrade = (string)result["costume"]["grade"];
                    item.itemusableelementaltype = (string)result["costume"]["elementalType"];
                    item.itemusableItemSubType = (string)result["costume"]["itemSubType"];
                    item.itemusableitemType = (string)result["costume"]["itemType"];

                    ItemList.Add(item);
                }

                using (var writer = new StreamWriter("C:/Report/reportplace/" + content + ".csv"))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(ItemList);
                }
                return true;

            }
            else
            {
                foreach (JObject result in resultObjects)
                {
                    ItemModel item = new ItemModel();
                    item.productId = (string)result["productId"];
                    item.price = (string)result["price"];
                    item.sellerAgentAddresss = (string)result["sellerAgentAddress"];
                    item.sellerAvatarAddress = (string)result["sellerAvatarAddress"];
                    item.itemusableid = (string)result["itemUsable"]["id"];
                    // item.itemusablecustomid = (string)result["costume"]["id"];
                    item.itemusableitemid = (string)result["itemUsable"]["itemId"];
                    item.itemusablegrade = (string)result["itemUsable"]["grade"];
                    item.itemusableelementaltype = (string)result["itemUsable"]["elementalType"];
                    item.itemusableItemSubType = (string)result["itemUsable"]["itemSubType"];
                    item.itemusableitemType = (string)result["itemUsable"]["itemType"];

                    ItemList.Add(item);
                }

                using (var writer = new StreamWriter("C:/Report/reportplace/" + content + ".csv"))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(ItemList);
                }

                return true;
            }
        }

        private static IEnumerable<JToken> AllChildren(JToken json)
        {
            foreach (var c in json.Children())
            {
                yield return c;
                foreach (var cc in AllChildren(c))
                {
                    yield return cc;
                }
            }
        }

        public static void RunChain()
        {
            while (true)
            {
                Process[] processes = Process.GetProcessesByName("NineChronicles.Headless.Executable");
                if (processes.Length == 0)
                {
                    Console.WriteLine("Chain is down");
                    System.Diagnostics.Process.Start("C:\\publish\\run.bat");
                }
                else
                {
                    Console.WriteLine("Running");
                }
                Thread.Sleep(320000);
            }
        }

        public static async Task BackupDBAsync()
        {
            Console.WriteLine("DB Backup System Online");
            while (true)
            {
                var sw1 = Stopwatch.StartNew();
                for (int ix = 0; ix < 15; ++ix) Thread.Sleep(60000);
                sw1.Stop();

                // Get the object used to communicate with the server.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://ftp.byethost11.com");
                request.Method = WebRequestMethods.Ftp.UploadFile;

                // This example assumes the FTP site uses anonymous logon.
                using (var client = new WebClient())
                {
                    client.Credentials = new NetworkCredential("b11_29437872", "ncgmpassword");
                    client.UploadFile("ftp://ftpupload.net/htdocs/backup/database.db", "database.db");
                }
            }
        }

        public static void Miningwhitelist(DiscordClient ctx)
        {
            ctx.MessageCreated += MessageCreatedHandler;
        }

        private static Task MessageCreatedHandler(DiscordClient sender, MessageCreateEventArgs e)
        {
            try
            {
                int approved = 0;
                var pass = DiscordEmoji.FromName(sender, ":donggeul_pat_01:");
                var fail = DiscordEmoji.FromName(sender, ":donggeul_04:");

                var memberid = e.Guild.GetMemberAsync(e.Message.Author.Id);
                var message = e.Message;
                if (!message.Content.Contains("0x") || message.ChannelId != 880743387916009522)
                {
                    //not valid
                }
                else
                {
                    var joindate = memberid.Result.JoinedAt;
                    var today = DateTime.Now;
                    //how long in days the user joined.
                    var howlong = (today - joindate).TotalDays;

                    var result = Blocks.Checklevel(e.Message.Content.ToString());
                    var result2 = Blocks.CheckMonster(e.Message.Content.ToString());

                    if (result == 1 && result2 == 1)
                        message.CreateReactionAsync(pass).ConfigureAwait(false);
                    else
                        message.CreateReactionAsync(fail).ConfigureAwait(false);



                    return null;
                }

                return null;
            }
            catch (Exception ex) 
            { 
                Console.WriteLine("Message got deleted");
                var fail = DiscordEmoji.FromName(sender, ":donggeul_04:");
                e.Message.CreateReactionAsync(fail).ConfigureAwait(false);
                return null; 
            }
        }

        public static async void UpdateStatusAsync(DiscordClient ctx)
        {
            var client = new RestClient("https://api.coingecko.com/api/v3/coins/wrapped-ncg");
            int warn = 1;
            int eventfinish = 1;
            int eventlastday = 1;

            while (true)
            {
                DiscordChannel guildChannel = await ctx.GetChannelAsync(856196469727035432);

                try
                {
                    Thread.Sleep(10000);
                    client.Timeout = -1;
                    var request = new RestRequest(Method.GET);
                    IRestResponse response = client.Execute(request);

                    JObject joResponse = JObject.Parse(response.Content);

                    JValue marketdata = (JValue)joResponse["market_data"]["current_price"]["usd"];
                    string price = marketdata.ToString();

                    DSharpPlus.Entities.DiscordActivity activity = new DSharpPlus.Entities.DiscordActivity();
                    activity.Name = "WNCG: " + marketdata + "$";
                    await ctx.UpdateStatusAsync(activity);

                    Thread.Sleep(10000);
                    float topblock = GetLatestBlock();
                    activity.Name = "Top Block: " + topblock;
                    await ctx.UpdateStatusAsync(activity);

                    SqliteDataAccess db = new SqliteDataAccess();
                    float arenafinish = db.ArenaBlock();

                    Thread.Sleep(10000);
                    //check if arena is gone
                    if (arenafinish < topblock)
                    {
                        await db.UpdateArena(arenafinish + 56000);
                        warn = 1;
                        eventfinish = 1;
                        eventlastday = 1;
                    }
                    else
                    {
                        float arenaignore = db.ArenaBlockIgnore();

                        if (arenaignore < topblock && warn == 0)
                        {
                            await ctx.SendMessageAsync(guildChannel, "**WARNING** Arena Ticket have been refreshed. <@&885844994156560435>");
                            warn = 1;
                        }

                            int blockdiff = (int)(Math.Round(arenafinish) - (int)Math.Round(topblock));
                        int remainder, quotient = Math.DivRem(blockdiff, 2000, out remainder);
                        //Console.WriteLine(remainder);
                        int time = (remainder * 11) / 60;
                        float left = time / 60;
                        string str = TimeSpan.FromMinutes(time).ToString();
                        activity.Name = "Arena Timer: " + str;
                        await ctx.UpdateStatusAsync(activity);

                        Console.WriteLine(left);
                        if(arenafinish - topblock < 100 && eventfinish == 1)
                        {
                            int eventleft = (int)(Math.Round(arenafinish) - Math.Round(topblock));
                            eventleft = (eventleft * 11) / 60;
                            await ctx.SendMessageAsync(guildChannel, "**WARNING** Arena Week finishing in aproximately " + eventleft + " minutes. <@&885844994156560435>");
                            eventfinish = 0;
                        }
                        else if (arenafinish - topblock < 12000 && eventlastday == 1)
                        {
                            int eventleft = (int)(Math.Round(arenafinish) - Math.Round(topblock));
                            eventleft = (eventleft * 11) / 60;
                            eventleft = eventleft / 60;
                            await ctx.SendMessageAsync(guildChannel, "**WARNING** Arena Week finishing in aproximately " + eventleft + " hours. <@&885844994156560435>");

                            eventlastday = 0;
                        }
                        else if (left < 1 && warn == 1)
                        {
                            await ctx.SendMessageAsync(guildChannel, "**WARNING** Arena Ticket refresh in around " + time + " minutes. <@&885844994156560435>");
                            var ignore = topblock + remainder;
                            await db.UpdateArenaIgnore(ignore);
                            warn = 0;
                        }
                    }
                    Thread.Sleep(10000);
                }
                catch(Exception ex) { };
            }
        }

        public static float GetLatestBlock()
        {
            var client = new RestClient("https://9c-main-full-state.planetarium.dev/graphql");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\"query\":\"query{nodeStatus{topmostBlocks(limit: 1){index}}}\",\"variables\":{}}",
                       ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            JObject joResponse = JObject.Parse(response.Content);

            JArray marketdata = (JArray)joResponse["data"]["nodeStatus"]["topmostBlocks"];
            JValue block = (JValue)marketdata[0]["index"];
            float topblock = float.Parse(block.ToString());
            return topblock;

        }

        //public static async Task PvpExploitFinderAsync(DiscordClient ctx)
        //{

        //    DiscordChannel guildChannel = await ctx.GetChannelAsync(856196469727035432);

        //    //string thh = "0xD31B391b156A4093310efdecFc8f04417e153c58";

        //    string bluewrath = "0x7F9EB9E3A73a6F242cA4Aa2329e4F1cc8263417B";

        //    int alarm = 1;

        //    while (true)
        //    {
        //        Thread.Sleep(90000);
        //        List<JObject> BabinEquipList = await GetData();

        //        for (int i = 0; i < BabinEquipList.Count - 1; i++)
        //        {
        //            Console.WriteLine(BabinEquipList[i]["itemId"]);
        //            var array = (JArray)BabinEquipList[i]["transactions"];
        //            var signer = (JValue)array[0]["signer"];
        //            var actionarray = (JArray)array[0]["actions"];
        //            var text = actionarray.ToString();

        //            if(text.Contains("ranking_battle6"))
        //            {
        //                Console.WriteLine("Exploiter Found");
        //                ctx.SendMessageAsync(guildChannel, "Exploiter Found with Public Key: " + signer.ToString());
        //            }
        //            else
        //            {
        //                Console.WriteLine(text);
        //            }
                    
        //            //if (itemid == babinarmor && alarm == 1)
        //            //{
        //            //    Console.WriteLine("It is the expected Armor");
        //            //    //Is it equipped?
        //            //    string equipped = (string)(JValue)BabinEquipList[i]["equipped"];
        //            //    if (equipped != "True")
        //            //    {
        //            //        Console.WriteLine("Busted");
        //            //        ctx.SendMessageAsync(guildChannel, "SECRETUSER Expected Armor Missing, take a screenshot. Character likely naked. <@145915312343220224> <@256686873764823041>");
        //            //        alarm = 0;
        //            //    }
        //            //    else
        //            //        Console.WriteLine("Armor is equipped as normal :(");
        //            //}
        //        }
        //    }
        //}

        //public static async Task PvpExploitFinderAsync2(DiscordClient ctx)
        //{

        //    DiscordChannel guildChannel = await ctx.GetChannelAsync(856196469727035432);

        //    //string thh = "0xD31B391b156A4093310efdecFc8f04417e153c58";

        //    string bluetooth = "0xb4F04A910b7e404b504eEA9eDf8C9a0F63aDDbba";
        //    string babinarmor = "e6718494-f788-45ee-9730-7b334250ef62";
        //    string babinbelt = "3107f7b7-7ed6-47e2-8421-d89beab83dfc";
        //    string babinneck = "d3ecb37a-04d5-490c-9227-32355b9a34c8";
        //    string babinwep = "a6bfa4d2-08f0-4963-b3c5-571462ee6ce8";
        //    int alarm = 1;

        //    while (true)
        //    {
        //        Thread.Sleep(30000);
        //        List<JObject> BabinEquipList = await GetData(bluetooth);

        //        if (alarm == 0)
        //        {
        //            Thread.Sleep(240000);
        //            alarm = 1;
        //        }

        //        for (int i = 0; i < BabinEquipList.Count - 1; i++)
        //        {
        //            Console.WriteLine(BabinEquipList[i]["itemId"]);
        //            string itemid = (string)(JValue)BabinEquipList[i]["itemId"];
        //            if (itemid == babinarmor && alarm == 1)
        //            {
        //                Console.WriteLine("It is the expected Armor");
        //                //Is it equipped?
        //                string equipped = (string)(JValue)BabinEquipList[i]["equipped"];
        //                if (equipped != "True")
        //                {
        //                    Console.WriteLine("Busted");
        //                    ctx.SendMessageAsync(guildChannel, "SECRETUSER-ALT Expected Armor Missing, take a screenshot. Character likely naked.");
        //                    alarm = 0;
        //                }
        //                else
        //                    Console.WriteLine("Armor is equipped as normal :(");
        //            }

        //            if (itemid == babinbelt && alarm == 1)
        //            {
        //                Console.WriteLine("It is the expected Belt");
        //                //Is it equipped?
        //                string equipped = (string)(JValue)BabinEquipList[i]["equipped"];
        //                if (equipped != "True")
        //                {
        //                    Console.WriteLine("Busted");
        //                    ctx.SendMessageAsync(guildChannel, "SECRETUSER-ALT Expected Belt Missing, take a screenshot. Character likely naked.");
        //                    alarm = 0;
        //                }
        //                else
        //                    Console.WriteLine("Belt is equipped as normal :(");
        //            }

        //            if (itemid == babinneck && alarm == 1)
        //            {
        //                Console.WriteLine("It is the expected Neck");
        //                //Is it equipped?
        //                string equipped = (string)(JValue)BabinEquipList[i]["equipped"];
        //                if (equipped != "True")
        //                {
        //                    Console.WriteLine("Busted");
        //                    ctx.SendMessageAsync(guildChannel, "SECRETUSER-ALT Expected Neck Missing, take a screenshot. Character likely naked.");
        //                    alarm = 0;
        //                }
        //                else
        //                    Console.WriteLine("Neck is equipped as normal :(");
        //            }

        //            if (itemid == babinwep && alarm == 1)
        //            {
        //                Console.WriteLine("It is the expected Wep");
        //                //Is it equipped?
        //                string equipped = (string)(JValue)BabinEquipList[i]["equipped"];
        //                if (equipped != "True")
        //                {
        //                    Console.WriteLine("Busted");
        //                    ctx.SendMessageAsync(guildChannel, "SECRETUSER-ALT Expected Wep Missing, take a screenshot. Character likely naked.");
        //                    alarm = 0;
        //                }
        //                else
        //                    Console.WriteLine("Wep is equipped as normal :(");
        //            }
        //        }
        //    }
        //}

        //public static async Task PvpExploitFinderAsync3(DiscordClient ctx)
        //{

        //    DiscordChannel guildChannel = await ctx.GetChannelAsync(856196469727035432);

        //    //string thh = "0xD31B391b156A4093310efdecFc8f04417e153c58";

        //    string thh = "a73d918da6b0faa5fe0cd9a474444153f13a7629";
        //    string babinarmor = "fb31977e-f756-4dde-94ba-a494eeee53f9";
        //    string babinbelt = "808fb02a-14fe-4532-b5d5-163c0035a523";
        //    string babinneck = "1996a652-3b68-4c56-b3a8-084fe481c5db";
        //    string babinwep = "161e8b95-6721-4c76-82ec-b8f89298fbd0";
        //    int alarm = 1;

        //    while (true)
        //    {
        //        Thread.Sleep(30000);
        //        List<JObject> BabinEquipList = await GetData(thh);

        //        if (alarm == 0)
        //        {
        //            Thread.Sleep(240000);
        //            alarm = 1;
        //        }

        //        for (int i = 0; i < BabinEquipList.Count - 1; i++)
        //        {
        //            Console.WriteLine(BabinEquipList[i]["itemId"]);
        //            string itemid = (string)(JValue)BabinEquipList[i]["itemId"];
        //            if (itemid == babinarmor && alarm == 1)
        //            {
        //                Console.WriteLine("It is the expected Armor");
        //                //Is it equipped?
        //                string equipped = (string)(JValue)BabinEquipList[i]["equipped"];
        //                if (equipped != "True")
        //                {
        //                    Console.WriteLine("Busted");
        //                    ctx.SendMessageAsync(guildChannel, "thh Expected Armor Missing, take a screenshot. Character likely naked.");
        //                    alarm = 0;
        //                }
        //                else
        //                    Console.WriteLine("Armor is equipped as normal :(");
        //            }

        //            if (itemid == babinbelt && alarm == 1)
        //            {
        //                Console.WriteLine("It is the expected Belt");
        //                //Is it equipped?
        //                string equipped = (string)(JValue)BabinEquipList[i]["equipped"];
        //                if (equipped != "True")
        //                {
        //                    Console.WriteLine("Busted");
        //                    ctx.SendMessageAsync(guildChannel, "thh Expected Belt Missing, take a screenshot. Character likely naked.");
        //                    alarm = 0;
        //                }
        //                else
        //                    Console.WriteLine("Belt is equipped as normal :(");
        //            }

        //            if (itemid == babinneck && alarm == 1)
        //            {
        //                Console.WriteLine("It is the expected Neck");
        //                //Is it equipped?
        //                string equipped = (string)(JValue)BabinEquipList[i]["equipped"];
        //                if (equipped != "True")
        //                {
        //                    Console.WriteLine("Busted");
        //                    ctx.SendMessageAsync(guildChannel, "thh Expected Neck Missing, take a screenshot. Character likely naked.");
        //                    alarm = 0;
        //                }
        //                else
        //                    Console.WriteLine("Neck is equipped as normal :(");
        //            }

        //            if (itemid == babinwep && alarm == 1)
        //            {
        //                Console.WriteLine("It is the expected Wep");
        //                //Is it equipped?
        //                string equipped = (string)(JValue)BabinEquipList[i]["equipped"];
        //                if (equipped != "True")
        //                {
        //                    Console.WriteLine("Busted");
        //                    ctx.SendMessageAsync(guildChannel, "thh Expected Wep Missing, take a screenshot. Character likely naked.");
        //                    alarm = 0;
        //                }
        //                else
        //                    Console.WriteLine("Wep is equipped as normal :(");
        //            }
        //        }
        //    }
        //}

        public static async Task<List<JObject>> GetData()
        {
            Console.WriteLine("Calling API");
            var client = new RestClient("https://9c-main-full-state.planetarium.dev/graphql");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            //Console.WriteLine(address);
            request.AddParameter("application/json", "{\"query\":\"query{\\r\\n  chainQuery{blockQuery{blocks(desc: true, limit: 20){index,transactions{signer,actions{inspection}}}}\\r\\n  }\\r\\n}\",\"variables\":{}}",
                                                     ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            var resultObjects = AllChildren(JObject.Parse(response.Content))
            .First(c => c.Type == JTokenType.Array && c.Path.Contains("blocks"))
            .Children<JObject>();

            List<JObject> ItemList = new List<JObject>();

            foreach (JObject result in resultObjects)
            {

                ItemList.Add(result);
            }

            return ItemList;
        }
    }
}
