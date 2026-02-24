using System.Collections.Generic;
using TriInspector;

namespace ThunderRoad.AI.Control
{
    [System.Serializable]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class Sequence : ControlNode
    {
        [PropertyOrder(0)]
        public bool evaluateAllNodesOnCycle;

    }
}
