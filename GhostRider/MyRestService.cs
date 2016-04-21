using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace GhostRider {
    public class MyRestService {
        public async Task SendTaskRequest() {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));
            HttpRequestMessage message = new HttpRequestMessage() {
                Method = HttpMethod.Post,
                RequestUri = new Uri("http://ghostriderwebapi.azurewebsites.net/api/sharepoint/AddTask")
            };

            //AddTaskRequest newTask = new AddTaskRequest() {
            //    TaskName = "Do something cool",
            //    UserFirstName = "Superman"
            //};
            //string contentToSend = JsonConvert.SerializeObject(newTask);
            //message.Content = new HttpStringContent(contentToSend, UnicodeEncoding.Utf8, "application/json");
            await httpClient.SendRequestAsync(message);

        }
    }
}
