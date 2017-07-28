using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System;
using System.Threading;

namespace ComputerAssistant.Bot.Controllers
{
    [BotAuthentication]
    public class ComputerController : ApiController
    {
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message
                || activity.Type == ActivityTypes.ConversationUpdate)
            {
                await Conversation.SendAsync(activity, () => new RootDialog());
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }


        [Serializable]
        public class RootDialog : IDialog<object>
        {
            public async Task StartAsync(IDialogContext context)
            {
                await context.PostAsync("When you're ready to report say: Captain's Log.");

                context.Wait(MessageReceivedAsync);
            }

            private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
            {
                var message = await argument;

                if (!string.IsNullOrWhiteSpace(message.Text)
                    && (message.Text.ToLower().Contains("captain's log")
                    || message.Text.ToLower().Contains("captains log")))
                {
                    context.Call(new CaptainsLogDialog(), OnResume);
                }
                else
                {
                    context.Wait(MessageReceivedAsync);
                }
            }

            private async Task OnResume(IDialogContext context, IAwaitable<object> result)
            {
                var resultFromLog = await result;

                await context.PostAsync($"Your report captain: {resultFromLog}");

                context.Wait(MessageReceivedAsync);
            }
        }

        [Serializable]
        public class CaptainsLogDialog : IDialog<string>
        {
            private string _logEntry;

            public async Task StartAsync(IDialogContext context)
            {
                await context.PostAsync("Go ahead captain.");

                context.Wait(MessageReceivedAsync);
            }

            private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
            {
                var response = await result;
                _logEntry = _logEntry + " " + response.Text;

                await context.PostAsync($"Your entry so far: {_logEntry}.");

                PromptDialog.Confirm(context, OnResponse, "Would you like to add more?");
            }

            private async Task OnResponse(IDialogContext context, IAwaitable<bool> result)
            {
                var response = await result;

                if (response)
                {
                    await context.PostAsync("What would you like to add?");

                    context.Wait(MessageReceivedAsync);
                }
                else
                {
                    context.Done(_logEntry);
                }
            }
        }
    }
}
