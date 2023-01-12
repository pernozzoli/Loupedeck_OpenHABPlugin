using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;

using Newtonsoft.Json;

using Newtonsoft.Json.Linq;

#nullable enable
namespace Loupedeck.OpenHABPlugin.Actions
{
    public class SwitchItemMSCommand : PluginMultistateDynamicCommand
    {
        protected OpenHABService _ohService => ((OpenHABPlugin)this.Plugin).OHService;

        public SwitchItemMSCommand() : base()
        {
            this.DisplayName = "Switches";
            this.GroupName = "Not used";

            this.AddState("OFF", "off", "Switch on");
            this.AddState("ON", "on", "Switch off");

            this.MakeProfileAction("tree;Select item:");

        }

        protected override void RunCommand(String actionParameter)
        {
            this.ToggleCurrentState(actionParameter);
            //var state = _ohService.ToggleItem(actionParameter);
            _ohService.SetItemState(actionParameter, this.GetCurrentState(actionParameter).Name);
            this.ActionImageChanged(actionParameter);
        }
 
        //protected override BitmapImage GetCommandImage(String actionParameter, Int32 stateIndex, PluginImageSize imageSize)
        //{
        //    /// Get the label of the item
        //    var item = _ohService.Switches.FirstOrDefault(i => i.Link == actionParameter);
        //    string? itemLabel = item?.Label;
        //    string? itemCategory = item?.Category;
        //    using (var bitmapBuilder = new BitmapBuilder(imageSize))
        //    {
        //        var imgStream = _ohService.GetItemIconForState(stateIndex == 1 ? "ON" : "OFF", itemCategory);
        //        bitmapBuilder.SetBackgroundImage(BitmapImage.FromArray(imgStream));
        //        /// TODO: Check if word wrap can be done
        //        bitmapBuilder.DrawText(itemLabel);
        //        return bitmapBuilder.ToImage();
        //    }
        //}

        protected override PluginProfileActionData GetProfileActionData()
        {
            // create tree data
            var tree = new PluginProfileActionTree("Select Switch item");
            tree.AddLevel("Group");
            tree.AddLevel("Item");

            Dictionary<string, PluginProfileActionTreeNode> nodes = new Dictionary<string, PluginProfileActionTreeNode>();

            var groups = _ohService.Switches.Select(i => i.Group).Distinct();

            foreach (var group in groups)
            {
                if (group != null)
                {
                    nodes.Add(group!, tree.Root.AddNode(group));
                }
            }

            const String others = "Others";
            /// Add a group for items without group
            nodes.Add(others, tree.Root.AddNode(others));

            foreach (var item in _ohService.Switches)
            {
                PluginProfileActionTreeNode node = nodes[item.Group != null ? item.Group : others];
                if (node != null)
                {
                    node.AddItem(item.Link, item.Label);
                }
            }

            return tree;
        }

    }
}

