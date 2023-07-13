using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Slack.Webhooks;
using SlackNet;

using JMS.UploadFile;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using Microsoft.Extensions.Logging;
using SlackNet.Events;
using SlackIntegration.Pages;
using static System.Net.WebRequestMethods;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Way.Lib;
using System.Net.Http;



namespace botApp.Pages
{
    public class messageSenderModel : PageModel
    {
        static readonly System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();
        // Replace with your own Slack webhook URL
        private string SlackWebhookUrl = "https://hooks.slack.com/services/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";
        private readonly ILogger<IndexModel> _logger;

        public messageSenderModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnPostEdit()
        {
            runForestRun();
            var message = Request.Form["message"];
            var channelNames = Request.Form["channelNames"];

            if(channelNames == "general")
            {
                SlackWebhookUrl = "https://hooks.slack.com/services/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";
            }

          

            var channelName = channelNames + "#";

            var slackClient = new SlackClient(SlackWebhookUrl);
            var slackMessage = new SlackMessage
            {
                Text = message,
                Channel = channelName 
            };

            try
            {
                slackClient.Post(slackMessage);

            }catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }

            _logger.LogInformation("text = " + message + " channelName = " + channelNames);
            _logger.LogInformation(slackMessage.Channel);
            
            RedirectToPage("messageSender");
        }

        public async Task runForestRun()
        {
            List<string> channelNames = await FetchChannelNamesAsync();

            if (channelNames != null)
            {
                _logger.LogInformation("Channel names:");
                foreach (string channel in channelNames)
                {
                    Console.WriteLine(channel);
                }

            }
        }

        async Task<List<string>> FetchChannelNamesAsync()
        {
            string url = "https://slack.com/api/conversations.list";
            string types = "public_channel,private_channel";
            string apiToken = "xoxb-5526553258965-5543788954934-Wvwn8VFl5Y6MlrbyGvBtUkSi";
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiToken}");


            string requestUrl = $"{url}?types={types}";

            HttpResponseMessage response = await httpClient.GetAsync(requestUrl);
            string responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(responseContent);
                if (data.ok == "true") {
                    _logger.LogInformation("bastirdim gitti");
                    List<string> channelNames = new List<string>();
                    _logger.LogInformation($"{data.channels}");

                    int i = 0;
                    foreach (var channel in data.channels)
                    {
                        _logger.LogInformation("hehehehe");
                        i++;
                        channelNames.Add(channel.name);

                    }


                    _logger.LogInformation($"{i}");



                    return channelNames;
                }
                else
                {
                        _logger.LogInformation("bastirmadim gitti");
                        Console.WriteLine("Error fetching channel names: " + data.error);
                }
            }
            else
            {
                Console.WriteLine("Error fetching channel names. Status code: " + response.StatusCode);
            }

            return null;
        }
    }
}

