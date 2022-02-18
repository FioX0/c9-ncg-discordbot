using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using C9_NCG_DiscordBot.Models;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace C9_NCG_DiscordBot.blockchain
{
    class WNCG
    {


        [Event("Transfer")]
        public class TransferEventDTO : IEventDTO
        {
            [Parameter("address", "_from", 1, true)]
            public string From { get; set; }

            [Parameter("address", "_to", 2, true)]
            public string To { get; set; }

            [Parameter("uint256", "_value", 3, false)]
            public BigInteger Value { get; set; }
        }

        public static async Task<string> CheckETHHash(string key)
        {
            var url = "https://mainnet.infura.io/v3/8072b1dcb24c48868f422462eddc7214";
            var web3 = new Web3(url);
            try
            {
                var txnReceipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(key);

                var events = txnReceipt.DecodeAllEvents<TransferEventDTO>();

                Console.WriteLine(events[0].Event.To);

                string text = events[0].Event.To;

                return text;
            }
            catch (Exception ex) { }
            return string.Empty;
        }

        public static List<WncgModel> GetETHHash()
        {
            Console.WriteLine("Getting hash");
            var client = new RestClient("https://api.etherscan.io/api?module=account&action=txlist&address=0xf203ca1769ca8e9e8fe1da9d147db68b6c919817&startblock=0&endblock=99999999&page=1&offset=10000&sort=desc&apikey=A6VU8EE8SDXNYKQQF1WIEICFPV1KBB9D9Y");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\"query\":\"\",\"variables\":{}}",
                       ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            var resultObjects = AllChildren(JObject.Parse(response.Content))
                .First(c => c.Type == JTokenType.Array && c.Path.Contains("result"))
                .Children<JObject>();

            List<WncgModel> txlist = new List<WncgModel>();

            foreach (JObject result in resultObjects)
            {
                WncgModel item = new WncgModel();
                item.hash = (string)result["hash"];

                txlist.Add(item);
            }

            return txlist;
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
