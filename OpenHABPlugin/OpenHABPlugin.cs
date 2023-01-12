namespace Loupedeck.OpenHABPlugin
{
    using System;

    // This class contains the plugin-level logic of the Loupedeck plugin.

    public class OpenHABPlugin : Plugin
    {
        // Gets a value indicating whether this is an Universal plugin or an Application plugin.
        public override Boolean UsesApplicationApiOnly => true;

        // Gets a value indicating whether this is an API-only plugin.
        public override Boolean HasNoApplication => true;

        public OpenHABService OHService { get; set; }
        protected string _baseUrl = "http://192.168.40.15:8080";

        // This method is called when the plugin is loaded during the Loupedeck service start-up.
        public override void Load()
        {
            OHService = new OpenHABService(_baseUrl);
        }

        // This method is called when the plugin is unloaded during the Loupedeck service shutdown.
        public override void Unload()
        {
        }
    }
}
