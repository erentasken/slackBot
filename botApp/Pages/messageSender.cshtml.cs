using Microsoft.AspNetCore.Mvc.RazorPages;
using Slack.Webhooks;
using SlackIntegration.Pages;

namespace botApp.Pages
{
    public class messageSenderModel : PageModel
    {
        static readonly System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();
        private string SlackWebhookUrl;
        private readonly ILogger<IndexModel> _logger;

        public messageSenderModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }


        private void sendMessageToChannel(string channelName, string message) {
            var slackClient = new SlackClient(SlackWebhookUrl);
            var slackMessage = new SlackMessage
            {
                Text = message,
                Channel = channelName + "#"
            };

            try
            {
                slackClient.Post(slackMessage);

            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }
        }

        public void OnPostEdit()
        {
            var message = Request.Form["message"];
            var channels= Request.Form["channelNames"];

            if (channels.Contains<string>("general")) {
                SlackWebhookUrl = "https://hooks.slack.com/services/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";
                sendMessageToChannel("general", message);
            }
            if (channels.Contains<string>("budget")) {
                SlackWebhookUrl = "https://hooks.slack.com/services/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";
                sendMessageToChannel("budget", message);
            }
            if (channels.Contains<string>("random"))
            {
                SlackWebhookUrl = "https://hooks.slack.com/services/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";
                sendMessageToChannel("random", message);
            }



            RedirectToPage("messageSender");
        }

        public async Task<HttpResponseMessage> requestFromApi()
        {
            string url = "https://slack.com/api/conversations.list";
            string apiToken = "xoxb-5526553258965-5543788954934-Wvwn8VFl5Y6MlrbyGvBtUkSi";
            httpClient.DefaultRequestHeaders.Remove("Authorization");
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiToken}");

            HttpResponseMessage response = await httpClient.GetAsync(url);

            return response;
        }

        public async Task<string[]> FetchChannelNamesAsync()
        {

            try
            {
                HttpResponseMessage response = await requestFromApi();
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(responseContent);

                    if (data.ok == "true")
                    {
                        string dataX = "";
                        foreach (var channel in data.channels)
                        {
                            dataX += channel.name + " ";
                        }

                        dataX = dataX.TrimEnd();
                        string[] channelsArray;
                        channelsArray = dataX.Split(" ");

                        return channelsArray;
                    }
                    else
                    {
                        Console.WriteLine("Error fetching channel names: " + data.error);
                    }
                }
                else
                {
                    Console.WriteLine("Error fetching channel names. Status code: " + response.StatusCode);
                }

                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + " exception handled");
            }
            return null;

        }
    }
}

