namespace Loupedeck.OpenHABPlugin
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Newtonsoft.Json.Linq;

    // This class contains the plugin-level logic of the Loupedeck plugin.

    public class OpenHABPlugin : Plugin
    {
        // Gets a value indicating whether this is an Universal plugin or an Application plugin.
        public override Boolean UsesApplicationApiOnly => true;

        // Gets a value indicating whether this is an API-only plugin.
        public override Boolean HasNoApplication => true;

        public OpenHABService OHService { get; } = new OpenHABService();
        protected string _baseUrl = "http://localhost:8080";
        protected string _apiToken = "";

        // This method is called when the plugin is loaded during the Loupedeck service start-up.
        public override void Load()
        {
            Console.WriteLine("OH Plugin loading");
            var pluginDataDirectory = this.GetPluginDataDirectory();
            if (IoHelpers.EnsureDirectoryExists(pluginDataDirectory))
            {
                var filePath = Path.Combine(pluginDataDirectory, "config.json");
                JObject jsonData;

                try
                {
                    jsonData = JObject.Parse(File.ReadAllText(filePath));
                    if (jsonData["url"] != null)
                    {
                        _baseUrl = jsonData["url"]!.ToString();
                    }
                    if (jsonData["token"] != null)
                    {
                        _apiToken = jsonData["token"].ToString();
                    }
                    Console.WriteLine("Tokens read from file: " + _baseUrl + ", " + _apiToken);
                }
                catch (Exception ex)
                {
                    // Handle exception if there is any error while reading or parsing the JSON file
                    Console.WriteLine("Error reading JSON file: " + ex.Message);
                }
            }

            OHService.Initialize(_baseUrl, _apiToken);
        }

        // This method is called when the plugin is unloaded during the Loupedeck service shutdown.
        public override void Unload()
        {
        }
    }
}
