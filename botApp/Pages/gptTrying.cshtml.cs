using botApp.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenAI_API;
using OpenAI_API.Completions;
using OpenAI_API.Models;

namespace botApp.Pages
{
    public class gptTryingModel : PageModel
    {
        private string APIkey;
        private OpenAI openAi;

        public void OnGet()
        {
            openAi = OpenAI.GetObject();
            APIkey = openAi.openAiKey;ff
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