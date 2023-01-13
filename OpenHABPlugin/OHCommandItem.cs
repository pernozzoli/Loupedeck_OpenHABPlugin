#nullable enable

namespace Loupedeck.OpenHABPlugin
{
    public class OpenHABCommandItem
    {
        public string? Type { get; set; }
        public string? Label { get; set; }
        public string? Name { get; set; }
        public string? Link { get; set; }
        public string? Group { get; set; }
        public string? Category { get; set; }
        public string? State { get; set; }
        public System.Boolean Registered { get; internal set; }
    }
}

#nullable restore