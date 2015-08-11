using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using ComputerAssistant.UI.Pages;
using System.Threading.Tasks;
using Windows.Media.SpeechRecognition;

namespace ComputerAssistant.UI
{
	sealed partial class App : Application
	{
		private Frame _rootFrame;

		public App()
		{
			this.InitializeComponent();
			this.Suspending += OnSuspending;
			RegisterVoiceCommands();
			this.UnhandledException += App_UnhandledException;
			TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
		}

		private void TaskScheduler_UnobservedTaskException( object sender, UnobservedTaskExceptionEventArgs e )
		{
			Debug.WriteLine( e.Exception );
		}

		private void App_UnhandledException( object sender, UnhandledExceptionEventArgs e )
		{
			Debug.WriteLine( e.Exception );
		}

		private async void RegisterVoiceCommands()
		{
			var storageFile =
				await Windows.Storage.StorageFile
				.GetFileFromApplicationUriAsync( new Uri( "ms-appx:///VoiceCommandDefinition.xml" ) );

			try
			{
				await Windows.ApplicationModel.VoiceCommands.VoiceCommandDefinitionManager
					.InstallCommandDefinitionsFromStorageFileAsync( storageFile );

				Debug.WriteLine( "Voice Commands Registered" );
			}
			catch ( Exception ex )
			{
				Debug.WriteLine( ex );
			}
		}

		protected override void OnLaunched( LaunchActivatedEventArgs e )
		{
			_rootFrame = Window.Current.Content as Frame;

			// Do not repeat app initialization when the Window already has content,
			// just ensure that the window is active
			if ( _rootFrame == null )
			{
				// Create a Frame to act as the navigation context and navigate to the first page
				_rootFrame = new Frame();

				_rootFrame.NavigationFailed += OnNavigationFailed;

				if ( e.PreviousExecutionState == ApplicationExecutionState.Terminated )
				{
					//TODO: Load state from previously suspended application
				}

				// Place the frame in the current Window
				Window.Current.Content = _rootFrame;
			}

			if ( _rootFrame.Content == null )
			{
				// When the navigation stack isn't restored navigate to the first page,
				// configuring the new page by passing required information as a navigation
				// parameter
				_rootFrame.NavigateTo<MainPage>( e.Arguments );
			}
			// Ensure the current window is active
			Window.Current.Activate();
		}

		void OnNavigationFailed( object sender, NavigationFailedEventArgs e )
		{
			throw new Exception( "Failed to load Page " + e.SourcePageType.FullName );
		}

		private void OnSuspending( object sender, SuspendingEventArgs e )
		{
			var deferral = e.SuspendingOperation.GetDeferral();
			//TODO: Save application state and stop any background activity
			deferral.Complete();
		}

		protected override void OnActivated( IActivatedEventArgs args )
		{
			base.OnActivated( args );

			_rootFrame = Window.Current.Content as Frame;

			if ( _rootFrame == null )
			{
				_rootFrame = new Frame();

				_rootFrame.NavigationFailed += OnNavigationFailed;

				Window.Current.Content = _rootFrame;
			}

			if ( args.Kind == ActivationKind.VoiceCommand )
			{
				var commandArgs = args as VoiceCommandActivatedEventArgs;
				SpeechRecognitionResult speechRecognitionResult = commandArgs.Result;
				string voiceCommandName = speechRecognitionResult.RulePath[0];

				foreach ( var item in speechRecognitionResult.RulePath )
				{
					Debug.WriteLine( item );
				}


				var command = speechRecognitionResult.Text;

				if ( string.Equals( voiceCommandName, "captainsLog" ) )
				{
					IReadOnlyList<string> captainsLogDictation;
					speechRecognitionResult.SemanticInterpretation.Properties
						.TryGetValue( "CaptainsLogDictation", out captainsLogDictation );
					string dictationText = captainsLogDictation.FirstOrDefault();

					_rootFrame.NavigateTo<RecordNotesPage>( dictationText );

				}
				else if ( string.Equals( voiceCommandName, "locationLog" ) )
				{
					_rootFrame.NavigateTo<LocationServicesPage>();
				}
				else
				{
					//ERROR: DID NOT FIND VOICE COMMAND
					_rootFrame.NavigateTo<MainPage>();
				}
			}

			Window.Current.Activate();
		}
	}
}
