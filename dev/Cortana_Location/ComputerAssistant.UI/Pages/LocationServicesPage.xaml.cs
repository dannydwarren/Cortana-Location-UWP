using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Navigation;
using ComputerAssistant.UI.Models;
using FeatureWrappers;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ComputerAssistant.UI.Pages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class LocationServicesPage : Page
	{
		public LocationServicesPage()
		{
			this.InitializeComponent();
		}

		public ObservableCollection<BeamedLocation> BeamedLocations { get; } =
			new ObservableCollection<BeamedLocation>();


		protected async override void OnNavigatedTo( NavigationEventArgs e )
		{
			base.OnNavigatedTo( e );

			if ( await CheckLocationAccessAsync() ) return;

			BeamedLocations.CollectionChanged += BeamedLocations_CollectionChanged;

			if ( !BeamedLocations.Any() )
			{
				//Seed for this run
				Geoposition geoposition = await LocationWrapper.Instance.GetSingleShotLocationAsync();
				await AddBeamedLocation( geoposition );

				//TODO: Go to disk and get the file.
			}

			//Activate Continous Tracking
			//LocationWrapper.Instance.LocationChanged += LocationChanged;
			//await LocationWrapper.Instance.ActivateContinousLocationTracking();
		}

		private async void LocationChanged( object sender, Geoposition e )
		{
			await AddBeamedLocation( e );
		}

		private static async Task<bool> CheckLocationAccessAsync()
		{
			GeolocationAccessStatus geolocationAccessStatus =
				await LocationWrapper.Instance.RequestAccessToLocationData();
			if ( geolocationAccessStatus != GeolocationAccessStatus.Allowed )
			{
				await new MessageDialog( "Access to Location Data is Denied." ).ShowAsync();
				return true;
			}
			return false;
		}

		protected override void OnNavigatedFrom( NavigationEventArgs e )
		{
			base.OnNavigatedFrom( e );

			BeamedLocations.CollectionChanged -= BeamedLocations_CollectionChanged;
		}

		private void BeamedLocations_CollectionChanged( object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e )
		{
			BeamedLocation[] newBeamedLocations = e.NewItems.OfType<BeamedLocation>().ToArray();

			foreach ( BeamedLocation newBeamedLocation in newBeamedLocations )
			{
				var mapElement = new MapIcon
				{
					Location = newBeamedLocation.Point,
					NormalizedAnchorPoint = new Point( 0.5, 1.0 )
				};

				MyMapControl.MapElements.Add( mapElement );
			}
		}

		private async Task AddBeamedLocation( Geoposition geoposition )
		{
			if ( await CheckLocationAccessAsync() ) return;

			BeamedLocations.Insert( 0, new BeamedLocation( geoposition ) );
		}

		private async void ForceBeamLocationClickHandler( object sender, RoutedEventArgs e )
		{
			Geoposition geoposition = await LocationWrapper.Instance.GetSingleShotLocationAsync();
			await AddBeamedLocation( geoposition );
		}

		private async void OpenLocationSettingsButtonClickHandler( object sender, RoutedEventArgs e )
		{
			await Launcher.LaunchUriAsync( new Uri( "ms-settings:privacy-location" ) );
		}
	}
}
