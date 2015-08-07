using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeatureWrappers;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Devices.Geolocation;

namespace CortanaAssistant.CortanaAppService
{
	//IMPORTANT: MUST BE SEALED!
	//	From the Documentation: 
	//	"The background task class itself, and all other classes in the background task project, need to be sealed public classes."
	public sealed class ComputerAssistantVoiceCommandService : IBackgroundTask
	{
		BackgroundTaskDeferral _deferral;
		VoiceCommandServiceConnection voiceServiceConnection;

		public async void Run( IBackgroundTaskInstance taskInstance )
		{
			//Get a deferral
			_deferral = taskInstance.GetDeferral();

			taskInstance.Canceled += OnTaskCanceled;

			var triggerDetails =
				taskInstance.TriggerDetails as AppServiceTriggerDetails;

			if ( triggerDetails != null
				//Name == [Name used in the appxmanifest Delaration]
				&& triggerDetails.Name == "ComputerAssistantCortanaAppService" )
			{
				try
				{
					voiceServiceConnection =
						VoiceCommandServiceConnection.FromAppServiceTriggerDetails( triggerDetails );

					voiceServiceConnection.VoiceCommandCompleted += VoiceCommandCompleted;

					VoiceCommand voiceCommand =
						await voiceServiceConnection.GetVoiceCommandAsync();

					VoiceCommandResponse response = null;

					switch ( voiceCommand.CommandName )
					{
						case "currentLocation":
							var userMessage = new VoiceCommandUserMessage();

							//Illegal? Geoposition geoposition = await LocationWrapper.Instance.GetSingleShotLocationAsync();

							//http://www.directionsmag.com/site/latlong-converter/
							string message = "Current location Decimal Degrees. Latitude 33.1362704. Longitude -117.2943426.";
							//string message = "Current location Decimal Degrees. "
							//	+ $"Latitude {geoposition.Coordinate.Point.Position.Latitude}. "
							//	+ $"Longitude {geoposition.Coordinate.Point.Position.Longitude}.";
							userMessage.DisplayMessage = message;
							userMessage.SpokenMessage = message;
							response = VoiceCommandResponse.CreateResponse( userMessage );
							break;

						default:
							break;
					}

					if ( response != null )
					{
						await voiceServiceConnection.ReportSuccessAsync( response );
					}
				}
				catch ( Exception ex )
				{
					Debug.WriteLine( ex );
					LaunchAppInForeground();
				}
				finally
				{
					if ( _deferral != null )
					{
						_deferral.Complete();
					}
				}


			}

			//all done
			_deferral.Complete();
		}
		private async void LaunchAppInForeground()
		{
			var userMessage = new VoiceCommandUserMessage();
			userMessage.SpokenMessage = "Failed to get current location";

			var response = VoiceCommandResponse.CreateResponse( userMessage );

			response.AppLaunchArgument = "";

			await voiceServiceConnection.RequestAppLaunchAsync( response );
		}

		private void VoiceCommandCompleted( VoiceCommandServiceConnection sender, VoiceCommandCompletedEventArgs args )
		{
			if ( _deferral != null )
			{
				_deferral.Complete();
			}
		}

		private void OnTaskCanceled( IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason )
		{
			if ( _deferral != null )
			{
				_deferral.Complete();
			}
		}
	}
}
