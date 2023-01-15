#nullable enable
namespace Loupedeck.OpenHABPlugin
{
    using System;

    /// <summary>
    /// This class contains information on an item update event
    /// </summary>
    public class OpenHABEventArgs : EventArgs
    {
        /// <summary>
        /// Item link (id and direct access via openHAB API)
        /// </summary>
        public string Link { get; set; }
        /// <summary>
        /// Item state
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// Command (not used currently)
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// Constructur
        /// </summary>
        /// <param name="item"></param>
        /// <param name="state"></param>
        /// <param name="command"></param>
        public OpenHABEventArgs(string item, string state, string command)
        {
            Link = item;
            State = state;
            Command = command;
        }
    }
}
#nullable restore

