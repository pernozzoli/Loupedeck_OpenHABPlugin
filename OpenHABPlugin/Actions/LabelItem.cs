using System;
using System.Collections.Generic;
using System.Linq;

namespace Loupedeck.OpenHABPlugin.Actions
{
    /// <summary>
    /// This class shows an item and value as a label.
    /// This class doesn't provide any action.
    /// </summary>
    public class LabelItem : PluginDynamicCommand
    {
        /// <summary>
        /// Reference to openHAB service
        /// </summary>
        protected OpenHABService _ohService => ((OpenHABPlugin)this.Plugin).OHService;

        public LabelItem() : base()
        {
            this.DisplayName = "Labels";
            this.GroupName = "Not used";

            this.MakeProfileAction("tree;Select item:");

        }

        /// <summary>
        /// Called when an item is updated
        /// </summary>
        /// <param name="sender">OH service sending the update</param>
        /// <param name="e">Item status</param>
        private void OnItemChanged(Object sender, OpenHABEventArgs e)
        {
            this.ActionImageChanged(e.Link);
        }

        /// <summary>
        /// Command settings (item selection tree)
        /// </summary>
        /// <returns>Action data</returns>
        protected override PluginProfileActionData GetProfileActionData()
        {
            // create tree data
            var tree = new PluginProfileActionTree("Select item for label");
            tree.AddLevel("Group");
            tree.AddLevel("Item");

            Dictionary<string, PluginProfileActionTreeNode> nodes = new Dictionary<string, PluginProfileActionTreeNode>();

            var groups = _ohService.Labels.Select(i => i.Group).Distinct();

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

            foreach (var item in _ohService.Items)
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
        /// Shows the value of the selected item when updated
        /// </summary>
        /// <param name="actionParameter">Item link</param>
        /// <param name="imageSize">Image size</param>
        /// <returns>Button image, rendering label and value of the item</returns>
        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            _ohService.ItemChanged -= this.OnItemChanged;
            _ohService.ItemChanged += this.OnItemChanged;

            _ohService.RegisterItem(actionParameter);

            using (BitmapBuilder bitmapBuilder = new BitmapBuilder(imageSize))
            {
                var x1 = bitmapBuilder.Width * 0.1;
                var w = bitmapBuilder.Width * 0.8;
                var y1 = bitmapBuilder.Height * 0.2;
                var y2 = bitmapBuilder.Height * 0.65;
                var h = bitmapBuilder.Height * 0.3;
                String label = _ohService.GetItemLabel(actionParameter);
                int maxLength = 9;
                String labelShortened = label.Length > maxLength ? label.Substring(0, maxLength-2) + "..." : label;
                bitmapBuilder.DrawText(labelShortened, (Int32)x1, (Int32)y1, (Int32)w, (Int32)h, BitmapColor.White, imageSize == PluginImageSize.Width90 ? 15 : 9, imageSize == PluginImageSize.Width90 ? 2 : 2);
                bitmapBuilder.DrawText(_ohService.GetStateOfItem(actionParameter), (Int32)x1, (Int32)y2, (Int32)w, (Int32)h, BitmapColor.White, imageSize == PluginImageSize.Width90 ? 15 : 9, imageSize == PluginImageSize.Width90 ? 2 : 2);
                return bitmapBuilder.ToImage();
            }
        }
    }

}
