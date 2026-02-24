using System.Collections;
using System.Collections.Generic;
using TriInspector;

namespace ThunderRoad.AI
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public abstract class ControlNode : Node
    {
        [ShowInInspector]
        [ListDrawerSettings(ShowElementLabels = false, DontForceInline = true)]
        public List<Node> childs = new();
    }
}