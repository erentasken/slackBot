using System.Net;
using System.Xml.Linq;
using SlackNet;

using SlackNet;
using JMS.UploadFile;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddRazorPages();
var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();



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



app.Run();