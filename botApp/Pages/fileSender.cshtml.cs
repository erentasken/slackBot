using botApp.Settings;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using RestSharp;
using SlackIntegration.Pages;
using SlackNet;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;

namespace botApp.Pages
{
    public class FileSenderModel : PageModel
    {
        static readonly HttpClient httpClient = new HttpClient();
        private readonly ILogger<IndexModel> _logger;
        private SlackToken token;
        private string API_key;
        private OpenAI openai;
        public FileSenderModel()
        {
            token = SlackToken.GetObject();
            openai = OpenAI.GetObject();
            API_key = openai.openAiKey;
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

        public static void uploadFileToChannel(string filePath, string token, string channel)
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

        private static RestResponse requestFromApi(string filePath, string token, string channel)
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
        
        public void OnPostUploadImage()
        {
            var prompt = Request.Form["prompt"];
            string url = "https://api.openai.com/v1/images/generations";
            string bearerToken = API_key;
            Json_Convert json1 = new Json_Convert();
            json1.prompt = prompt;
            json1.n = 1;
            json1.size = "256x256";
            json1.response_format = "b64_json";
            string body = JsonConvert.SerializeObject(json1);
            //string body = "{\"prompt\": \"an isometric view of a miniature city, tilt shift, bokeh, voxel, vray render, high detail\",\"n\": 1,\"size\": \"256x256\",\"response_format\":\"b64_json\"}";

            // Prepare data for the POST request
            var data = Encoding.ASCII.GetBytes(body);
            Console.WriteLine(data);

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = data.Length;

            // Authentication
            if (bearerToken != null)
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                request.PreAuthenticate = true;
                request.Headers.Add("Authorization", "Bearer " + bearerToken);
            }
            else
            {
                request.Credentials = CredentialCache.DefaultCredentials;
            }

            // Perform request
            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            // Retrieve response
            var response = (HttpWebResponse)request.GetResponse();
            Console.WriteLine("Request response: " + response.StatusCode);

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();


            // Deserialize JSON
            dynamic responseJson = JsonConvert.DeserializeObject<dynamic>(responseString);

            var channels = Request.Form["channelNamesSecond"];

            string base64 = responseJson.data[0].b64_json;

            Bitmap img = Base64StringToBitmap(base64);

            long unixTime = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
            string filename = string.Format("image_{0}_{1}.png", unixTime, 0);

            img.Save(filename);
            Console.WriteLine("Saving image to " + filename);
            // Get the project's root directory path dynamically
            string rootDirectory = AppContext.BaseDirectory;

            string parentDirectory = Directory.GetParent(Directory.GetParent(rootDirectory).FullName).FullName;
            parentDirectory = Directory.GetParent(parentDirectory).FullName;
            parentDirectory = Directory.GetParent(parentDirectory).FullName;
            parentDirectory = Directory.GetParent(parentDirectory).FullName;

            string FilePath = parentDirectory + @"\filesForUpload" + @"\" + filename;

            System.IO.File.Move(filename, FilePath);
            uploadFileToChannel(FilePath, token.BotToken, channels);
            Console.WriteLine(channels);
        }

        public static string useDallE(string prompt) {
            var keyObject = OpenAI.GetObject();
            string url = "https://api.openai.com/v1/images/generations";
            string bearerToken = keyObject.openAiKey;
            Json_Convert json1 = new Json_Convert();
            json1.prompt = prompt;
            json1.n = 1;
            json1.size = "256x256";
            json1.response_format = "b64_json";
            string body = JsonConvert.SerializeObject(json1);
            //string body = "{\"prompt\": \"an isometric view of a miniature city, tilt shift, bokeh, voxel, vray render, high detail\",\"n\": 1,\"size\": \"256x256\",\"response_format\":\"b64_json\"}";

            // Prepare data for the POST request
            var data = Encoding.ASCII.GetBytes(body);
            Console.WriteLine(data);

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = data.Length;

            // Authentication
            if (bearerToken != null)
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                request.PreAuthenticate = true;
                request.Headers.Add("Authorization", "Bearer " + bearerToken);
            }
            else
            {
                request.Credentials = CredentialCache.DefaultCredentials;
            }

            // Perform request
            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            // Retrieve response
            var response = (HttpWebResponse)request.GetResponse();
            Console.WriteLine("Request response: " + response.StatusCode);

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();


            // Deserialize JSON
            dynamic responseJson = JsonConvert.DeserializeObject<dynamic>(responseString);

            //var channels = Request.Form["channelNamesSecond"];
            string base64 = responseJson.data[0].b64_json;

            Bitmap img = Base64StringToBitmap(base64);

            long unixTime = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
            string filename = string.Format("image_{0}_{1}.png", unixTime, 0);

            img.Save(filename);
            Console.WriteLine("Saving image to " + filename);
            // Get the project's root directory path dynamically
            string rootDirectory = AppContext.BaseDirectory;

            string parentDirectory = Directory.GetParent(Directory.GetParent(rootDirectory).FullName).FullName;
            parentDirectory = Directory.GetParent(parentDirectory).FullName;
            parentDirectory = Directory.GetParent(parentDirectory).FullName;
            parentDirectory = Directory.GetParent(parentDirectory).FullName;

            string FilePath = parentDirectory + @"\filesForUpload" + @"\" + filename;

            System.IO.File.Move(filename, FilePath);

            return FilePath;
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

        public static Bitmap Base64StringToBitmap(string base64String)
        {
            Bitmap bmpReturn = null;

            byte[] byteBuffer = Convert.FromBase64String(base64String);
            MemoryStream memoryStream = new MemoryStream(byteBuffer);

            memoryStream.Position = 0;

            bmpReturn = (Bitmap)Bitmap.FromStream(memoryStream);

            memoryStream.Close();
            memoryStream = null;
            byteBuffer = null;

            return bmpReturn;
        }
    }
}



