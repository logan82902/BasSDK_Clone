using UnityEngine;
using System.Collections;
using TriInspector;

namespace ThunderRoad.AI.Action
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class MoveLeftToRight : ActionNode
    {
        public bool startActionOnDisarm = true;
        public Vector2 disarmedActionStartDelay = new Vector2(0.1f, 0.25f);
    }
}
