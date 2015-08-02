﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ComputerAssistant.UI.FeatureWrappers;
using ComputerAssistant.UI.Models;

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

			if (await CheckLocationAccessAsync()) return;

			BeamedLocations.CollectionChanged += BeamedLocations_CollectionChanged;

			if ( !BeamedLocations.Any() )
			{
				//Seed for this run
				await AddBeamedLocation();

				//TODO: Go to disk and get the file.
			}

		}

		private static async Task<bool> CheckLocationAccessAsync()
		{
			GeolocationAccessStatus geolocationAccessStatus = await LocationWrapper.Instance.RequestAccessToLocationData();
			if (geolocationAccessStatus != GeolocationAccessStatus.Allowed)
			{
				await new MessageDialog("Access to Location Data is Denied.").ShowAsync();
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

		private async Task AddBeamedLocation()
		{
			if (await CheckLocationAccessAsync()) return;

			Geoposition geoposition = await LocationWrapper.Instance.GetSingleShotLocationAsync();
			BeamedLocations.Insert( 0, new BeamedLocation( geoposition ) );
		}

		private async void ForceBeamLocationClickHandler( object sender, RoutedEventArgs e )
		{
			await AddBeamedLocation();
		}

		private async void OpenLocationSettingsButtonClickHandler( object sender, RoutedEventArgs e )
		{
			await Launcher.LaunchUriAsync( new Uri( "ms-settings:privacy-location " ) );
		}
	}
}