using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Slack.Webhooks;
using SlackNet;

using JMS.UploadFile;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using Microsoft.Extensions.Logging;
using SlackIntegration.Pages;

namespace botApp.Pages
{
    public class fileSenderModel : PageModel
    {
        //private const string SlackWebhookUrl = "https://hooks.slack.com/services/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";
        private readonly ILogger<IndexModel> _logger;

        public fileSenderModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public async void OnPostUpdate()
        {
            const string botUserOAuthToken = "xoxb-5526553258965-5543788954934-Wvwn8VFl5Y6MlrbyGvBtUkSi"; //modify this one

            var api = new SlackServiceBuilder()
                .UseApiToken(botUserOAuthToken)
                .GetApiClient();

            var channelName = Request.Form["channelName"];
            var fileName = Request.Form["fileName"];
            var fileContent = Request.Form["fileContent"];
            var fileExtension = Request.Form["fileExtension"];
            var fileTitle = Request.Form["fileTitle"];
            var fileComment = Request.Form["fileComment"];

            try
            {
                var result = await api.Files.Upload(fileContent, fileExtension, fileName + fileExtension, fileTitle, fileComment, null, new List<string>() { channelName });

            }catch(Exception e)
            {
                _logger.LogInformation(e.Message);
            }

            RedirectToPage("fileSender");
        }
    }
}
