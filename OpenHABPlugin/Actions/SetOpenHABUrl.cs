using System;
namespace Loupedeck.OpenHABPlugin.Actions
{
    public class SetOpenHABUrl : PluginDynamicCommand
    {
        /// <summary>
        /// Reference to openHAB plugin
        /// </summary>
        protected OpenHABPlugin _ohPlugin=> ((OpenHABPlugin)this.Plugin);

        public SetOpenHABUrl() : base(displayName: "Set OpenHAB URL", description: "Use this command to configure the OpenHAB URL", groupName: "Settings")
        {
            this.MakeProfileAction("text;Enter OpenHAB URL:");
        }

        protected override void RunCommand(String actionParameter)
        {
            _ohPlugin.SetBaseUrl(actionParameter);
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            var fn = EmbeddedResources.FindFile("openhab_80.png");
            return EmbeddedResources.ReadImage(fn);
        }
    }
}

