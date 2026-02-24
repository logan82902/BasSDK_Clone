using System.Collections.Generic;
using TriInspector;

namespace ThunderRoad.AI.Condition
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class BrainStateCheck : ConditionNode
    {
        public bool invert;
        public List<Brain.State> successStates = new List<Brain.State>();

    }
}
