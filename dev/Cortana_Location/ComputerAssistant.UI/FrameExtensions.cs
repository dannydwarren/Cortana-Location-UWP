using Windows.UI.Xaml.Controls;

namespace ComputerAssistant.UI
{
	public static class FrameExtensions
	{
		public static bool NavigateTo<TPage>( this Frame frame, object parameter = null )
			where TPage : Page
		{
			if ( parameter == null )
			{
				return frame.Navigate( typeof( TPage ) );
			}
			return frame.Navigate( typeof( TPage ), parameter );
		}
	}
}