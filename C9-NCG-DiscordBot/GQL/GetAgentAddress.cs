using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace C9_NCG_DiscordBot.GQL
{
    public class GetAgentAddress
    {

        public static string ViaAvatarAddress(string avatarAddress)
        {
            var client = new RestClient("http://localhost:23061/graphql");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\"query\":\"query {\\r\\n  stateQuery {\\r\\n    avatar(avatarAddress: \\\""+avatarAddress+"\\\") {\\r\\n        agentAddress,level\\r\\n      }\\r\\n    }\\r\\n  }\",\"variables\":{}}",
                       ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            JObject joResponse = JObject.Parse(response.Content);
            JObject ojObject = (JObject)joResponse["data"]["stateQuery"]["avatar"];
            JValue goldvalue = (JValue)ojObject["agentAddress"];
            string agentAddress = goldvalue.Value<string>();

            return agentAddress;
        }
    }
}
