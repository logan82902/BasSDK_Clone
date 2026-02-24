using UnityEngine;
using TriInspector;

namespace ThunderRoad.AI.Action
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class HolsterItem : ActionNode
    {
        public bool holsterRight = true;
        public bool holsterLeft = false;
        public bool waitForFinished = false;

    }
}

