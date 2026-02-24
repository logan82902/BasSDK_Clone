using UnityEngine;
using TriInspector;

namespace ThunderRoad.AI.Action
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class Wait : ActionNode
    {
        public bool startWaiting = true;
        public float minDuration = 1;
        public float maxDuration = 5;

    }
}
