using C9_NCG_DiscordBot.blockchain;
using C9_NCG_DiscordBot.Handlers;
using C9_NCG_DiscordBot.Models;
using CsvHelper;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static C9_NCG_DiscordBot.Enums.TipEnums;

namespace C9_NCG_DiscordBot
{
    class extras
    {
        public static async Task ShopDataAsync(DiscordClient client2)
        {
            int isittime = 0;
            while (true)
            {
                var sw1 = Stopwatch.StartNew();
                for (int ix = 0; ix < 60; ++ix) Thread.Sleep(1000);
                sw1.Stop();
                //Console.WriteLine("Sleep: {0}", sw1.ElapsedMilliseconds);
                
                string time = DateTime.UtcNow.ToString("HH");

                //Console.WriteLine(time);

                if (time == "13" && isittime == 0)
                {
                    await GetMeMyShit("WEAPON");
                    await GetMeMyShit("ARMOR");
                    await GetMeMyShit("BELT");
                    await GetMeMyShit("NECKLACE");
                    await GetMeMyShit("RING");
                    await GetMeMyShit("FOOD");
                    await GetMeMyShit("TITLE");
                    await GetMeMyShit("FULL_COSTUME");

                    string startPath = @"F:\Report\reportplace\";
                    string zipPath = @"F:\Report\result.zip";

                    if(File.Exists(zipPath))
                    {
                        File.Delete(zipPath);
                    }

                    ZipFile.CreateFromDirectory(startPath, zipPath);

                    DiscordChannel channel = await client2.GetChannelAsync(331443974722027522);
                    await channel.SendFileAsync("C:/Report/result.zip", "Here's your Report");


                    isittime = 1;
                }

                if (time != "13" && isittime == 1)
                    isittime = 0;
            }
        }

        public static async Task OnDemandShopDataAsync(CommandContext ctx)
        {
            await GetMeMyShit("WEAPON");
            await GetMeMyShit("ARMOR");
            await GetMeMyShit("BELT");
            await GetMeMyShit("NECKLACE");
            await GetMeMyShit("RING");
            await GetMeMyShit("FOOD");
            await GetMeMyShit("TITLE");
            await GetMeMyShit("FULL_COSTUME");

            string startPath = @"C:\Report\reportplace\";
            string zipPath = @"C:\Report\result.zip";

            if (File.Exists(zipPath))
            {
                File.Delete(zipPath);
            }

            ZipFile.CreateFromDirectory(startPath, zipPath);

            await ctx.Channel.SendFileAsync("C:/Report/result.zip", "Here's your Report");
        }

        public static async Task DailyBlockReport(DiscordClient client2)
        {
            Console.WriteLine("DailyBlockEnabled");
            int howfar = 8640;
            int isittime = 0;
            while (true)
            {
                var sw1 = Stopwatch.StartNew();
                for (int ix = 0; ix < 60; ++ix) Thread.Sleep(1000);
                sw1.Stop();
                //Console.WriteLine("Sleep: {0}", sw1.ElapsedMilliseconds);
                string time = DateTime.Now.ToString("HH");

                //Console.WriteLine(time);

                if (time == "00" && isittime == 0)
                {
                    if (howfar > 0)
                    {

                        SqliteDataAccess sqli = new SqliteDataAccess();
                        var blocks = new Blocks();
                        var ncg = new NCG();
                        var comms = new Communication();
                        await SqliteDataAccess.Deleteblockreport();
                        var result = await blocks.CustomDayReport(howfar);

                            BlockReportModel[] dataSet = await SqliteDataAccess.GetBlockReportData();
                            using (var writer = new StreamWriter("C:/Release/Report/FullDayBlocks.csv"))
                            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                            {
                                csv.WriteRecords(dataSet);
                            }
                        DiscordChannel channel = await client2.GetChannelAsync(856196469727035432);
                        DiscordChannel channel2 = await client2.GetChannelAsync(826783795842777102);
                        await channel.SendFileAsync("C:/Release/Report/FullDayBlocks.csv", "Daily Reset Full Block Report");
                        await channel2.SendFileAsync("C:/Release/Report/FullDayBlocks.csv", "Daily Reset Full Block Report");
                    }
                    isittime = 1;
                }

                if (time != "00" && isittime == 1)
                    isittime = 0;
            }
        }

        public static async Task<bool> GetMeMyShit(string content)
        {
            var client = new RestClient("https://9c-main-full-state.planetarium.dev/graphql");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\"query\":\"query {\\r\\n  stateQuery {\\r\\n    shop {\\r\\n      products(itemSubType:" +content+") { \\r\\n        productId\\r\\n        price\\r\\n        sellerAgentAddress\\r\\n        sellerAvatarAddress\\r\\n        itemUsable {\\r\\n          id\\r\\n          itemId\\r\\n          grade\\r\\n          elementalType\\r\\n          itemType\\r\\n          itemSubType\\r\\n        }\\r\\n        costume {\\r\\n          id\\r\\n          itemId\\r\\n          grade\\r\\n          elementalType\\r\\n          itemType\\r\\n          itemSubType\\r\\n        }\\r\\n      }\\r\\n    }\\r\\n  }\\r\\n}\",\"variables\":{}}",
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

        public static async Task Alarm(DiscordClient client2)
        {
            bool need = true;
            string[] addresses = { "0x583E09c9aA2d204e0089C7ccB212c76bE41D294c", "0xC8C3C19842f6ca03848c57100b2e04EB608Da9cd", "0xeC64FcE292B1a3544d5C3A070280c807FEf7A0cC", "0xe219608e22D3A9d345d1E23F3245A313f23eF164", "0x6819ee33C56584a54ae572E7d85240b30815f605", "0xbBB97256a63339144efEAe4E7e244b21632AE03e", "0x8F69d940f58783acC9ed89DF1B1fB10686d2CD2C", "0x894d673bcD5594b0AdA5CFbf03aC3b454e348fdD", "0x849219d33E2dc1429DceFfe7dB0eF40D430Cd6a2", "0x6819ee33C56584a54ae572E7d85240b30815f605", "0xC8C3C19842f6ca03848c57100b2e04EB608Da9cd", "0x583E09c9aA2d204e0089C7ccB212c76bE41D294c"};
            Communication comms = new Communication();
            while (need)
            {
                var sw1 = Stopwatch.StartNew();
                for (int ix = 0; ix < 60; ++ix) Thread.Sleep(1000);
                sw1.Stop();
                foreach (string address in addresses)
                {
                    Console.WriteLine(address);
                    int state = await AlarmReport(address);
                    if (state == 1)
                    {
                        await comms.WarningTransfer(client2, address);
                        break;
                    }
                    Console.WriteLine("next");
                }
            }
        }

        public static async Task<int> AlarmReport(string address)
        {
            var client = new RestClient("http://localhost:23061/graphql");
            client.Timeout = 200000;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\"query\":\"query{\\r\\n chainQuery{transactionQuery{transactions(desc:true signer:\\\""+address+"\\\" limit:10){signer,actions{inspection}}}}\\r\\n}\",\"variables\":{}}",
                       ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            try
            {
                var resultObjects = AllChildren(JObject.Parse(response.Content))
               .First(c => c.Type == JTokenType.Array && c.Path.Contains("transactions"))
               .Children<JObject>();

                foreach (JObject result in resultObjects)
                {
                    var text = result["actions"];
                    string tstring = text.ToString();
                    if (tstring.Contains("transfer_asset", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("yes");
                        return 1;
                    }
                }
            }
            catch { return 0; }
            return 0;
        }
    }
}
