using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Slack.Webhooks;
using SlackNet;


using SlackNet;
using JMS.UploadFile;
using System.Runtime.CompilerServices;

namespace SlackIntegration.Pages
{
    public class IndexModel : PageModel
    {
        // Replace with your own Slack webhook URL
        private const string SlackWebhookUrl = "https://hooks.slack.com/services/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";

        public void OnPostEdit()
        {
            var slackClient = new SlackClient(SlackWebhookUrl);
            var slackMessage = new SlackMessage
            {
                Text = "ASP.NET Core!",
                Channel = "#budget"
            };

            slackClient.Post(slackMessage);

            RedirectToPage("Index");
        }

        public async void OnPostUpdate() {
            const string botUserOAuthToken = "xoxb-5526553258965-5543788954934-Wvwn8VFl5Y6MlrbyGvBtUkSi"; //modify this one
            const string slackChannel = "#general";

            const string fileName = "dsa";
            const string fileExtension = ".gif";
            const string title = "A Customer Report";
            const string comment = "Attached is a customer's report";

            var api = new SlackServiceBuilder()
                .UseApiToken(botUserOAuthToken)
                .GetApiClient();

            var result = await api.Files.Upload("bayildi", fileExtension, fileName + fileExtension, title, comment, null, new List<string>() { slackChannel });

            RedirectToPage("Index");
        }
    }
}