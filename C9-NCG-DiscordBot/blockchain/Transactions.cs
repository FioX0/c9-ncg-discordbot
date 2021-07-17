using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace C9_NCG_DiscordBot.blockchain
{
    class Transactions
    {
        public string[] TransactionDetails(string txid)
        {
            try
            {
                Console.WriteLine("Calling API");
                var client = new RestClient("http://localhost:23061/graphql");
                client.Timeout = -1;
                var request = new RestSharp.RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", "{\"query\":\"query{getTx(txId: \\\"282f1d068d5f3eff8da47dc649e1aa03a217ce0b04097007c76136112e6517d7\\\"){updatedAddresses}}\"}",ParameterType.RequestBody);

                IRestResponse response = client.Execute(request);
                Console.WriteLine("Response Received, parsing");
                //We got response, now let's parse it.
                JObject joResponse = JObject.Parse(response.Content);
                JObject ojObject = (JObject)joResponse["data"];
                JArray jAddresses = (JArray)ojObject["getTx"]["updatedAddresses"];
                string[] address = jAddresses.ToObject<string[]>();
                //0 is sender // 1 is receiver.
                Console.WriteLine(address[0]);
                Console.WriteLine(address[1]);
                return address;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }
    }
}
