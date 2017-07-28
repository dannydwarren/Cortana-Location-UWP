using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System;

namespace ComputerAssistant.Bot.Controllers
{
    [BotAuthentication]
    public class ComputerController : ApiController
    {
        /// <summary>
        /// Endpoint: https://api.cognitive.microsoft.com/sts/v1.0
        /// //DO NOT COMMIT KEYS!!!!
        /// Key 1: 47180e9a7bb848c38bfce05a7ada93a7
        /// 
        /// Key 2: 049c54f2c9b6441ca257495168246454
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, () => new CaptainsLogDialog());
            }
           
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }


        [Serializable]
        public class CaptainsLogDialog : IDialog<object>
        {
            public Task StartAsync(IDialogContext context)
            {
                context.Wait(MessageReceivedAsync);
                return Task.CompletedTask;
            }

            private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
            {
                var message = await argument;
                await context.PostAsync($"Logged: {message.Text}");
                context.Wait(MessageReceivedAsync);
            }
        }
    }
}
