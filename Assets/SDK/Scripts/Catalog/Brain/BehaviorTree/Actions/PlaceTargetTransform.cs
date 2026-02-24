using UnityEngine;
using TriInspector;

namespace ThunderRoad.AI.Action
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class PlaceTargetTransform : ActionNode
    {
        public string vectorPositionName = "Position";
        public string transformVariableName = "PositionTargetTransform";

    }
}
