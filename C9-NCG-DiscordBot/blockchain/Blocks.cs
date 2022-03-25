using C9_NCG_DiscordBot.Models;
using DSharpPlus.CommandsNext;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace C9_NCG_DiscordBot.blockchain
{
    public class Blocks
    {
        private static string MainObject;
        public int BlockReportYesterday(string key)
        {
            string todayDate = DateTime.Now.ToString("MM/dd/yyyy");
            int todayMonth = int.Parse(todayDate.Substring(0, 2));
            int year = int.Parse(todayDate.Substring(6, 4));
            int todayDay = int.Parse(todayDate.Substring(3, 2));
            int yesterDay = todayDay - 1;
            int stopday = yesterDay - 1;
            int i = 0;
            int y = 0;
            bool required = true;

            //OH SHIT SCENARIO
            int endMonth = todayMonth;
            int endTotalDays = DateTime.DaysInMonth(year, endMonth-1);
            int track = yesterDay;
            //OH SHIT END

            if (stopday <= 0 && yesterDay > 0)
            {
                endMonth -= 1;
                stopday = endTotalDays;
               
            }
            else if(stopday <= 0 && yesterDay <= 0)
            {
                yesterDay = endTotalDays - 1;
                stopday = yesterDay - 1;
                todayMonth -= 1;
                endMonth -= 1;
            }

            try
            {
                Console.WriteLine("Calling API");
                while (required)
                {
                    Console.WriteLine("Getting data");
                    //We got response, now let's parse it.
                    var response = GetMeBlockData(y,key);

                    if (response == null)
                    {
                        return 99999;
                    }

                    JObject joResponse = JObject.Parse(response.Content);


                    var resultObjects = AllChildren(JObject.Parse(response.Content))
                       .First(c => c.Type == JTokenType.Array && c.Path.Contains("data"))
                       .Children<JObject>();

                    // JArray jAddresses = (JArray)ojObject["chainQuery"]["blockQuery"]["blocks"];
                    //Console.WriteLine(jAddresses);


                    foreach (JObject result in resultObjects)
                    {
                        JValue equipped = (JValue)result["timestamp"];
                        string blockdate = (string)equipped;
                        int blockday = int.Parse(blockdate.Substring(3, 2));
                        int blockmonth = int.Parse(blockdate.Substring(0, 2));

                        Console.WriteLine("blockmonth: " + blockmonth);
                        Console.WriteLine("todayMonth: " + todayMonth);
                        Console.WriteLine("blockday: " + blockday);
                        Console.WriteLine("yesterDay: " + yesterDay);
                        Console.WriteLine("blockday: " + blockday);
                        Console.WriteLine("stopday: " + stopday);
                        Console.WriteLine("endMonth: " + endMonth);

                        if (blockmonth == todayMonth && blockmonth != endMonth)
                        {
                            if (blockday <= yesterDay)
                                i++;

                            if (blockday <= stopday && blockmonth == endMonth)
                                required = false;
                        }
                        else if (blockmonth == todayMonth)
                        {
                            if (blockday <= yesterDay && blockday > stopday)
                                i++;

                            if (blockday <= stopday && blockmonth == endMonth)
                                required = false;
                        }
                        else if (blockmonth == endMonth)
                        {
                            if (blockday <= endTotalDays && blockday > stopday)
                                i++;

                            if (blockday <= stopday && blockmonth == endMonth)
                                required = false;
                        }
                        Console.WriteLine(blockdate);
                    }
                    //Console.WriteLine(todayDate);
                    //Console.WriteLine(todayDay);
                    //Console.WriteLine(yesterDay);
                    Console.WriteLine(i);
                    //0 is sender // 1 is receiver.
                    y += 50;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return i;
        }

        public async Task<bool> DayReportFullBlock()
        {
            string todayDate = DateTime.Now.ToString("MM/dd/yyyy");
            int todayMonth = int.Parse(todayDate.Substring(0, 2));
            int year = int.Parse(todayDate.Substring(6, 4));
            int todayDay = int.Parse(todayDate.Substring(3, 2));
            int yesterDay = todayDay - 1;
            int stopday = yesterDay - 1;
            int i = 0;
            int y = 0;
            bool required = true;

            //OH SHIT SCENARIO
            int endMonth = todayMonth;
            int endTotalDays = DateTime.DaysInMonth(year, endMonth - 1);
            int track = yesterDay;
            //OH SHIT END

            if (stopday <= 0 && yesterDay > 0)
            {
                endMonth -= 1;
                stopday = endTotalDays;

            }
            else if (stopday <= 0 && yesterDay <= 0)
            {
                yesterDay = endTotalDays - 1;
                stopday = yesterDay - 1;
                todayMonth -= 1;
                endMonth -= 1;
            }

            try
            {
                Console.WriteLine("Calling API");
                while (required)
                {
                    Console.WriteLine("Getting data");
                    //We got response, now let's parse it.
                    var response = GetMeFullBlockData();

                    if (response == null)
                    {
                        return false;
                    }

                    JObject joResponse = JObject.Parse(response.Content);


                    var resultObjects = AllChildren(JObject.Parse(response.Content))
                       .First(c => c.Type == JTokenType.Array && c.Path.Contains("data"))
                       .Children<JObject>();

                    List<string> minerList = new List<string>();
                    // JArray jAddresses = (JArray)ojObject["chainQuery"]["blockQuery"]["blocks"];
                    //Console.WriteLine(jAddresses);
                    foreach (JObject miner in resultObjects)
                    {
                        JValue minerJValue = (JValue)miner["miner"];
                        string minerString = minerJValue.ToString();
                        if (!minerList.Any(s => s.StartsWith(minerString)))
                        {
                            ///Which Miner we are looking at.
                            Console.WriteLine(minerString);

                            foreach (JObject result in resultObjects)
                            {
                                JValue minerToCheck = (JValue)result["miner"];
                                if (minerString == minerToCheck.ToString())
                                {
                                    JValue equipped = (JValue)result["timestamp"];
                                    string blockdate = (string)equipped;
                                    int blockday = int.Parse(blockdate.Substring(3, 2));
                                    int blockmonth = int.Parse(blockdate.Substring(0, 2));

                                    //Console.WriteLine("blockmonth: " + blockmonth);
                                    //Console.WriteLine("todayMonth: " + todayMonth);
                                    //Console.WriteLine("blockday: " + blockday);
                                    //Console.WriteLine("yesterDay: " + yesterDay);
                                    //Console.WriteLine("blockday: " + blockday);
                                    //Console.WriteLine("stopday: " + stopday);
                                    //Console.WriteLine("endMonth: " + endMonth);

                                    if (blockmonth == todayMonth && blockmonth != endMonth)
                                    {
                                        if (blockday <= yesterDay)
                                            i++;

                                        if (blockday <= stopday && blockmonth == endMonth)
                                            break;
                                    }
                                    else if (blockmonth == todayMonth)
                                    {
                                        if (blockday <= yesterDay && blockday > stopday)
                                            i++;

                                        if (blockday <= stopday && blockmonth == endMonth)
                                            break;
                                    }
                                    else if (blockmonth == endMonth)
                                    {
                                        if (blockday <= endTotalDays && blockday > stopday)
                                            i++;

                                        if (blockday <= stopday && blockmonth == endMonth)
                                            break;
                                    }
                                    //Console.WriteLine(blockdate);
                                }
                            }
                            Console.WriteLine("For Miner:" + minerString + " we found " + i + " blocks");
                            string minerResult = minerString + " - " + i;
                            minerList.Add(minerResult);
                            await SqliteDataAccess.blockreport(minerString, i);
                            i = 0;
                        }
                    }
                    required = false;
                }
                int total = await SqliteDataAccess.GetBlockBlockCount();
                Console.WriteLine(total);
                BlockReportModel[] dataSet = await SqliteDataAccess.GetBlockReportData();
                foreach(BlockReportModel set in dataSet)
                {
                    decimal calc = 0;
                    decimal blocks = set.Blocks;
                    decimal totaldec = total;
                    calc = (blocks / totaldec) * 100;
                    set.Control = calc.ToString()+"%";
                    Console.WriteLine(set.Control);
                    bool state = await SqliteDataAccess.UpdateBlockBlockCount(set.PublicKey, set.Control);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        public async Task<bool> CustomDayReport(int howfar)
        {
            int i = 0;
            int y = 0;
            bool required = true;

            try
            {
                Console.WriteLine("Calling API");
                while (required)
                {
                    Console.WriteLine("Getting data");
                    //We got response, now let's parse it.
                    var response = GetMeCustomBlockData(howfar);

                    if (response == null)
                    {
                        return false;
                    }

                    JObject joResponse = JObject.Parse(response.Content);


                    var resultObjects = AllChildren(JObject.Parse(response.Content))
                       .First(c => c.Type == JTokenType.Array && c.Path.Contains("data"))
                       .Children<JObject>();

                    List<string> minerList = new List<string>();
                    // JArray jAddresses = (JArray)ojObject["chainQuery"]["blockQuery"]["blocks"];
                    //Console.WriteLine(jAddresses);
                    foreach (JObject miner in resultObjects)
                    {
                        JValue minerJValue = (JValue)miner["miner"];
                        string minerString = minerJValue.ToString();
                        if (!minerList.Any(s => s.StartsWith(minerString)))
                        {
                            ///Which Miner we are looking at.
                            Console.WriteLine(minerString);

                            foreach (JObject result in resultObjects)
                            {
                                JValue minerToCheck = (JValue)result["miner"];
                                if (minerString == minerToCheck.ToString())
                                {
                                    i++;
                                }
                            }
                            Console.WriteLine("For Miner:" + minerString + " we found " + i + " blocks");
                            string minerResult = minerString + " - " + i;
                            minerList.Add(minerResult);
                            await SqliteDataAccess.blockreport(minerString, i);
                            i = 0;
                        }
                    }
                    required = false;
                }
                int total = await SqliteDataAccess.GetBlockBlockCount();
                Console.WriteLine(total);
                BlockReportModel[] dataSet = await SqliteDataAccess.GetBlockReportData();
                foreach (BlockReportModel set in dataSet)
                {
                    decimal calc = 0;
                    decimal blocks = set.Blocks;
                    decimal totaldec = total;
                    calc = (blocks / totaldec) * 100;
                    set.Control = calc.ToString() + "%";
                    Console.WriteLine(set.Control);
                    bool state = await SqliteDataAccess.UpdateBlockBlockCount(set.PublicKey, set.Control);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        public int BlockReportWeek(string key)
        {
            string todayDate = DateTime.Now.ToString("MM/dd/yyyy");
            int todayMonth = int.Parse(todayDate.Substring(0, 2));
            int year = int.Parse(todayDate.Substring(6, 4));
            int todayDay = int.Parse(todayDate.Substring(3, 2));
            int yesterDay = todayDay - 1;
            int stopday = yesterDay - 7;
            int i = 0;
            int y = 0;
            int z = 0;
            bool required = true;

            //OH SHIT SCENARIO
            int endMonth = todayMonth;
            int endTotalDays = DateTime.DaysInMonth(year, endMonth);
            int track = yesterDay;
            //OH SHIT END


            //int todayDay = int.Parse(todayDate.Substring(3, 2));



            if (stopday <= 0)
            {
                endMonth = todayMonth - 1;
                endTotalDays = DateTime.DaysInMonth(year, endMonth);
                //Oh fuck we need to consider moving across months FML.
                //let's see how many days we can get before we need to swap months.
                for (int x = 0; yesterDay > 0;x++)
                {
                    yesterDay--;
                    Console.WriteLine(yesterDay);
                    z++;
                }
                stopday = endTotalDays - (7 - z);
                yesterDay = track;
                Console.WriteLine(stopday);
                Console.WriteLine(yesterDay);
            }

            try
            {
                Console.WriteLine("Calling API");
                while (required)
                {
                    Console.WriteLine("Getting data");
                    //We got response, now let's parse it.
                    var response = GetMeBlockData(y, key);

                    if(response== null)
                    {
                        return 99999;
                    }
           
                    JObject joResponse = JObject.Parse(response.Content);

                    var resultObjects = AllChildren(JObject.Parse(response.Content))
                       .First(c => c.Type == JTokenType.Array && c.Path.Contains("data"))
                       .Children<JObject>();

                    // JArray jAddresses = (JArray)ojObject["chainQuery"]["blockQuery"]["blocks"];
                    //Console.WriteLine(jAddresses);

                    var Entry = new List();

                    foreach (JObject result in resultObjects)
                    {
                        JValue equipped = (JValue)result["timestamp"];
                        string blockdate = (string)equipped;
                        int blockday = int.Parse(blockdate.Substring(3, 2));
                        int blockmonth = int.Parse(blockdate.Substring(0, 2));


                        Console.WriteLine("blockmonth: " + blockmonth);
                        Console.WriteLine("todayMonth: " + todayMonth);
                        Console.WriteLine("blockday: " + blockday);
                        Console.WriteLine("yesterDay: " + yesterDay);
                        Console.WriteLine("blockday: " + blockday);
                        Console.WriteLine("stopday: " + stopday);

                        if (blockmonth == todayMonth)
                        {
                            if (blockday <= yesterDay)
                                i++;

                            if (blockday <= stopday && blockmonth == endMonth)
                                required = false;
                        }
                        else if(blockmonth == endMonth)
                        {
                            if (blockday <= endTotalDays && blockday > stopday)
                                i++;

                            if (blockday <= stopday && blockmonth == endMonth)
                                required = false;

                        }
                        Console.WriteLine(blockdate);
                    }
                    //Console.WriteLine(todayDate);
                    //Console.WriteLine(todayDay);
                    //Console.WriteLine(yesterDay);
                    Console.WriteLine(i);
                    //0 is sender // 1 is receiver.
                    y += 50;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return i;
        }

        public int BlockReportMonth(string key)
        {
            string todayDate = DateTime.Now.ToString("MM/dd/yyyy");
            int todayMonth = int.Parse(todayDate.Substring(0, 2));
            int year = int.Parse(todayDate.Substring(6, 4));
            int todayDay = int.Parse(todayDate.Substring(3, 2));
            int yesterDay = todayDay - 1;
            int stopday = yesterDay - 30;
            int i = 0;
            int y = 0;
            int z = 0;
            bool required = true;

            //OH SHIT SCENARIO
            int endMonth = todayMonth;
            int endTotalDays = DateTime.DaysInMonth(year, endMonth);
            int track = yesterDay;
            //OH SHIT END


            //int todayDay = int.Parse(todayDate.Substring(3, 2));



            if (stopday <= 0)
            {
                endMonth = todayMonth - 1;
                endTotalDays = DateTime.DaysInMonth(year, endMonth);
                //Oh fuck we need to consider moving across months FML.
                //let's see how many days we can get before we need to swap months.
                for (int x = 0; yesterDay > 0; x++)
                {
                    yesterDay--;
                    Console.WriteLine(yesterDay);
                    z++;
                }
                stopday = endTotalDays - (30 - z);
                yesterDay = track;
                Console.WriteLine(stopday);
                Console.WriteLine(yesterDay);
            }

            try
            {
                Console.WriteLine("Calling API");
                while (required)
                {
                    Console.WriteLine("Getting data");
                    //We got response, now let's parse it.
                    var response = GetMeBlockData(y,key);

                    if (response == null)
                    {
                        return 99999;
                    }

                    JObject joResponse = JObject.Parse(response.Content);


                    var resultObjects = AllChildren(JObject.Parse(response.Content))
                       .First(c => c.Type == JTokenType.Array && c.Path.Contains("data"))
                       .Children<JObject>();

                    // JArray jAddresses = (JArray)ojObject["chainQuery"]["blockQuery"]["blocks"];
                    //Console.WriteLine(jAddresses);

                    var Entry = new List();

                    foreach (JObject result in resultObjects)
                    {
                        JValue equipped = (JValue)result["timestamp"];
                        string blockdate = (string)equipped;
                        int blockday = int.Parse(blockdate.Substring(3, 2));
                        int blockmonth = int.Parse(blockdate.Substring(0, 2));


                        Console.WriteLine("blockmonth: " + blockmonth);
                        Console.WriteLine("todayMonth: " + todayMonth);
                        Console.WriteLine("endMonth: " + endMonth);
                        Console.WriteLine("blockday: " + blockday);
                        Console.WriteLine("yesterDay: " + yesterDay);
                        Console.WriteLine("blockday: " + blockday);
                        Console.WriteLine("stopday: " + stopday);

                        if (blockmonth == todayMonth && blockmonth != endMonth)
                        {
                            if (blockday <= yesterDay)
                                i++;

                            if (blockday <= stopday && blockmonth == endMonth)
                                required = false;
                        }
                        else if (blockmonth == endMonth)
                        {
                            if (blockday <= endTotalDays && blockday > stopday)
                                i++;

                            if (blockday <= stopday && blockmonth == endMonth)
                                required = false;

                        }
                        Console.WriteLine(blockdate);
                    }
                    //Console.WriteLine(todayDate);
                    //Console.WriteLine(todayDay);
                    //Console.WriteLine(yesterDay);
                    Console.WriteLine(i);
                    //0 is sender // 1 is receiver.
                    y += 50;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return i;
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


        private static IRestResponse GetMeBlockData(int offset, string key)
        {
            int loop = 0;
            while (loop < 10)
            {
                Console.WriteLine(key);
                var client = new RestClient("https://mars.9cscan.com/graphql");
                client.Timeout = -1;
                var request = new RestSharp.RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", "{\"query\":\"query{\\r\\nchainQuery{\\r\\n  blockQuery{\\r\\n    blocks(desc:true,offset:" + offset + ", miner:\\\"" + key + "\\\", excludeEmptyTxs:true, limit:50) {timestamp}\\r\\n  \\t}\\r\\n\\t}\\r\\n}\",\"variables\":{}}", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                if (response.StatusCode.ToString() == "520")
                {
                    loop++;
                }
                else
                {
                    return response;
                }
            }
            return null;
        }

        private static IRestResponse GetMeFullBlockData()
        {
            int loop = 0;
            while (loop < 10)
            {
                var client = new RestClient("https://mars.9cscan.com/graphql");
                client.Timeout = -1;
                var request = new RestSharp.RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", "{\"query\":\"query{\\r\\nchainQuery{\\r\\n  blockQuery{\\r\\n    blocks(desc:true,offset:0, excludeEmptyTxs:true, limit:20000) {miner,timestamp}\\r\\n          }\\r\\n        }\\r\\n}\",\"variables\":{}}", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                if (response.StatusCode.ToString() == "520")
                {
                    loop++;
                }
                else
                {
                    return response;
                }
            }
            return null;
        }

        private static IRestResponse GetMeCustomBlockData(int howfar)
        {
            int loop = 0;
            while (loop < 10)
            {
                var client = new RestClient("https://mars.9cscan.com/graphql");
                client.Timeout = -1;
                var request = new RestSharp.RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", "{\"query\":\"query{\\r\\nchainQuery{\\r\\n  blockQuery{\\r\\n    blocks(desc:true,offset:3, excludeEmptyTxs:true, limit:"+howfar+") {miner,timestamp}\\r\\n          }\\r\\n        }\\r\\n}\",\"variables\":{}}", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                if (response.StatusCode.ToString() == "520")
                {
                    loop++;
                    Console.WriteLine("We fked up");
                }
                else
                {
                    return response;
                }
            }
            return null;
        }


        public static IRestResponse GetExploiterData(string address)
        {
                var client = new RestClient("http://localhost:23061/graphql");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", "{\"query\":\"query{\\r\\nchainQuery{\\r\\n  transactionQuery{\\r\\n    transactions(signer:\\\""+address+"\\\",desc: true, offset: 0, limit: 20000){\\r\\n      signer,publicKey,updatedAddresses,actions{inspection},timestamp\\r\\n    }\\r\\n  }\\r\\n}}\",\"variables\":{}}",
                           ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

            return response;
        }

        public static async Task<bool> ExploitReport(string address, CommandContext ctx )
        {
            int amount = 0;
            int y = 0;
            bool required = true;

            try
            {
                Console.WriteLine("Calling API");

                Console.WriteLine("Getting data");
                //We got response, now let's parse it.
                var response = GetExploiterData(address);

                if (response == null)
                {
                    return false;
                }

                List<JObject> blocklist = new List<JObject>();
                List<string> ignorereceiver = new List<string>();

                try
                {
                    JObject joResponse = JObject.Parse(response.Content);

                    var resultObjects = AllChildren(JObject.Parse(response.Content))
                    .First(c => c.Type == JTokenType.Array && c.Path.Contains("transactions"))
                    .Children<JObject>();

                    foreach (JObject blocklines in resultObjects)
                    {
                        blocklist.Add(blocklines);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    await ctx.Client.SendMessageAsync(ctx.Channel, "I timed out, this was likely due to no TX's being sent from this address.");
                }

                // JArray jAddresses = (JArray)ojObject["chainQuery"]["blockQuery"]["blocks"];
                //Console.WriteLine(jAddresses);

                while (y!=20)
                {
                    foreach (JObject miner in blocklist)
                    {
                        if (MainObject == null)
                        {
                            Console.WriteLine("MainObject is empty");
                            var transfercheck = miner.ToString().Contains("transfer_asset");
                            if (transfercheck)
                            {
                                JArray minerJValue = (JArray)miner["updatedAddresses"];
                                JValue signer = (JValue)miner["signer"];
                                Console.WriteLine(signer.ToString());

                                if(signer.ToString() == minerJValue[0].ToString())
                                {
                                    if (!ignorereceiver.Contains(minerJValue[1].ToString()))
                                    {
                                        MainObject = minerJValue[1].ToString();
                                    }
                                    else
                                    {
                                        Console.WriteLine("This Address already in ignore list");
                                    }
                                }
                                else
                                {
                                    if (!ignorereceiver.Contains(minerJValue[0].ToString()))
                                    {
                                        MainObject = minerJValue[0].ToString();
                                    }
                                    else
                                    {
                                        Console.WriteLine("This Address already in ignore list");
                                    }
                                }


                            }
                        }
                        else
                        {
                            //let's find for miners inside that match MainObject;
                            // minerJValue[0] receiver, minerJValue[1] sender.
                    
                            var transfercheck = miner.ToString().Contains("transfer_asset");
                            //it wasn't a transfer
                            if (!transfercheck)
                            {
                                Console.WriteLine("Not Transfer.");
                            }
                            else
                            {
                                JArray receiver = (JArray)miner["updatedAddresses"];
                                JValue signer = (JValue)miner["signer"];

                                if (signer.ToString() == receiver[0].ToString())
                                {
                                    if (receiver[1].ToString() != MainObject)
                                    {
                                        Console.WriteLine("Not Same Receiver.");
                                    }
                                    else
                                    {
                                        JArray action = (JArray)miner["actions"];
                                        //Console.WriteLine(action.ToString());
                                        string actionstring = action.ToString();
                                        int firstStringPosition = actionstring.IndexOf("NCG");
                                        int secondStringPosition = actionstring.IndexOf("recipient");
                                        string stringBetweenTwoStrings = actionstring.Substring(firstStringPosition + 3,
                                            secondStringPosition - firstStringPosition - 3);
                                        //Console.WriteLine(stringBetweenTwoStrings);
                                        string resultString = Regex.Match(stringBetweenTwoStrings, @"\d+").Value;
                                        int valuetoadd = int.Parse(resultString);
                                        amount += valuetoadd;
                                    }
                                }
                                else
                                {
                                    if (receiver[0].ToString() != MainObject)
                                    {
                                        Console.WriteLine("Not Same Receiver.");
                                    }
                                    else
                                    {
                                        JArray action = (JArray)miner["actions"];
                                        //Console.WriteLine(action.ToString());
                                        string actionstring = action.ToString();
                                        int firstStringPosition = actionstring.IndexOf("NCG");
                                        int secondStringPosition = actionstring.IndexOf("recipient");
                                        string stringBetweenTwoStrings = actionstring.Substring(firstStringPosition + 3,
                                            secondStringPosition - firstStringPosition - 3);
                                        //Console.WriteLine(stringBetweenTwoStrings);
                                        string resultString = Regex.Match(stringBetweenTwoStrings, @"\d+").Value;
                                        int valuetoadd = int.Parse(resultString);
                                        amount += valuetoadd;
                                    }
                                }
                            }   
                        }
                    }

                    if (amount > 0)
                    {
                        amount += 1;
                        await ctx.Client.SendMessageAsync(ctx.Channel, address + " -> " + MainObject + " the following amount of NCG " + amount);
                    }
                    if (amount == 0)
                        y++;
                    amount = 0;
                    ignorereceiver.Add(MainObject);
                    MainObject = string.Empty;
                }
                Console.WriteLine("stopped");
                ignorereceiver.Clear();
                blocklist.Clear();
                GC.Collect();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await ctx.Client.SendMessageAsync(ctx.Channel,"I timed out, this was likely due to no TX's being sent from this address.");
            }
            return false;
        }

        public static int Checklevel(string address)
        {
            int pass = 0;
            var client = new RestClient("https://mars.9cscan.com/graphql");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\"query\":\"query {\\r\\n  stateQuery{\\r\\n    agent(address: \\\""+address+"\\\")\\r\\n        {avatarStates{level}\\r\\n    \\r\\n    }\\r\\n  }\\r\\n}\\r\\n   \",\"variables\":{}}",
                       ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            JObject joResponse = JObject.Parse(response.Content);

            var resultObjects = AllChildren(JObject.Parse(response.Content))
            .First(c => c.Type == JTokenType.Array && c.Path.Contains("data"))
            .Children<JObject>();

            foreach (JObject entry in resultObjects)
            {
                JValue level = (JValue)entry["level"];
                Console.WriteLine(level.ToString());
                int levelvalue = int.Parse(level.ToString());
                if (levelvalue >= 150)
                    pass = 1;
            }

            return pass;
        }

        public static int CheckMonster(string address)
        {
            int pass = 0;
            var client = new RestClient("https://mars.9cscan.com/graphql");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\"query\":\"query{\\r\\n  stateQuery{\\r\\n    monsterCollectionState(agentAddress:\\\""+address+"\\\"){\\r\\n    \\tlevel\\r\\n    }\\r\\n  }\\r\\n}\",\"variables\":{}}",
                       ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            JObject joResponse = JObject.Parse(response.Content);

                JValue level = (JValue)joResponse["data"]["stateQuery"]["monsterCollectionState"]["level"];
                Console.WriteLine(level.ToString());
                int levelvalue = int.Parse(level.ToString());
                if (levelvalue >= 3)
                    pass = 1;

            return pass;
        }
    }
}
