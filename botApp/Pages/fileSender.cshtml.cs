using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Slack.Webhooks;
using SlackNet;

using JMS.UploadFile;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using Microsoft.Extensions.Logging;
using SlackIntegration.Pages;
using RestSharp;

namespace botApp.Pages
{
    public class fileSenderModel : PageModel
    {
        static readonly System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();
        //private const string SlackWebhookUrl = "https://hooks.slack.com/services/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";
        private readonly ILogger<IndexModel> _logger;

        public fileSenderModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        private async void sendFileToChannel(string channelName) {
            const string botUserOAuthToken = "xoxb-5526553258965-5543788954934-Wvwn8VFl5Y6MlrbyGvBtUkSi"; //modify this one

            var api = new SlackServiceBuilder()
                .UseApiToken(botUserOAuthToken)
                .GetApiClient();

            var fileName = Request.Form["fileName"];
            var fileContent = Request.Form["fileContent"];
            var fileExtension = Request.Form["fileExtension"];
            var fileTitle = Request.Form["fileTitle"];
            var fileComment = Request.Form["fileComment"];

            try
            {
                var result = await api.Files.Upload(fileContent, fileExtension, fileName + fileExtension, fileTitle, fileComment, null, new List<string>() { channelName });
            }
            catch (Exception e)
            {
                _logger.LogInformation(e.Message);
            }
        }

        public void OnPostUploadFile()
        {
            var fileName = Request.Form["fileNameFirst"];
            var fileExtension = Request.Form["fileExtensionFirst"];
            string fileFullName = fileName + "." + fileExtension;
            string filePath = @"C:\Users\erenn\Masaüstü\slackBot\filesForUpload\" + fileFullName;
            string token = "xoxb-5526553258965-5543788954934-Wvwn8VFl5Y6MlrbyGvBtUkSi";

            var channels = Request.Form["channelNamesFirst"];
            if (channels.Contains<string>("general"))
            {
                uploadFileToChannel(filePath, token, "general");
            }
            if (channels.Contains<string>("budget"))
            {
                uploadFileToChannel(filePath, token, "budget");
            }
            if (channels.Contains<string>("random"))
            {
                uploadFileToChannel(filePath, token, "random");
            }

            
        }

        private void uploadFileToChannel(string filePath, string token, string channel)
        {
            var client = new RestClient("https://slack.com/api/files.upload");
            var request = new RestRequest("https://slack.com/api/files.upload", Method.Post);
            request.AddParameter("token", token);
            request.AddParameter("channels", channel);
            request.AddFile("file", filePath);

            var response = client.Execute(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine("File uploaded successfully!");
            }
            else
            {
                Console.WriteLine("Error uploading file: " + response.Content);
            }
        }



        public async void OnPostUpdate()
        {
            var channels = Request.Form["channelNames"];
            if (channels.Contains<string>("general"))
            {
                sendFileToChannel("general");
            }
            if (channels.Contains<string>("budget"))
            {
                sendFileToChannel("budget");
            }
            if (channels.Contains<string>("random"))
            {
                sendFileToChannel("random");
            }

            

            RedirectToPage("fileSender");
        }

        public async Task<string[]> FetchChannelNamesAsync()
        {
            try
            {
                string url = "https://slack.com/api/conversations.list";
                string types = "public_channel,private_channel";
                string apiToken = "xoxb-5526553258965-5543788954934-Wvwn8VFl5Y6MlrbyGvBtUkSi";
                httpClient.DefaultRequestHeaders.Remove("Authorization");
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiToken}");

                string requestUrl = $"{url}?types={types}";
                HttpResponseMessage response = await httpClient.GetAsync(requestUrl);
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
                Console.WriteLine(e.Message);
            }
            return null;

        }
    }
}



