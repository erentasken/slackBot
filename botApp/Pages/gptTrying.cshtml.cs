using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenAI_API;
using OpenAI_API.Completions;
using OpenAI_API.Models;
using SlackNet.Blocks;
using SlackNet.Events;
using SlackNet.Interaction;
using SlackNet.WebApi;
using SlackNet;

namespace botApp.Pages
{
    public class gptTryingModel : PageModel
    {
        private string APIkey = "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";

        [BindProperty]
        public string Prompt { get; set; }
        public string ResultText { get; set; }

        public void OnGet()
        {
            // Initialize the Prompt if needed
            if (string.IsNullOrEmpty(Prompt))
            {
                Prompt = "how are you";
            }
        }

        public async Task OnPostAsync()
        {
            var openAI = new OpenAIAPI(APIkey);

            var completionRequest = new CompletionRequest
            {
                Prompt = Prompt,
                Model = Model.DavinciText
            };
            var completions = await openAI.Completions.CreateCompletionAsync(completionRequest);

            foreach (var completion in completions.Completions)
            {
                ResultText = completion.Text;
            }
        }
    }
}