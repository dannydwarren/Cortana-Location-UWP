using System.Linq;
using Microsoft.Bot.Connector.DirectLine;
using System.Threading.Tasks;
using System.Collections.Generic;

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

        public async Task<IEnumerable<string>> StartConversation()
        {
            _conversation = await _client.Conversations.StartConversationAsync();
            return await ReceiveMessage();
        }

        public async Task<IEnumerable<string>> SendMessage(string message)
        {
            await _client.Conversations.PostActivityAsync(_conversation.ConversationId, new Activity
            {
                Action = ActionTypes.Call,
                Text = message,
                From = new ChannelAccount("ComputerAssistantUI")
            });

            return await ReceiveMessage();
        }

        private async Task<IEnumerable<string>> ReceiveMessage()
        {
            var activitySet = await _client.Conversations.GetActivitiesAsync(_conversation.ConversationId);
            if (string.IsNullOrWhiteSpace(activitySet.Watermark))
            {
                return new List<string>();
            }
            _lastWatermark = activitySet.Watermark;
            var messages = activitySet.Activities.Where(a => a.Id == BotId).Select(a => a.Text).ToList();

            return messages;
        }
    }
}
