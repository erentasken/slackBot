using System.Text;
using botApp.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace botApp.Pages
{
    public class GptTryingModel : PageModel
    {
        private SlackToken token;
        private string API_key;
        private OpenAI openai;
        public GptTryingModel()
        {
            token = SlackToken.GetObject();
            openai = OpenAI.GetObject();
            API_key = openai.openAiKey;
        }

        [BindProperty]
        public string Prompt { get; set; }
        public string ResultText { get; set; }

        public async Task OnPostUpdate()
        {
            Prompt = Request.Form["prompt"];
            ResultText = await useChat(Prompt);
        }

        public static async Task<string> useChat(string prompt) {
            var conversation = new[]
            {
                new { role = "system", content = "You are a helpful assistant." },
                new { role = "user", content = prompt },
            };

            // Create the JSON payload directly
            string jsonRequest = $@"{{
                ""model"": ""gpt-3.5-turbo"",
                ""messages"": {Newtonsoft.Json.JsonConvert.SerializeObject(conversation)},
                ""max_tokens"": 1500
            }}";

            // Prepare the HTTP request
            using (var httpClient = new HttpClient())
            {
                var keyObject = OpenAI.GetObject();
                
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {keyObject.openAiKey}");
                
                var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", new StringContent(jsonRequest, Encoding.UTF8, "application/json"));

                // Read the response and extract the completion result
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    dynamic completionResult = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse);
                    string resultText = completionResult.choices[0].message.content;
                    return resultText;
                }
                else
                {
                    Console.WriteLine("Error code: " + response.StatusCode);
                    return "Error code: " + response.StatusCode;
                }
            }
        }
    }
}