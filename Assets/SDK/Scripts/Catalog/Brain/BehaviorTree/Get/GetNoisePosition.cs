using UnityEngine;
using TriInspector;

namespace ThunderRoad.AI.Get
{
    [DeclareBoxGroup("Output", Title = "Output")]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class GetNoisePosition : ActionNode
    {
        public float offsetHeightRatioWhenOnFloor = 1;
        public float raycastOffset = 0.1f;

        [Group("Output")]
        public string outputPositionVariableName = "NoisePosition";
        [Group("Output")]
        public string outputOffsetPositionVariableName = "NoiseOffsetPosition";

        public GetNoisePosition() { }

    }
}
