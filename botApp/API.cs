using System.Net;
using System.Text;
using System.Web;
using botApp.Pages;
using botApp.Settings;

public class MyHttpRequestHandler
{   
    private string url;

    public MyHttpRequestHandler(string url) {
        this.url = url;
    }

    public void initAPI() {
        using (HttpListener listener = new HttpListener())
        {
            listener.Prefixes.Add(this.url);
            listener.Start();
            Console.WriteLine("Listening for requests at: " + this.url);

            while (true)
            {
                try
                {
                    HttpListenerContext context = listener.GetContext();
                    HandleRequest(context);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error handling request: " + ex.Message);
                }
            }
        }
    }

    private void HandleErrorResponse(HttpListenerContext context, Exception ex) {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        string errorMessage = "Error handling the request: " + ex.Message;
        byte[] errorBytes = Encoding.UTF8.GetBytes(errorMessage);
        context.Response.OutputStream.Write(errorBytes, 0, errorBytes.Length);
    }

    private void HandleDallERequest(HttpListenerContext context) {
        SlackToken token = SlackToken.GetObject();
        try
        {
            string requestData;
            using (var reader = new System.IO.StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
            {
                requestData = reader.ReadToEnd();
            }

            byte[] responseBytes = Encoding.UTF8.GetBytes(requestData);
            string responseString = Encoding.UTF8.GetString(responseBytes);
            var queryParams = HttpUtility.ParseQueryString(responseString);

            string postContent = queryParams["text"];
            string channelName = queryParams["channel_name"];

            string FilePath = FileSenderModel.useDallE(postContent);

            FileSenderModel.uploadFileToChannel(FilePath, token.BotToken, channelName);

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.ContentType = "text/plain";
            context.Response.ContentLength64 = responseBytes.LongLength;
            context.Response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
        }
        catch (Exception ex)
        {
            HandleErrorResponse(context, ex);
        }
        finally
        {
            context.Response.OutputStream.Close();
        }
    }

    private async void HandleChatRequest(HttpListenerContext context) {
        try
        {
            Console.WriteLine("This is raw URL  : " + context.Request.RawUrl);
            string requestData;
            using (var reader = new System.IO.StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
            {
                requestData = reader.ReadToEnd();
            }

            byte[] responseBytes = Encoding.UTF8.GetBytes(requestData);
            string responseString = Encoding.UTF8.GetString(responseBytes);
            var queryParams = HttpUtility.ParseQueryString(responseString);

            string textParameter = queryParams["text"];
            string textParameter2 = queryParams["channel_name"];
            var gptResponse = await GptTryingModel.useChat(textParameter);

            string SlackWebhookUrl;

            if (textParameter2 == "general")
            {
                SlackWebhookUrl = "https://hooks.slack.com/services/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";
                MessageSenderModel.sendMessageToChannel(gptResponse, SlackWebhookUrl);
            }
            if (textParameter2 == "budget")
            {
                SlackWebhookUrl = "https://hooks.slack.com/services/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";
                MessageSenderModel.sendMessageToChannel(gptResponse, SlackWebhookUrl);
            }
            if (textParameter2 == "random")
            {
                SlackWebhookUrl = "https://hooks.slack.com/services/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";
                MessageSenderModel.sendMessageToChannel(gptResponse, SlackWebhookUrl);
            }
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.ContentType = "text/plain";
            context.Response.ContentLength64 = responseBytes.LongLength;
            context.Response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
        }
        catch (Exception ex)
        {
            HandleErrorResponse(context, ex);
        }
        finally
        {

            context.Response.OutputStream.Close();
        }
    }

    public async void HandleRequest(HttpListenerContext context)
    {

        if (context.Request.HttpMethod != "POST")
        {
            context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
            byte[] errorBytes = Encoding.UTF8.GetBytes("Method not allowed.");
            context.Response.OutputStream.Write(errorBytes, 0, errorBytes.Length);
            context.Response.OutputStream.Close();
            return;
        }

        string rawUrl = context.Request.RawUrl;

        if (rawUrl == "/chat")
        {
            HandleChatRequest(context);
        }
        else if (rawUrl == "/dalle")
        {
            HandleDallERequest(context);
        }

    }
}
