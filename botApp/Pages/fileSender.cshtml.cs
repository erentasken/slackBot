using botApp.Settings;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RestSharp;
using SlackIntegration.Pages;
using SlackNet;
using System.IO;

namespace botApp.Pages
{
    public class fileSenderModel : PageModel
    {
        static readonly HttpClient httpClient = new HttpClient();
        private readonly ILogger<IndexModel> _logger;
        private SlackToken token;
        public fileSenderModel()
        {
            token = SlackToken.GetObject();
        }

        public void OnPostUpdate()
        {
            var channels = Request.Form["channelNames"];
            sendFileToChannel(channels);

            RedirectToPage("fileSender");
        }

        public void OnPostUploadFile()
        {
            IFormFile postedFile = Request.Form.Files["file12"];
            string fileFullName = postedFile.FileName;
            
            // Get the project's root directory path dynamically
            string rootDirectory = AppContext.BaseDirectory;
            string filesDirectory = "filesForUpload";
            string filePath = Path.Combine(rootDirectory, filesDirectory, fileFullName);

            string parentDirectory = Directory.GetParent(rootDirectory).FullName;
            parentDirectory = Directory.GetParent(Directory.GetParent(rootDirectory).FullName).FullName;
            parentDirectory = Directory.GetParent(parentDirectory).FullName;
            parentDirectory = Directory.GetParent(parentDirectory).FullName;
            parentDirectory = Directory.GetParent(parentDirectory).FullName;

            filePath = parentDirectory + @"\filesForUpload" +@"\" + fileFullName;
            
            var channels = Request.Form["channelNamesFirst"];
 
            uploadFileToChannel(filePath, token.BotToken, channels);
        }

        private async void sendFileToChannel(string channelName)
        {
            var api = new SlackServiceBuilder()
                .UseApiToken(token.BotToken)
                .GetApiClient();

            var fileName = Request.Form["fileName"];
            var fileContent = Request.Form["fileContent"];
            var fileExtension = Request.Form["fileExtension"];
            var fileTitle = Request.Form["fileTitle"];
            var fileComment = Request.Form["fileComment"];

            try
            {
                var result = await api.Files.Upload(fileContent, fileExtension, fileName + "." + fileExtension, fileTitle, fileComment, null, new List<string>() { channelName });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void uploadFileToChannel(string filePath, string token, string channel)
        {
            var response = requestFromApi(filePath, token, channel);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine("File uploaded successfully!");
            }
            else
            {
                Console.WriteLine("Error uploading file: " + response.Content);
            }
        }

        private RestResponse requestFromApi(string filePath, string token, string channel)
        {
            var client = new RestClient("https://slack.com/api/files.upload");
            var request = new RestRequest("https://slack.com/api/files.upload", Method.Post);
            request.AddParameter("token", token);
            request.AddParameter("channels", channel);
            
            try
            {
                request.AddFile("file", filePath);
            }catch(FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
            var response = client.Execute(request);

            return response;
        }

        public async Task<string[]> FetchChannelNamesAsync()
        {
            try
            {
                string url = "https://slack.com/api/conversations.list";
                httpClient.DefaultRequestHeaders.Remove("Authorization");
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.BotToken}");

                HttpResponseMessage response = await httpClient.GetAsync(url);
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
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;

        }
    }
}



