namespace botApp.Settings
{
    public class OpenAI
    {
        public OpenAI(string openAiKey)
        {
            this.openAiKey = openAiKey;
        }

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

        public static OpenAI GetObject()
        {
            string settingPath;
            settingPath = setSettingsPath();
            
            var config = new ConfigurationBuilder()
            .AddJsonFile(path: settingPath, true)
            .Build();

            var openAiKey = config.GetSection("OpenAI:ApiToken").Value;
            return new OpenAI(openAiKey);
        }

        public string openAiKey { get; set; }
    }


}
