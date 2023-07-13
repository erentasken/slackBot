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

namespace botApp.Pages
{
    public class messageSenderModel : PageModel
    {
        // Replace with your own Slack webhook URL
        private const string SlackWebhookUrl = "https://hooks.slack.com/services/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";
        private readonly ILogger<IndexModel> _logger;

        public messageSenderModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnPostEdit()
        {
            var message = Request.Form["message"];
            var channelNames = Request.Form["channelNames"];

            var channelName = channelNames;

            var slackClient = new SlackClient(SlackWebhookUrl);
            var slackMessage = new SlackMessage
            {
                Text = message,
                Channel = "#" + channelName
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
    }
}
