using GhostRider.Domain.Models;
using GhostRiderWebApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace GhostRiderWebApi.Controllers {
    public class SharePointController : ApiController {
        private readonly SharePointService SharePointService;
        public SharePointController() {
            this.SharePointService = new SharePointService();
        }

        [HttpGet]
        public List<SharePointTeam> GetSites() {
            return this.SharePointService.GetSites();
        }

        [HttpPost]
        public Uri BuildSite([FromBody]BuildSiteRequest buildSiteRequest) {
            return this.SharePointService.BuildAppSite(buildSiteRequest.SiteName);
        }

        [HttpPost]
        public SharePointTask AddTask(AddTaskRequest addTaskRequest) {
            return this.SharePointService.AddUserTaskToLatestSite(addTaskRequest.TaskName, addTaskRequest.UserFirstName);
        }

        [HttpGet]
        public List<SharePointTask> GetTasks(string userFirstName) {
            return this.SharePointService.GetUserTasks(userFirstName);
        }

        //  return this.SharePointService.AddUserTaskToLatestSite("Do something useful","green lantern");
        [HttpGet]
        public SharePointTask Test() {
            return this.SharePointService.AddUserTaskToLatestSite("Do something useful", "green lantern");
        }

    }
}