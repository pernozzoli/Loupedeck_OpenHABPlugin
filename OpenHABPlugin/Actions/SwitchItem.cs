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
    public class SwitchItem : PluginMultistateDynamicCommand
    {
        /// <summary>
        /// Reference to openHAB service
        /// </summary>
        protected OpenHABService _ohService => ((OpenHABPlugin)this.Plugin).OHService;

        public SwitchItem() : base()
        {
            this.DisplayName = "Switches";
            this.GroupName = "Not used";

            this.AddState("OFF", "off", "Switch on");
            this.AddState("ON", "on", "Switch off");

            this.MakeProfileAction("tree;Select item:");
        }

        /// <summary>
        /// Called when registered item changes
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Item info</param>
        private void OnItemChanged(Object sender, OpenHABEventArgs e)
        {
            if (this.GetCurrentState(e.Link).Name != e.State)
            {
                this.ToggleCurrentState(e.Link);
            }
            this.ActionImageChanged(e.Link);
        }

        /// <summary>
        /// Command execution
        /// </summary>
        /// <param name="actionParameter">Item link</param>
        protected override void RunCommand(String actionParameter)
        {
            this.ToggleCurrentState(actionParameter);
            _ohService.SendItemState(actionParameter, this.GetCurrentState(actionParameter).Name);
            this.ActionImageChanged(actionParameter);
        }

        /// <summary>
        /// Command settings (item selection tree)
        /// </summary>
        /// <returns>Action data</returns>
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

        /// <summary>
        /// Get image, register item and register to item change event
        /// </summary>
        /// <param name="actionParameter">Item link</param>
        /// <param name="stateIndex">Status</param>
        /// <param name="imageSize">Image size for button</param>
        /// <returns></returns>
        protected override BitmapImage GetCommandImage(String actionParameter, Int32 stateIndex, PluginImageSize imageSize)
        {
            //Console.WriteLine("Image for item requested: " + actionParameter + ", stateIndex: " + stateIndex);
            _ohService.ItemChanged -= this.OnItemChanged;
            _ohService.ItemChanged += this.OnItemChanged;

            _ohService.RegisterItem(actionParameter);
            return base.GetCommandImage(actionParameter, stateIndex, imageSize);
        }
    }
}

