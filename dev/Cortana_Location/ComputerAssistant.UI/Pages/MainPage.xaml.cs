using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ComputerAssistant.UI.Pages
{
	public sealed partial class MainPage : Page
	{
		public MainPage()
		{
			this.InitializeComponent();
		}

		private void RecordNotesDemoClickHandler( object sender, RoutedEventArgs e )
		{
			Frame.NavigateTo<RecordNotesPage>();
		}

		private void LocationServicesDemoClickHandler( object sender, RoutedEventArgs e )
		{
			Frame.NavigateTo<LocationServicesPage>();
		}
	}
}
