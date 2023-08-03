using System.Text;
using botApp.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using Rystem.OpenAi;

namespace botApp.Pages
{
    public class gptTryingModel : PageModel
    {
        private SlackToken token;
        private string API_key;
        private OpenAI openai;
        public gptTryingModel()
        {
            Console.WriteLine("i am up ");
            

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
            var conversation = new[]
            {
                new { role = "system", content = "You are a helpful assistant." },
                //new { role = "user", content = "Who won the world series in 2020?" },
                new { role = "user", content = Prompt },
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
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {API_key}");

                // Send the POST request to the API
                var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", new StringContent(jsonRequest, Encoding.UTF8, "application/json"));

                // Read the response and extract the completion result
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    dynamic completionResult = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse);
                    string resultText = completionResult.choices[0].message.content;
                    Console.WriteLine("This is the result : " + resultText);
                    ResultText = resultText;
                    return;
                }
                else
                {
                    // Handle error response
                    Console.WriteLine("Error code: " + response.StatusCode);
                    return;
                }
            }
        }

        public static async Task<string> useChat(string prompt) {
            var conversation = new[]
            {
                new { role = "system", content = "You are a helpful assistant." },
                //new { role = "user", content = "Who won the world series in 2020?" },
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
                //httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {API_key}");
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer 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");


                
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