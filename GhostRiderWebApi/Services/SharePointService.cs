using GhostRider.Domain.Models;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace GhostRiderWebApi.Services {
    public class SharePointService {
        public string FullSiteUrl { get; set; }
        public ClientContext SharePointContext { get; set; }
        public SharePointService() {
            this.FullSiteUrl = $"{SHAREPOINT_SERVER_URL}/{ROOT_SITE_NAME}"; //
            this.SharePointContext = GetDefaultSharePointSiteContext();
        }

        private const string SHAREPOINT_SERVER_URL = "https://YourDomain.sharepoint.com";
        private const string ROOT_SITE_NAME = "JusticeLeague";
        private const string USER_NAME = "YourUsername@YourDomain.onmicrosoft.com";
        private const string PASSWORD = "YourPassword";
        private const string TASK_LIST_TITLE = "Hero Tasks";

        public Uri BuildAppSite(string siteName) {
            try {
                // Load RootSite e.g. JusticeLeague
                Web rootSite = SharePointContext.Web;
                SharePointContext.Load(rootSite);
                //build new project site 
                WebCreationInformation subsiteToCreationInfo = new WebCreationInformation() {
                    Url = siteName.Replace(" ", ""),
                    Title = siteName,
                    Description = "For Justice!"
                };
                Web newSubsite = SharePointContext.Web.Webs.Add(subsiteToCreationInfo);
                SharePointContext.Load(newSubsite, site => site.Lists);
                ListCollection lists = newSubsite.Lists;
                ListCreationInformation listCreationInfo = new ListCreationInformation() {
                    Title = TASK_LIST_TITLE,
                    TemplateType = 107, // ListTemplateType.Tasks,
                    Description = "Complete these quests to save the city!"
                };
                lists.Add(listCreationInfo);
                SharePointContext.ExecuteQuery();
                return new Uri(ConstructFullUrl(siteName.Replace(" ", "")));
                
            }
            catch (Exception ex) {
                return new Uri(ConstructFullUrl(siteName.Replace(" ", "")));
                //Debug.WriteLine(ex.Message);
                //throw ex;
            }
        }

        public SharePointTask AddUserTaskToLatestSite(string taskName, string userFirstName) {
            SharePointTask toReturn;
            DateTime dueDate = DateTime.Today.AddDays(2);
            Web latestSite = GetLatestSite();
            List myList = GetHeroTaskList(latestSite);
            var newSharePointTask = new ListItemCreationInformation();
            User toAssignTo = latestSite.SiteUsers.FirstOrDefault(user => user.Title.ToLower().Contains(userFirstName.ToLower()));
            ListItem taskToAdd = myList.AddItem(newSharePointTask);
            taskToAdd["AssignedTo"] = toAssignTo;
            taskToAdd["Title"] = taskName;
            taskToAdd["DueDate"] = dueDate;
            taskToAdd.Update();

            SharePointContext.ExecuteQuery();
            toReturn = new SharePointTask() {
                Title = taskName,
                AssignedTo = new SharePointUser() { Name = toAssignTo.Title },
                DueDate = dueDate
            };
            return toReturn;
        }

        private List GetHeroTaskList(Web site) {
            List toReturn = site.Lists.FirstOrDefault(list => list.Title == TASK_LIST_TITLE);
            return toReturn;
        }

        public List<SharePointTask> GetUserTasks(string userFirstName) {
            var toReturn = new List<SharePointTask>();
            string lowerCaseFirstName = userFirstName.ToLower();
            Web latestSite = GetLatestSite();

            List heroTaskList = latestSite.Lists.GetByTitle(TASK_LIST_TITLE); 
            CamlQuery query = CamlQuery.CreateAllItemsQuery();
       
            ListItemCollection taskList = heroTaskList.GetItems(query);
            SharePointContext.Load(taskList);
            SharePointContext.ExecuteQuery();
          
            foreach (ListItem heroTask in taskList) {
                FieldUserValue assignedUser = heroTask["AssignedTo"] as FieldUserValue;
                if (assignedUser.LookupValue.ToLower().Contains(lowerCaseFirstName)) {
                    toReturn.Add(new SharePointTask() {
                        // AssignedTo = (User)heroTask["AssignedTo"],
                        Title = (string)heroTask["Title"],
                        Status = (string)heroTask["Status"]
                    });
                }
            }
            return toReturn;
        }

        private Web GetLatestSite() {
            WebCollection webCollection = GetRootSite().Webs;
            Web toReturn = webCollection.OrderByDescending(subsite => subsite.Created).FirstOrDefault();
            this.SharePointContext.Load(toReturn, 
                site => site.SiteUsers,
                site => site.Lists);
            this.SharePointContext.ExecuteQuery();
            return toReturn;
        }

        private string ConstructFullUrl(string siteName) {
            return $"{SHAREPOINT_SERVER_URL}/{ROOT_SITE_NAME}/{siteName}";
        }
        
        public List<SharePointTeam> GetSites() {
            List<SharePointTeam> toReturn = new List<SharePointTeam>();
            Web rootSite = GetRootSite();
            foreach (Web subsite in rootSite.Webs) {
                Debug.WriteLine(subsite.Title);
                toReturn.Add(new SharePointTeam() {
                    Title = subsite.Title,
                //    TeamMembers = GetTeamMembers(subsite),
                    CreationDate = subsite.Created
                });
            }
            return toReturn.OrderByDescending(team => team.CreationDate).ToList();
        }

        private Web GetRootSite() {
            Web rootSite = this.SharePointContext.Web;
            this.SharePointContext.Load(rootSite, 
                website => website.Webs, 
                website => website.SiteUsers, 
              //  Site => Site.Webs.
                site => site.Lists);
            this.SharePointContext.ExecuteQuery();
            return rootSite;
        }

        private List<SharePointUser> GetTeamMembers(Web site) {
            List<SharePointUser> toReturn = new List<SharePointUser>();
            foreach (User teamMember in site.SiteUsers) {
                toReturn.Add(new SharePointUser() {
                    Name = teamMember.Title
                });
            }
            return toReturn; 
        }

        //public void AddUserToDMSite(string useremail, string securityGroupName, Web aWeb) {
        //    GroupCollection collGroup = aWeb.SiteGroups;
        //    Group oGroup1 = collGroup.GetByName("UserList");
        //    Group oGroup2 = collGroup.GetByName(securityGroupName);
        //    UserCollection oUserCollection1 = oGroup1.Users;
        //    UserCollection oUserCollection2 = oGroup2.Users;
        //    SPContext.Load(oUserCollection1);
        //    SPContext.Load(oUserCollection2);
        //    SPContext.ExecuteQuery();
        //    var uname = oGroup1.Users.GetByEmail(useremail);
        //    var userCheck = oUserCollection2.Where(u => u.Email == useremail).FirstOrDefault();
        //    if (userCheck == null) {
        //        Microsoft.SharePoint.Client.User oUser2 = oGroup2.Users.AddUser(uname);
        //    }
        //    SPContext.ExecuteQuery();
        //}


        private ClientContext GetDefaultSharePointSiteContext() {
            return new ClientContext(this.FullSiteUrl) {
                Credentials = new SharePointOnlineCredentials(USER_NAME, CreateSecuredPassword(PASSWORD))
            };
        }

        private SecureString CreateSecuredPassword(string password) {
            SecureString toReturn = new SecureString();
            foreach (char c in password.ToCharArray()) toReturn.AppendChar(c);
            return toReturn;
        }

        private async Task SetInitialProjectStatusAsync(ClientContext ctx, Web web) {
            var allProperties = web.AllProperties;
            allProperties["ProjectStatus"] = "In Process";

            web.Update();
            ctx.ExecuteQuery();
        }

    }
}
