using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Web.Http;
using System.Text;
using System.Threading.Tasks;
using SimpleRest.Api.MessageServices;
using Newtonsoft.Json;

namespace SimpleRest.Api {
    internal class MessageSender {
        public static async Task<ISimpleResponse<T>> SendMessage<T>(ISimpleRestClient client, ISimpleMessage message) {
            T messageTypeToReturn = Activator.CreateInstance<T>();
            ISimpleResponse<T> toReturn = new SimpleResponse<T>();
            
            HttpClient restClient = new HttpClient();
            Windows.Web.Http.HttpMethod method = message.HttpMethod == HttpMethod.Get ? Windows.Web.Http.HttpMethod.Get : Windows.Web.Http.HttpMethod.Post;

            // Add Global Request Parameters
            foreach (var globalRequestParameter in client.GlobalRequestParameters) {
                message.Parameters[globalRequestParameter.Key] = globalRequestParameter.Value;
            }

            Uri uriWithQueryString = await MessageHelper.BuildUri(baseUrl: client.BaseUrl, endPointPath: message.EndPointPath, queryStringParameters: message.Parameters);
            HttpRequestMessage requestMessage = new HttpRequestMessage(method, uriWithQueryString);
            // Add Headers
            foreach (var header in client.Headers) {
                requestMessage.Headers.Add(header.Key, header.Value);
            }
           

            HttpResponseMessage response = await restClient.SendRequestAsync(requestMessage);
            toReturn.DataRaw = response.Content;
            toReturn.DataConverted = Deserializer.JsonToObject<T>(response.Content.ToString());


            return toReturn;
        }
    }
}
