using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace C9_NCG_DiscordBot.blockchain
{
    public class NodesCheck
    {

        public static async Task<string> NodeSelector()
        {
            while (true)
            {
                try
                {
                    string address = string.Empty;
                    int blockindex = 0;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    var client = new RestClient("https://api.9cscan.com/status");
                    client.Timeout = -1;
                    var request = new RestRequest(Method.GET);
                    IRestResponse response = client.Execute(request);

                    var resultObjects = AllChildren(JObject.Parse(response.Content))
                        .First(c => c.Type == JTokenType.Array && c.Path.Contains("nodes"))
                        .Children<JObject>();

                    foreach (var entry in resultObjects)
                    {
                        var indexstring = (string)entry["blockIndex"];
                        if (indexstring != null)
                        {
                            if (blockindex < (int)entry["blockIndex"])
                            {
                                blockindex = (int)entry["blockIndex"];
                                address = (string)entry["endpoint"];
                            }
                        }
                    }

                    return address;
                }
                catch (Exception ex)
                {
                    return "http://localhost:23061/graphql";
                }

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
    }
}
