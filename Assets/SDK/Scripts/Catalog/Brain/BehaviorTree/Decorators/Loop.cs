using System.Collections;
using TriInspector;

namespace ThunderRoad.AI.Decorator
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class Loop : DecoratorNode
    {
        public Node loopingNode;
        public Mode mode = Mode.LoopingNodeFirst;
        public enum Mode
        {
            LoopingNodeFirst,
            LoopingNodeLast,
        }

        public Loop() : base() { }

    }
}
