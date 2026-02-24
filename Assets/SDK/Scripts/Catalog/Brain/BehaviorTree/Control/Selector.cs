using TriInspector;

namespace ThunderRoad.AI.Control
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class Selector : ControlNode
    {
        [PropertyOrder(0)]
        public bool evaluateAllNodesOnCycle;

    }
}