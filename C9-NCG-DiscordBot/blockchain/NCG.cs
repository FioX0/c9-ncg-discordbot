using System;
using System.Collections.Generic;
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
                //"{\"query\":\"{\\n  goldBalance(address:\\\"0xa49d64c31A2594e8Fb452238C9a03beFD1119963\\\")\\n}\\n\"}"
                return ncg;
            }
            catch(Exception e)
            {
                return "false";
            }
        }
    }
}
