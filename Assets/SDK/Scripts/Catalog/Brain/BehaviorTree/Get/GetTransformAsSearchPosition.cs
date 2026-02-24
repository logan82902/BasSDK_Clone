using UnityEngine;
using TriInspector;

namespace ThunderRoad.AI.Get
{
    [DeclareBoxGroup("Input", Title = "Input")]
    [DeclareBoxGroup("Output", Title = "Output")]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class GetTransformAsSearchPosition : ActionNode
    {
        public float offsetHeightRatioWhenOnFloor = 1;
        public float raycastOffset = 0.1f;

        [Group("Input")]
        public string inputTransformVariableName = "Transform";
        [Group("Output")]
        public string outputPositionVariableName = "Position";
        [Group("Output")]
        public string outputOffsetPositionVariableName = "OffsetPosition";

        public GetTransformAsSearchPosition() { }

    }
}
