namespace botApp.Settings
{
    public class SlackToken
    {
        public SlackToken(string appToken, string botToken, string signingSecret)
        {
            AppToken = appToken;
            BotToken = botToken;
            SigningSecret = signingSecret;
        }

        public string AppToken { get; set; }
        public string BotToken { get; set; }
        public string SigningSecret { get; set; }

        // Static method to create and return a Token object with configuration values


        private static string setSettingsPath()
        {
            string rootDirectory = AppContext.BaseDirectory;
            string parentDirectory = rootDirectory;

            // Loop to navigate up the directory tree 4 levels (adjust if needed)
            for (int i = 0; i < 4; i++)
            {
                parentDirectory = Directory.GetParent(parentDirectory).FullName;
            }

            string settingsPath = Path.Combine(parentDirectory, "Settings", "appsettings.json");
            return settingsPath;
        }

        public static SlackToken GetObject()
        {
            // Read token values from the configuration
            string settingPath;
            settingPath = setSettingsPath();

            var config = new ConfigurationBuilder()
            .AddJsonFile(path: settingPath, true)
            .Build();

            var botToken = config.GetSection("Slack:BotToken").Value;
            var signingSecret = config.GetSection("Slack:SigningSecret").Value;
            var appToken = config.GetSection("Slack:AppToken").Value;
            // Create a new Token object with the values obtained from the configuration
            return new SlackToken(appToken, botToken, signingSecret);
        }
    }


}
