using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using C9_NCG_DiscordBot.Models;
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
                //Console.WriteLine("Calling API");
                var address = blockchain.NodesCheck.NodeSelector();
                var client = new RestClient(address.Result);
                client.Timeout = -1;
                var request = new RestSharp.RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", "{\"query\":\"{\\n  goldBalance(address:\\\"" + publickey + "\\\")\\n}\\n\"}", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                //Console.WriteLine("Response Received, parsing");
                //We got response, now let's parse it.
                JObject joResponse = JObject.Parse(response.Content);
                JObject ojObject = (JObject)joResponse["data"];
                JValue goldvalue = (JValue)ojObject["goldBalance"];
                string ncg = goldvalue.Value<string>();
                //Console.WriteLine(ncg);
                return ncg;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }

        public string NCGGoldLocal(string publickey , string address)
        {
            try
            {
                var client = new RestClient(address);
                client.Timeout = -1;
                var request = new RestSharp.RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", "{\"query\":\"{\\n  goldBalance(address:\\\"" + publickey + "\\\")\\n}\\n\"}", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                //Console.WriteLine("Response Received, parsing");
                //We got response, now let's parse it.
                JObject joResponse = JObject.Parse(response.Content);
                JObject ojObject = (JObject)joResponse["data"];
                JValue goldvalue = (JValue)ojObject["goldBalance"];
                string ncg = goldvalue.Value<string>();
                //Console.WriteLine(ncg);
                return ncg;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }

        public async Task<string> NCGProfileNewAsync(string publickey)
        {
            ProfileModel profile = new ProfileModel();

                //Regular API Call Below
                try
                {
                    Console.WriteLine("Calling API");
                    var address = blockchain.NodesCheck.NodeSelector();
                    var client = new RestClient(address.Result);
                    client.Timeout = -1;
                    var request = new RestRequest(Method.POST);
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
                    return (ncg);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            return null;
        }

        public string NCGGoldBeforeHash(string publickey, string hash)
        {
            try
            {
                Console.WriteLine("Calling API");
                var address = blockchain.NodesCheck.NodeSelector();
                var client = new RestClient(address.Result);
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
            catch (Exception)
            {
                return "false";
            }
        }


        public async Task<string> SendNCG(string publickey, float amount)
        {
            try
            {
                Console.WriteLine("Calling API");
                var client = new RestClient("http://localhost:23061/graphql");
                client.Timeout = -1;
                var request = new RestSharp.RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", "{\"query\":\"mutation{transferGold(recipient:\\\"" + publickey + "\\\",amount:\\\"" + amount + "\\\")}\"}", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                Console.WriteLine("Response Received, parsing");
                //We got response, now let's parse it.
                JObject joResponse = JObject.Parse(response.Content);
                JObject ojObject = (JObject)joResponse["data"];
                JValue goldvalue = (JValue)ojObject["transferGold"];
                string ncg = goldvalue.Value<string>();
                Console.WriteLine(ncg);
                return ncg;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }

        public async Task<string> CheckNCG(string txid)
        {
            try
            {
                Console.WriteLine("Calling API");
                var address = blockchain.NodesCheck.NodeSelector();
                var client = new RestClient(address.Result);
                client.Timeout = -1;
                var request = new RestSharp.RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", "{\"query\":\"query{\\r\\n  getTx(txId: \\\""+txid+"\\\")\\r\\n{id}\\r\\n}\",\"variables\":{}}",ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                Console.WriteLine("Response Received, parsing");
                //We got response, now let's parse it.
                JObject joResponse = JObject.Parse(response.Content);
                JObject ojObject = (JObject)joResponse["data"];
                JValue goldvalue = (JValue)ojObject["getTx"]["id"];
                string ncg = goldvalue.Value<string>();
                Console.WriteLine(ncg);
                return ncg;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }

        public async Task<string> ChainReady()
        {
            try
            {
                //Console.WriteLine("Calling API");
                var client = new RestClient("http://localhost:23061/graphql");
                client.Timeout = -1;
                var request = new RestSharp.RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", "{\"query\":\"query {\\r\\n  nodeStatus{preloadEnded}\\r\\n}\",\"variables\":{}}", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                //Console.WriteLine("Response Received, parsing");
                //We got response, now let's parse it.
                JObject joResponse = JObject.Parse(response.Content);
                JObject ojObject = (JObject)joResponse["data"];
                JValue goldvalue = (JValue)ojObject["nodeStatus"]["preloadEnded"];
                string ncg = goldvalue.Value<string>();
                //Console.WriteLine(ncg);
                return ncg;
            }
            catch (Exception ex)
            {
                return "false";
            }
            return "false";
        }

        public static async Task<int> TipImport(string address, string tomatch)
        {
            SqliteDataAccess db = new SqliteDataAccess();
            var address2 = blockchain.NodesCheck.NodeSelector();
            var client = new RestClient(address2.Result);
            client.Timeout = 200000;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\"query\":\"query{\\r\\n chainQuery{transactionQuery{transactions(desc:true signer:\\\"" + address + "\\\" limit:50){id,signer,actions{inspection},updatedAddresses}}}\\r\\n}\",\"variables\":{}}",
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
                    var id = (JValue)result["id"];
                    string tid = id.ToString();
                    string tstring = text.ToString();
                    if (tstring.Contains("transfer_asset", StringComparison.OrdinalIgnoreCase))
                    {
                        var affect = result["updatedAddresses"];
                        string sender = affect.ToString();
                        string receiver = sender;
                        receiver = receiver.Split(',')[1];
                        sender = Regex.Match(sender, @"0x[a-zA-Z0-9]{40}").Value;
                        receiver = Regex.Match(receiver, @"0x[a-zA-Z0-9]{40}").Value;
                        //check if ID has been used before.
                        var exists = db.TipTopUpIDCheck(tid);
                        if (sender == address && receiver == tomatch && exists.Result == 0)
                        {
                            tstring = tstring.Replace(System.Environment.NewLine, string.Empty);
                            tstring = tstring.Replace(@"\n", string.Empty);
                            tstring = tstring.Replace(@"\", string.Empty);
                            var resultString = Regex.Match(tstring, @"      [0-9]\d\d\d?\d?\d?\d?\d?    ").Value;
                            resultString = resultString.Replace(@" ", string.Empty);
                            int amount = int.Parse(resultString);
                            amount = amount / 100;
                            //register the id, so it can't be rewarded again.
                            var verify = db.TipTopUpAdd(tid,sender);
                            if (verify.Result)
                                return amount;
                            else
                                return 0;
                        }
                        Console.WriteLine("Not address we looking for");
                    }
                }
            }
            catch { return 0; }
            return 0;
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
