using System;
using System.Linq;
using Microsoft.Bot.Connector.DirectLine;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Diagnostics;

namespace ComputerAssistant.UI.BotCommunication
{
    public class ComputerAssistantBot
    {
        private const string BotId = "computer-assistant";
        private DirectLineClient _client;
        private Conversation _conversation;
        private string _lastWatermark;

        public ComputerAssistantBot()
        {
            _client = new DirectLineClient(CredsProvider.DirectLineKey);
        }

        public async Task StartConversation()
        {
            _conversation = await _client.Conversations.StartConversationAsync();
        }

        public async Task SendMessage(string message)
        {
            var activity = new Activity
            {
                From = new ChannelAccount("Captain"),
                Text = message,
                Type = ActivityTypes.Message
            };

            await _client.Conversations.PostActivityAsync(_conversation.ConversationId, activity);
        }

        public async Task<string> CheckForReply()
        {
            var activitySet =
                await _client.Conversations.GetActivitiesAsync(_conversation.ConversationId, _lastWatermark);

            if (string.IsNullOrWhiteSpace(activitySet.Watermark))
            {
                return string.Empty;
            }

            _lastWatermark = activitySet.Watermark;
            var activity = activitySet.Activities.LastOrDefault(a => a.From.Id == BotId);

            string message = activity?.Text ?? string.Empty;
            if (activity.Attachments.Any())
            {
                try
                {
                    var prompt = JsonConvert.DeserializeObject<Prompt>(activity.Attachments.First().Content.ToString());

                    message = prompt.text;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            return message;
        }

        public class Button
        {
            public string type { get; set; }
            public string title { get; set; }
            public string value { get; set; }
        }

        public class Prompt
        {
            public string text { get; set; }
            public List<Button> buttons { get; set; }
        }
    }
}
