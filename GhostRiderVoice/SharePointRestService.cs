using GhostRider.Domain.Models;
using SimpleRest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace GhostRider.VoiceCommands {
    public sealed class SharePointRestService {
        private SimpleRestClient SharePointRestClient { get; set; }
        // http://ghostriderwebapi.azurewebsites.net/api/sharepoint/GetSites
        public SharePointRestService() {
            this.SharePointRestClient = new SimpleRestClient() {
                BaseUrl = "http://ghostriderwebapi.azurewebsites.net/api/sharepoint",
                ResponseFormat = HttpResponseFormat.Json
            };
        }

        //public IAsyncOperationWithProgress<Windows.Web.Http.HttpResponseMessage, Windows.Web.Http.HttpProgress> GetSiteListAsync() {
        //    SimpleMessage siteListMessage = new SimpleMessage() {
        //        EndPointPath = "/GetSites",
        //        HttpMethod = HttpMethod.Get
        //    };

        //    return await SharePointRestClient.SendMessageWithProgress<string>(siteListMessage);
        //   // return from.AsAsyncOperation();

        //    //SimpleResponse<IEnumerable<SharePointTeam>> response = SharePointRestClient.SendMessageAsync<IEnumerable<SharePointTeam>>(siteListMessage).Result;


        //    //return Task.Run(() => (IEnumerable<SharePointTeam>)response.DataConverted).AsAsyncOperation<IEnumerable<SharePointTeam>>();


        //    //  SimpleResponse <List<SharePointTeam>> response = 
        //    // return response.DataConverted;
        //}

      

        public async void CreateNewSite(string siteName) {
            BuildSiteRequest toSend = new BuildSiteRequest() {
                SiteName = siteName
            };
            SimpleMessage buildSiteMessage = new SimpleMessage() {
                EndPointPath = "/BuildSite",
                HttpMethod = HttpMethod.Post,
                Content = toSend
            };
           // SharePointRestClient.SendOneWayMessage(buildSiteMessage);
              SimpleResponse<Uri> response = await SharePointRestClient.SendMessageAsync<Uri>(buildSiteMessage);
            //  return new Uri("");//response.DataConverted;
        }

        public async void CreateNewTask(string userFirstName, string action) {
            AddTaskRequest contentToSend = new AddTaskRequest() {
                TaskName = action,
                UserFirstName = userFirstName
            };
            SimpleMessage addTaskMessage = new SimpleMessage() {
                EndPointPath = "/AddTask",
                HttpMethod = HttpMethod.Post,
                Content = contentToSend
            };
          //  SharePointRestClient.SendOneWayMessage(addTaskMessage);
             SimpleResponse<SharePointTask> response = await SharePointRestClient.SendMessageAsync<SharePointTask>(addTaskMessage);
        }
       
    }


}
