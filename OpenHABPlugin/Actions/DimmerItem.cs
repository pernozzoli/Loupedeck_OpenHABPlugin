using System;
using System.Collections.Generic;
using System.Linq;

namespace Loupedeck.OpenHABPlugin.Actions
{
    /// <summary>
    /// This class provides an adjustment and reset for dimmer items
    /// </summary>
    public class DimmerItem : PluginDynamicAdjustment
    {
        /// <summary>
        /// Reference to openHAB service
        /// </summary>
        protected OpenHABService _ohService => ((OpenHABPlugin)this.Plugin).OHService;

        /// <summary>
        /// Constructor
        /// </summary>
        public DimmerItem() : base(true)
        {
            this.DisplayName = "Dimmer";

            this.MakeProfileAction("tree;Select item:");
        }

        /// <summary>
        /// Calls when user turns a know and sends the new value to openHAB
        /// </summary>
        /// <param name="actionParameter">Item link</param>
        /// <param name="diff">Adjustment difference</param>
        protected override void ApplyAdjustment(String actionParameter, Int32 diff)
        {
            /// Get the state from the item
            var currentValue = OpenHABService.ExtractNumericalValue(_ohService.GetStateOfItem(actionParameter));
            currentValue = OpenHABService.MinMax(currentValue + diff, 0, 100);
            var state = _ohService.SendItemState(actionParameter, currentValue.ToString());

            this.AdjustmentValueChanged(actionParameter);
        }

        /// <summary>
        /// Toggles on and off depending on the current value of the item
        /// </summary>
        /// <param name="actionParameter"></param>
        protected override void RunCommand(String actionParameter)
        {
            var currentValue = OpenHABService.ExtractNumericalValue(_ohService.GetStateOfItem(actionParameter));
            if (currentValue > 0)
            {
                _ohService.SendItemState(actionParameter, "OFF");
            }
            else
            {
                _ohService.SendItemState(actionParameter, "ON");
            }

            this.AdjustmentValueChanged(actionParameter);
        }

        /// <summary>
        /// Gets the current cached state of the item as adjustment
        /// </summary>
        /// <param name="actionParameter"></param>
        /// <returns></returns>
        protected override String GetAdjustmentValue(String actionParameter)
        {
            var currentValue = OpenHABService.ExtractNumericalValue(_ohService.GetStateOfItem(actionParameter));

            return $"{currentValue} %";
        }

        /// <summary>
        /// This method is used to register this class for udpates
        /// </summary>
        /// <param name="actionParameter">Item link</param>
        /// <param name="imageSize">Image size</param>
        /// <returns></returns>
        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            _ohService.ItemChanged -= this.OnItemChanged;
            _ohService.ItemChanged += this.OnItemChanged;

            _ohService.RegisterItem(actionParameter);
            return base.GetCommandImage(actionParameter, imageSize);
        }

        /// <summary>
        /// Calls when an item was updated externally
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Item state and other info according to <see cref="OpenHABEventArgs"/></param>
        private void OnItemChanged(Object sender, OpenHABEventArgs e)
        {
            Console.WriteLine($"Update received for {e.Link}: {e.State}");
            AdjustmentValueChanged(e.Link);
        }


        /// <summary>
        /// Command settings (item selection tree)
        /// </summary>
        /// <returns>Action data</returns>
        protected override PluginProfileActionData GetProfileActionData()
        {
            // create tree data
            var tree = new PluginProfileActionTree("Select Dimmer item");
            tree.AddLevel("Group");
            tree.AddLevel("Item");

            Dictionary<string, PluginProfileActionTreeNode> nodes = new Dictionary<string, PluginProfileActionTreeNode>();

            var groups = _ohService.Dimmer.Select(i => i.Group).Distinct();

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

            foreach (var item in _ohService.Dimmer)
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

