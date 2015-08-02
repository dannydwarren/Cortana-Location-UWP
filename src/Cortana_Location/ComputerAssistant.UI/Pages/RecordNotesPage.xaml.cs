using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using ComputerAssistant.UI.Models;

namespace ComputerAssistant.UI.Pages
{
	public sealed partial class RecordNotesPage : Page
	{
		public RecordNotesPage()
		{
			this.InitializeComponent();
		}

		public ObservableCollection<CaptainsLogEntry> LogEntries { get; } =
			new ObservableCollection<CaptainsLogEntry>();

		protected override void OnNavigatedTo( NavigationEventArgs e )
		{
			base.OnNavigatedTo( e );

			if ( !LogEntries.Any() )
			{
				//Seed for this run
				AddLogEntry( "Starting App" );

				//TODO: Go to disk and get the file.
			}

		}

		private void AddLogEntry( string entryText )
		{
			LogEntries.Insert( 0, new CaptainsLogEntry( entryText ) );
		}
	}
}
