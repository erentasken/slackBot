using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Slack.Webhooks;

namespace SlackIntegration.Pages
{
    public class IndexModel : PageModel
    {
        // Replace with your own Slack webhook URL
        private const string SlackWebhookUrl = "https://hooks.slack.com/services/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";

        public IActionResult OnPostEdit()
        {
            var slackClient = new SlackClient(SlackWebhookUrl);

            var slackMessage = new SlackMessage
            {
                Text = "ASP.NET Core!",
                Channel = "#budget"
            };

            slackClient.Post(slackMessage);

            return RedirectToPage("Index");
            //return new OkResult();
        }
    }
}