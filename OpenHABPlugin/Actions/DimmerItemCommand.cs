using System;
using System.Collections.Generic;
using System.Linq;

namespace Loupedeck.OpenHABPlugin.Actions
{
    public class DimmerItemCommand : PluginDynamicAdjustment
    {
        protected OpenHABService _ohService => ((OpenHABPlugin)this.Plugin).OHService;

        protected Int32 _dimmerValue = 0;
        protected bool _on = false;

        public DimmerItemCommand() : base(true)
        {
            this.DisplayName = "Dimmer";

            this.MakeProfileAction("tree;Select item:");
        }

        protected override void ApplyAdjustment(String actionParameter, Int32 diff)
        {
            _dimmerValue += diff;
            var state = _ohService.SetItemState(actionParameter, _dimmerValue.ToString());

            if (_dimmerValue > 0)
            {
                _on = true;
            }
            else
            {
                _on = false;
            }
            this.AdjustmentValueChanged(actionParameter);
        }

        protected override void RunCommand(String actionParameter)
        {
            if (_on)
            {
                _ohService.SetItemState(actionParameter, "OFF");
            }
            else
            {
                _ohService.SetItemState(actionParameter, _dimmerValue.ToString());
            }
            this.AdjustmentValueChanged(actionParameter);
        }

        protected override String GetAdjustmentValue(String actionParameter) => $"{this._dimmerValue} %";

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

