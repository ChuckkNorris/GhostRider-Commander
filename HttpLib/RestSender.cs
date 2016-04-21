using GhostRider.Domain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace HttpLib
{
    public class RestSender
    {

        public async Task GetTeams() {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));
            HttpRequestMessage message = new HttpRequestMessage() {
                Method = HttpMethod.Get,
                RequestUri = new Uri("http://ghostriderwebapi.azurewebsites.net/api/sharepoint/GetSites")
            };
            HttpResponseMessage response =  await httpClient.SendRequestAsync(message);

        }

        public async Task CreateTaskAsync(string userFirstName, string taskName) {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));
            HttpRequestMessage message = new HttpRequestMessage() {
                Method = HttpMethod.Post,
                RequestUri = new Uri("http://ghostriderwebapi.azurewebsites.net/api/sharepoint/AddTask")
            };

            AddTaskRequest newTask = new AddTaskRequest() {
                TaskName = taskName,
                UserFirstName = userFirstName
            };
            string contentToSend = JsonConvert.SerializeObject(newTask);
            message.Content = new HttpStringContent(contentToSend, UnicodeEncoding.Utf8, "application/json");
            await httpClient.SendRequestAsync(message);

        }

        public async Task<Uri> CreateSharePointSiteAsync(string siteName) {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));
            HttpRequestMessage message = new HttpRequestMessage() {
                Method = HttpMethod.Post,
                RequestUri = new Uri("http://ghostriderwebapi.azurewebsites.net/api/sharepoint/BuildSite")
            };

            var newSite = new BuildSiteRequest() {
                SiteName = siteName
            };
            string contentToSend = JsonConvert.SerializeObject(newSite);
            message.Content = new HttpStringContent(contentToSend, UnicodeEncoding.Utf8, "application/json");
            HttpResponseMessage response = await httpClient.SendRequestAsync(message);
            Debug.WriteLine(response.Content.ToString());
            return JsonConvert.DeserializeObject<Uri>(response.Content.ToString());
        }

        public async Task NavigateToUriAsync(Uri toNavigateTo) {
            await Windows.System.Launcher.LaunchUriAsync(toNavigateTo);
        }

        public async Task GetTasksForUser(string userFirstName) {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));

            HttpRequestMessage message = new HttpRequestMessage() {
                Method = HttpMethod.Get,
                RequestUri = new Uri("http://ghostriderwebapi.azurewebsites.net/api/sharepoint/GetTasks")
            };

            HttpResponseMessage response = await httpClient.SendRequestAsync(message);
            // IEnumerable<SharePointTask> toReturn = JsonConvert.DeserializeObject<IEnumerable<SharePointTask>>(response.Content.ToString());
        }
    }
}
