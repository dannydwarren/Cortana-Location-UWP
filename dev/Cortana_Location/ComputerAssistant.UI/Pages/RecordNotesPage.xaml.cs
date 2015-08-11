using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using ComputerAssistant.UI.Models;
using System;
using Windows.Media.SpeechSynthesis;
using Windows.Media.SpeechRecognition;
using System.Threading;
using System.Threading.Tasks;

namespace ComputerAssistant.UI.Pages
{
	public sealed partial class RecordNotesPage : Page
	{
		CancellationTokenSource cancellationSource;
		public RecordNotesPage()
		{
			this.InitializeComponent();
		}

		public ObservableCollection<CaptainsLogEntry> LogEntries { get; } =
			new ObservableCollection<CaptainsLogEntry>();

		protected override void OnNavigatedTo( NavigationEventArgs e )
		{
			base.OnNavigatedTo( e );

			cancellationSource = new CancellationTokenSource();

			StartVoiceRecognition();

			if ( !LogEntries.Any() )
			{
				//Seed for this run
				AddLogEntry( "Starting App" );

				//TODO: Go to disk and get the file.
			}

			if ( e.Parameter != null )
			{
				AddLogEntry( e.Parameter.ToString() );
			}

		}

		SpeechRecognizer speechRecognizerCaptainsLogCommand;

		private async void StartVoiceRecognition()
		{
			await SpeakText( "Say Captains Log at any time to create a log entry." );

			speechRecognizerCaptainsLogCommand = new SpeechRecognizer();

			while ( !cancellationSource.IsCancellationRequested )
			{
				// Listen for user to say "Captains Log"
				ISpeechRecognitionConstraint commandConstraint = 
					new SpeechRecognitionListConstraint( new[] { "Captains Log", "Computer Captains Log" } );
				speechRecognizerCaptainsLogCommand.Constraints.Add( commandConstraint );
				await speechRecognizerCaptainsLogCommand.CompileConstraintsAsync();

				SpeechRecognitionResult commandResult = await speechRecognizerCaptainsLogCommand.RecognizeAsync();

				if ( commandResult.Status != SpeechRecognitionResultStatus.Success
					|| commandResult.Confidence == SpeechRecognitionConfidence.Rejected
					|| cancellationSource.IsCancellationRequested )
				{
					continue;
				}
				// Recognized user saying "Captains Log"

				// Listen for the user's dictation entry
				var captainsLogDictationRecognizer = new SpeechRecognizer();

				ISpeechRecognitionConstraint dictationConstraint = 
					new SpeechRecognitionTopicConstraint( 
						SpeechRecognitionScenario.Dictation, "LogEntry", "LogEntryDictation" );

				captainsLogDictationRecognizer.Constraints.Add( dictationConstraint );

				await captainsLogDictationRecognizer.CompileConstraintsAsync();

				captainsLogDictationRecognizer.UIOptions.ExampleText = "Boldly going where no man or woman has gone before.";
				captainsLogDictationRecognizer.UIOptions.AudiblePrompt = "Go ahead";
				captainsLogDictationRecognizer.UIOptions.IsReadBackEnabled = true;
				captainsLogDictationRecognizer.UIOptions.ShowConfirmation = true;

				SpeechRecognitionResult dictationResult = await captainsLogDictationRecognizer.RecognizeWithUIAsync();

				if ( dictationResult.Status != SpeechRecognitionResultStatus.Success
					|| dictationResult.Confidence == SpeechRecognitionConfidence.Rejected
					|| string.IsNullOrWhiteSpace( dictationResult.Text )
					|| cancellationSource.IsCancellationRequested )
				{
					captainsLogDictationRecognizer.Dispose();

					continue;
				}
				// Recognized user's dictation entry

				AddLogEntry( dictationResult.Text );

				captainsLogDictationRecognizer.Dispose();
			}

			speechRecognizerCaptainsLogCommand.Dispose();
		}

		protected override void OnNavigatedFrom( NavigationEventArgs e )
		{
			base.OnNavigatedFrom( e );

			cancellationSource.Cancel();
		}

		private void AddLogEntry( string logEntryText )
		{
			if ( string.IsNullOrWhiteSpace( logEntryText ) )
			{
				return;
			}
			LogEntries.Insert( 0, new CaptainsLogEntry( logEntryText ) );
		}

		private async Task SpeakText( string textToSpeak )
		{
			SpeechSynthesizer speechSynth = new SpeechSynthesizer();

			// The media object for controlling and playing audio.
			MediaElement mediaElement = TextToSpeechMediaElement;

			// Generate the audio stream from plain text.
			SpeechSynthesisStream stream = await speechSynth.SynthesizeTextToStreamAsync( textToSpeak );

			// Send the stream to the media object.
			mediaElement.SetSource( stream, stream.ContentType );
			mediaElement.Play();
		}
	}
}
