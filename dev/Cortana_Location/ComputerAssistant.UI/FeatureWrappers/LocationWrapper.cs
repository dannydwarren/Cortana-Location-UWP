using System;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Geolocation;
using Windows.UI.Core;
using Windows.UI.Popups;

namespace ComputerAssistant.UI.FeatureWrappers
{
	public class LocationWrapper
	{
		private Geolocator _continuousGeolocator;



		#region Singleton
		private static LocationWrapper _instance;
		public static LocationWrapper Instance => _instance ?? ( _instance = new LocationWrapper() );

		private LocationWrapper()
		{

		}
		#endregion



		public event EventHandler<Geoposition> LocationChanged = delegate { };
		private void OnLocationChanged( Geoposition geoposition )
		{
			LocationChanged( this, geoposition );
		}


		public GeolocationAccessStatus GeolocationAccessStatus { get; private set; }
		public bool IsTrackingLocation { get; private set; }


		public async Task<GeolocationAccessStatus> RequestAccessToLocationData()
		{
			GeolocationAccessStatus = await Geolocator.RequestAccessAsync();
			return GeolocationAccessStatus;
		}


		public async Task<Geoposition> GetSingleShotLocationAsync()
		{
			if ( GeolocationAccessStatus != GeolocationAccessStatus.Allowed )
			{
				return null;
			}

			//TODO: LocationWrapper 1.0 - GetSingleShotLocationAsync
			/*
			 * Create Geolocator
			 * Get location via geolocator.GetGeopositionAsync
			 * 
			 */


			Geolocator geolocator = new Geolocator
			{
				DesiredAccuracy = PositionAccuracy.High
			};

			Geoposition geoposition = null;
			string errorMessage = string.Empty;
			try
			{
				geoposition = await geolocator.GetGeopositionAsync( maximumAge: TimeSpan.FromSeconds( 1 ),
																	timeout: TimeSpan.FromSeconds( 10 ) );
			}
			catch ( Exception ex )
			{
				if ( (uint)ex.HResult == 0x80004004 )
				{
					errorMessage = "Location is disabled in device settings.";
				}
				if ( (uint)ex.HResult == 0x80070005 )
				{
					errorMessage = "Access denied by user.";
				}
			}

			if ( geoposition == null && !string.IsNullOrEmpty( errorMessage ) )
			{
				MessageDialog dialog = new MessageDialog( errorMessage );
				await dialog.ShowAsync();
			}

			//NOTE: Return value will be null if the user does not allow location or if location services are disabled.
			return geoposition;
		}

		public async Task<Geoposition> ActivateContinousLocationTracking()
		{
			if ( GeolocationAccessStatus != GeolocationAccessStatus.Allowed )
			{
				return null;
			}


			//TODO: LocationWrapper 2.0 - ActivateContinousLocationTracking
			/*
			 * Create Geolocation with reporting parameters
			 * Subscribe to geolocator.PositionChanged
			 * Call geolocator.GetGeopositionAsync to activate location tracking
			 * 
			 */

			if ( IsTrackingLocation )
				return await _continuousGeolocator.GetGeopositionAsync();

			_continuousGeolocator = new Geolocator()
			{
				DesiredAccuracy = PositionAccuracy.High,
				ReportInterval = 2000,
				MovementThreshold = 0,
			};

			_continuousGeolocator.PositionChanged += PositionChangedHandler;
			IsTrackingLocation = true;
			return await _continuousGeolocator.GetGeopositionAsync();
		}

		public void DeactivateContinuousLocaitonTracking()
		{
			if (_continuousGeolocator != null)
			{
				_continuousGeolocator.PositionChanged -= PositionChangedHandler;
				_continuousGeolocator = null;
			}

			IsTrackingLocation = false;
		}




		private async void PositionChangedHandler( Geolocator sender, PositionChangedEventArgs args )
		{
			await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync( CoreDispatcherPriority.Normal,
																	() => OnLocationChanged( args.Position ) );
		}

		public async Task<string> ConvertGeocoordinateToCivicAddress( Geoposition geoposition )
		{
			//TODO: LocationWrapper 3.0 - ConvertGeocoordinateToCivicAddress
			/*
			 * Use a service like BingMaps to convert a Geoposition into a civic address
			 * 
			 */

			//Temp Key: Create your own here: http://www.microsoft.com/maps/create-a-bing-maps-key.aspx
			string conversionUrlFormat =
				@"http://dev.virtualearth.net/REST/v1/Locations/{0},{1}?o=xml&key=AobdDrOVYLltY6q5iT9tsFDGiJm93KTUL6_hlp6QJLJNGoxa6O0s7C3HPs6RxW1D";

			string conversionUrl =
				string.Format( conversionUrlFormat,
				geoposition.Coordinate.Point.Position.Latitude,
				geoposition.Coordinate.Point.Position.Longitude );

			string response = null;

			HttpClient client = new HttpClient();
			response = await client.GetStringAsync( new Uri( conversionUrl ) );

			return response;
		}
	}
}
