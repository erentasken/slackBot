using botApp.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using OpenAI_API;
using OpenAI_API.Completions;
using OpenAI_API.Models;
using Slack.NetStandard;
using Slack.NetStandard.RequestHandler;
using Slack.NetStandard.Socket;
using Slack.NetStandard.Interaction;
using System.Net.WebSockets;
using Slack.NetStandard.RequestHandler.Handlers;
using Slack.Webhooks;

namespace botApp.Pages
{
    public class gptTryingModel : PageModel
    {
        private string APIkey;
        private OpenAI openAi;

        public void OnGet()
        {
            openAi = OpenAI.GetObject();
            APIkey = openAi.openAiKey;
        }

        [BindProperty]
        public string Prompt { get; set; }
        public string ResultText { get; set; }

        public async Task OnPostAsync()
        {
            var openAI = new OpenAIAPI(APIkey);
            
            var completionRequest = new CompletionRequest
            {
                Prompt = Prompt,
                Model = Model.ChatGPTTurbo
            };

            var completions = await openAI.Completions.CreateCompletionAsync(completionRequest);

            foreach (var completion in completions.Completions)
            {
                ResultText = completion.Text;
            }
        }
    }
}