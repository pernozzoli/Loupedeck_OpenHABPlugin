#nullable enable
namespace Loupedeck.OpenHABPlugin
{
    using System;

    public class OpenHABEventArgs : EventArgs
    {
        public string Link { get; set; }
        public string State { get; set; }
        public string Command { get; set; }

        public OpenHABEventArgs(string item, string state, string command)
        {
            Link = item;
            State = state;
            Command = command;
        }
    }
}
#nullable restore

