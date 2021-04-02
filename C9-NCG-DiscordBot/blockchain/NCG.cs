﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;

namespace C9_NCG_DiscordBot.blockchain
{
    public class NCG
    {

        public string NCGGold(string publickey)
        {
            try
            {
                Console.WriteLine("Calling API");
                var client = new RestClient("http://localhost:23061/graphql");
                client.Timeout = -1;
                var request = new RestSharp.RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", "{\"query\":\"{\\n  goldBalance(address:\\\"" + publickey + "\\\")\\n}\\n\"}", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                Console.WriteLine("Response Received, parsing");
                //We got response, now let's parse it.
                JObject joResponse = JObject.Parse(response.Content);
                JObject ojObject = (JObject)joResponse["data"];
                JValue goldvalue = (JValue)ojObject["goldBalance"];
                string ncg = goldvalue.Value<string>();
                Console.WriteLine(ncg);
                return ncg;
            }
            catch(Exception e)
            {
                return "false";
            }
        }

        public string NCGProfile(ulong discordid)
        {
            string docPath = "C:/profilerecord/profile.txt";
            string publickey = "";
            string stringcheck = "";

            if (File.ReadAllText(docPath).Contains(discordid.ToString()))
            {
                string[] lines = File.ReadAllLines(docPath);
                for (int x = 0; x < lines.Length; x++)
                {
                    stringcheck = lines[x].Split(':')[0];
                    Console.WriteLine(stringcheck);
                    if (discordid.ToString() == stringcheck)
                    {
                        publickey = lines[x].Split(':')[1];
                    }
                    else
                    {
                        
                    }
                }

                //Regular API Call Below
                try
                {
                Console.WriteLine("Calling API");
                var client = new RestClient("http://localhost:23061/graphql");
                client.Timeout = -1;
                var request = new RestSharp.RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", "{\"query\":\"{\\n  goldBalance(address:\\\"" + publickey + "\\\")\\n}\\n\"}", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                Console.WriteLine("Response Received, parsing");
                //We got response, now let's parse it.
                JObject joResponse = JObject.Parse(response.Content);
                JObject ojObject = (JObject)joResponse["data"];
                JValue goldvalue = (JValue)ojObject["goldBalance"];
                string ncg = goldvalue.Value<string>();
                Console.WriteLine(ncg);
                return ncg;
                }
                catch (Exception e)
                {
                    return "false";
                }
            }
            else
                return "false";
        }

        public string NCGGoldBeforeHash(string publickey, string hash)
        {
            try
            {
                Console.WriteLine("Calling API");
                var client = new RestClient("http://localhost:23061/graphql");
                client.Timeout = -1;
                var request = new RestSharp.RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", "{\"query\":\"{\\n  goldBalance(address: \\\""+ publickey+ "\\\", hash: \\\""+hash+"\\\")\\n}\\n\"}", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                Console.WriteLine("Response Received, parsing");
                //We got response, now let's parse it.
                JObject joResponse = JObject.Parse(response.Content);
                JObject ojObject = (JObject)joResponse["data"];
                JValue goldvalue = (JValue)ojObject["goldBalance"];
                string ncg = goldvalue.Value<string>();
                Console.WriteLine(ncg);
                return ncg;
            }
            catch (Exception e)
            {
                return "false";
            }
        }
    }
}
