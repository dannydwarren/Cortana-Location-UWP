using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ComputerAssistant.UI
{
	public sealed partial class PageHeaderControl : UserControl
	{
		public PageHeaderControl()
		{
			this.InitializeComponent();
		}



		public string PageHeaderText
		{
			get { return (string)GetValue( PageHeaderTextProperty ); }
			set { SetValue( PageHeaderTextProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for PageHeaderText.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty PageHeaderTextProperty =
			DependencyProperty.Register( "PageHeaderText", typeof( string ), typeof( PageHeaderControl ), new PropertyMetadata( "Page Header" ) );



		public string PageSubHeaderText
		{
			get { return (string)GetValue( PageSubHeaderTextProperty ); }
			set { SetValue( PageSubHeaderTextProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for PageSubHeaderText.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty PageSubHeaderTextProperty =
			DependencyProperty.Register( "PageSubHeaderText", typeof( string ), typeof( PageHeaderControl ), new PropertyMetadata( "Page SubHeader" ) );




		public Page HostPage
		{
			get { return (Page)GetValue( HostPageProperty ); }
			set { SetValue( HostPageProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for HostPage.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty HostPageProperty =
			DependencyProperty.Register( "HostPage", typeof( Page ), typeof( PageHeaderControl ), new PropertyMetadata( null, PropertyChangedCallbackHandler ) );

		private static void PropertyChangedCallbackHandler( DependencyObject d, DependencyPropertyChangedEventArgs e )
		{
			( (PageHeaderControl)d ).PropertyChangedCallbackHandler( e );
		}

		private void PropertyChangedCallbackHandler( DependencyPropertyChangedEventArgs e )
		{
			if (HostPage == null)
			{
				BackButton.Visibility = Visibility.Collapsed;
			}
			else
			{
				BackButton.Visibility = HostPage.Frame.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		private void BackClickHandler( object sender, RoutedEventArgs e )
		{
			if ( HostPage.Frame.CanGoBack )
			{
				HostPage.Frame.GoBack();
			}
			else
			{
				BackButton.Visibility = Visibility.Collapsed;
			}
		}
	}
}
