using HttpLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.VoiceCommands;
using Windows.UI.Core;

namespace GhostRider.VoiceCommands
{
    public sealed class GhostRiderVoiceCommandService : IBackgroundTask {
        public VoiceCommandServiceConnection VoiceServiceConnection { get; set; }
        //public SharePointRestService SharePointRestService { get; set; } = new SharePointRestService();
        private const string CREATE_TEAM_COMMAND = "createTeam";
        private const string ADD_TASK_COMMAND = "addTask";
        public async void Run(IBackgroundTaskInstance taskInstance) {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            var triggerDetails = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            if (triggerDetails != null) {
                this.VoiceServiceConnection = VoiceCommandServiceConnection.FromAppServiceTriggerDetails(triggerDetails);
                //    toReturn = await voiceCommandConnection.GetVoiceCommandAsync();
            }
            VoiceCommandResponse voiceResponse = VoiceCommandResponse.CreateResponse(new VoiceCommandUserMessage() {
                DisplayMessage = "Default Response",
                SpokenMessage = $"Sorry, something went wrong when running the command"
            }); 

            try {
                VoiceCommand voiceCommand = await VoiceServiceConnection.GetVoiceCommandAsync();
                RestSender rester = new RestSender();
                switch (voiceCommand.CommandName) {
                    case CREATE_TEAM_COMMAND:
                        string teamName = voiceCommand?.Properties["siteName"]?[0];
                        // voiceResponse = CreateSite(teamName);
                        await ShowProgressScreen($"Creating a team portal for the {teamName}");
                        Uri toNavTo = await rester.CreateSharePointSiteAsync(teamName);
                        await ShowProgressScreen($"Navigating to the {teamName} portal");
                        await rester.NavigateToUriAsync(toNavTo);
                        voiceResponse = VoiceCommandResponse.CreateResponse(new VoiceCommandUserMessage() {
                            DisplayMessage = "Team portal created successfully!",
                            SpokenMessage = $"Team portal was created successfully!"
                        });
                        break;
                    case ADD_TASK_COMMAND:
                        string firstName = voiceCommand.Properties["firstName"][0];
                        string taskName = voiceCommand.Properties["action"][0];
                        await ShowProgressScreen($"Telling {firstName} to {taskName}");
                        await rester.CreateTaskAsync(userFirstName: firstName, taskName: taskName);
                        voiceResponse = VoiceCommandResponse.CreateResponse(new VoiceCommandUserMessage() {
                            DisplayMessage = "Task created successfully!",
                            SpokenMessage = $"{firstName} was told to {taskName}!"
                        });
                        break;
                    default:
                        break;
                }
              
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.Message);
            }
            finally {
                deferral.Complete();
            }
            await VoiceServiceConnection.ReportSuccessAsync(voiceResponse);

        }

        #region PRIVATE HELPER METHODS
        //private VoiceCommandServiceConnection GetVoiceCommandConnection(AppServiceTriggerDetails triggerDetails) {
        //    VoiceCommandServiceConnection toReturn = null;
        //    if (triggerDetails != null) {
        //        toReturn = VoiceCommandServiceConnection.FromAppServiceTriggerDetails(triggerDetails);
        //        //    toReturn = await voiceCommandConnection.GetVoiceCommandAsync();
        //    }
        //    return toReturn;
        //}

        //private async Task HandleSharePointVoiceCommand() {
        //    VoiceCommand voiceCommand = await VoiceServiceConnection.GetVoiceCommandAsync();
        //    VoiceCommandResponse voiceResponse = GetDefaultResponse();

        //    Uri teamPortalUri = GetFullSiteUrl(voiceCommand);
            
        //    switch (voiceCommand.CommandName) {
        //        case CREATE_TEAM_COMMAND:
        //            string teamName = voiceCommand?.Properties["siteName"]?[0];
                  
        //            voiceResponse = CreateSite(teamName);
        //          //  await ShowProgressScreen($"Creating a team portal for the {teamName}");

        //            break;
        //        case "addTask":
                 

                        
        //                RestSender rester = new RestSender();
                        
        //                string firstName = voiceCommand.Properties["firstName"][0];
        //                string taskName = voiceCommand.Properties["action"][0];
        //                await ShowProgressScreen($"Telling {firstName} to {taskName}");
        //             //   rester.CreateTaskAsync(userFirstName: firstName, taskName: taskName);
        //                //await restService.SendTaskRequest();

        //                //  voiceResponse = CreateTask(firstName, taskName);

        //         //   await ShowProgressScreen($"Telling {firstName} to {taskName}");
        //            break;
        //        default:
        //            break;
        //    }
        //    await VoiceServiceConnection.ReportSuccessAsync(voiceResponse);
        //    // Can't Launch a URI until Success/Failure/etc has been reported or it will break
        //    // Can't do this when the app isn't launched
        //    // MAKE SURE THERE IS A BREAKPOINT HERE (Background app is ending tasks before they are finished)
        //    if (voiceCommand.CommandName == CREATE_TEAM_COMMAND) {
        //        await Windows.System.Launcher.LaunchUriAsync(teamPortalUri);
        //    }
        //}

        //private VoiceCommandResponse GetDefaultResponse() {
        //    var cortanaResponse = new VoiceCommandUserMessage() {
        //        DisplayMessage = "Couldn't understand the command",
        //        SpokenMessage = $"Sorry, I didn't understand the command"
        //    };
        //    return VoiceCommandResponse.CreateResponse(cortanaResponse);
        //}



        //private Uri GetFullSiteUrl(VoiceCommand voiceCommand) {
        //    Uri toReturn = null;
        //    if (voiceCommand.CommandName == CREATE_TEAM_COMMAND) {
        //        string teamName = voiceCommand?.Properties["siteName"]?[0];
        //        string teamNameSubsiteUrl = teamName?.Replace(" ", "");
        //        toReturn = new Uri($"https://lfuller941.sharepoint.com/JusticeLeague/{teamNameSubsiteUrl}");
        //    }
        //    return toReturn;
           
        //}
       

        //private VoiceCommandResponse CreateTask(string firstName, string action) {
        //    var cortanaResponse = new VoiceCommandUserMessage() {
        //        DisplayMessage = "Task is being created!",
        //        SpokenMessage = $"Told {firstName} to {action}"
        //    };
        //    this.SharePointRestService.CreateNewTask(firstName, action);
        //    return VoiceCommandResponse.CreateResponse(cortanaResponse);
        //}

        //private VoiceCommandResponse CreateSite(string siteName) {
        //    var cortanaResponse = new VoiceCommandUserMessage() {
        //        DisplayMessage = "Team is now being created!",
        //        SpokenMessage = $"The {siteName} is now being assembled!"
        //    };
        //    this.SharePointRestService.CreateNewSite(siteName);

        //    return VoiceCommandResponse.CreateResponse(cortanaResponse);
        //}

        private async Task ShowProgressScreen(string message) {
            var userProgressMessage = new VoiceCommandUserMessage() {
                DisplayMessage = message
            };
            VoiceCommandResponse response = VoiceCommandResponse.CreateResponse(userProgressMessage);
            await VoiceServiceConnection.ReportProgressAsync(response);
        }

        #endregion
    }
}
