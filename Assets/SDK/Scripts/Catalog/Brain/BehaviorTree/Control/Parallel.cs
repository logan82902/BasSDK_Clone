using System.Collections;
using System.Collections.Generic;
using TriInspector;

namespace ThunderRoad.AI.Control
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class Parallel : ControlNode
    {
        [PropertyOrder(0)]
        public bool stopOnFirstSuccess;
        [PropertyOrder(0)]
        public bool stopOnFirstFailure;

    }
}

