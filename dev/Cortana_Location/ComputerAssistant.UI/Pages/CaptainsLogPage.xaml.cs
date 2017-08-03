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
using ComputerAssistant.UI.BotCommunication;
using System.Collections.Generic;

namespace ComputerAssistant.UI.Pages
{
    public sealed partial class CaptainsLogPage : Page
    {
        private ComputerAssistantBot _bot;
        private CancellationTokenSource _cancellationSource;

        public CaptainsLogPage()
        {
            this.InitializeComponent();
        }

        public ObservableCollection<CaptainsLogEntry> LogEntries { get; } =
            new ObservableCollection<CaptainsLogEntry>();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _cancellationSource = new CancellationTokenSource();
            _bot = new ComputerAssistantBot();

            StartCaptainsLogBot();

            if (!LogEntries.Any())
            {
                AddLogEntry("Starting App");
            }

            if (e.Parameter != null)
            {
                AddLogEntry(e.Parameter.ToString());
            }
        }

        private async void StartCaptainsLogBot()
        {
            await _bot.StartConversation();
            while (!_cancellationSource.IsCancellationRequested)
            {
                var message = await _bot.CheckForReply();
                if (message == "When you're ready to report say: Captain's Log.")
                {
                    await SpeakText(message);
                    var result = await StartSpeechRecognition(new[] { "Captains Log" });
                    if (!_cancellationSource.IsCancellationRequested)
                    {
                        await _bot.SendMessage(result);
                    }
                }
                else if (message == "Go ahead captain." || message == "What would you like to add?")
                {
                    var result = await StartSpeechRecognitionWithUI("LogEntry", "LogEntryDictation", "Boldly going where no man or woman has gone before.", message);
                    if (!_cancellationSource.IsCancellationRequested)
                    {
                        await _bot.SendMessage(result);
                    }
                }
                else if (message == "Would you like to add more?")
                {
                    await SpeakText(message);
                    var result = await StartSpeechRecognition(new[] { "Yes", "No" });
                    if (!_cancellationSource.IsCancellationRequested)
                    {
                        await _bot.SendMessage(result);
                    }
                }
                else if (message.StartsWith("Your report captain:"))
                {
                    AddLogEntry(message.Substring("Your report captain: ".Length));
                    await SpeakText(message);
                    var result = await StartSpeechRecognition(new[] { "Captains Log" });
                    if (!_cancellationSource.IsCancellationRequested)
                    {
                        await _bot.SendMessage(result);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(message))
                {
                    await SpeakText(message);
                }
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        private async Task<string> StartSpeechRecognition(IEnumerable<string> commands)
        {
            SpeechRecognizer speechRecognizerCaptainsLogCommand = new SpeechRecognizer();

            ISpeechRecognitionConstraint commandConstraint = new SpeechRecognitionListConstraint(commands);
            speechRecognizerCaptainsLogCommand.Constraints.Add(commandConstraint);
            await speechRecognizerCaptainsLogCommand.CompileConstraintsAsync();

            string result = string.Empty;
            while (!_cancellationSource.IsCancellationRequested)
            {
                SpeechRecognitionResult commandResult = await speechRecognizerCaptainsLogCommand.RecognizeAsync();

                if (commandResult.Status != SpeechRecognitionResultStatus.Success
                    || commandResult.Confidence == SpeechRecognitionConfidence.Rejected
                    || _cancellationSource.IsCancellationRequested)
                {
                    continue;
                }

                result = commandResult.Text;
                break;
            }

            speechRecognizerCaptainsLogCommand.Dispose();
            return result;
        }

        private async Task<string> StartSpeechRecognitionWithUI(string topicHint, string tag, string exampleText, string audiblePrompt)
        {
            var captainsLogDictationRecognizer = new SpeechRecognizer();
            string result = string.Empty;

            ISpeechRecognitionConstraint dictationConstraint =
                new SpeechRecognitionTopicConstraint(
                    SpeechRecognitionScenario.Dictation, topicHint, tag);

            captainsLogDictationRecognizer.Constraints.Add(dictationConstraint);

            await captainsLogDictationRecognizer.CompileConstraintsAsync();

            captainsLogDictationRecognizer.UIOptions.ExampleText = exampleText;
            captainsLogDictationRecognizer.UIOptions.AudiblePrompt = audiblePrompt;
            captainsLogDictationRecognizer.UIOptions.IsReadBackEnabled = true;
            captainsLogDictationRecognizer.UIOptions.ShowConfirmation = true;

            while (!_cancellationSource.IsCancellationRequested)
            {
                SpeechRecognitionResult dictationResult = await captainsLogDictationRecognizer.RecognizeWithUIAsync();

                if (dictationResult.Status != SpeechRecognitionResultStatus.Success
                    || dictationResult.Confidence == SpeechRecognitionConfidence.Rejected
                    || string.IsNullOrWhiteSpace(dictationResult.Text)
                    || _cancellationSource.IsCancellationRequested)
                {
                    captainsLogDictationRecognizer.Dispose();

                    continue;
                }

                // Recognized user's dictation entry
                result = dictationResult.Text;
                break;
            }

            captainsLogDictationRecognizer.Dispose();
            return result;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            _cancellationSource.Cancel();
        }

        private void AddLogEntry(string logEntryText)
        {
            if (string.IsNullOrWhiteSpace(logEntryText))
            {
                return;
            }
            LogEntries.Insert(0, new CaptainsLogEntry(logEntryText));
        }

        private async Task SpeakText(string textToSpeak)
        {
            // The media object for controlling and playing audio.
            MediaElement mediaElement = TextToSpeechMediaElement;

            SpeechSynthesizer speechSynth = new SpeechSynthesizer();

            // Generate the audio stream from plain text.
            SpeechSynthesisStream stream = await speechSynth.SynthesizeTextToStreamAsync(textToSpeak);

            // Send the stream to the media object.
            mediaElement.SetSource(stream, stream.ContentType);
            mediaElement.Play();
        }
    }
}
