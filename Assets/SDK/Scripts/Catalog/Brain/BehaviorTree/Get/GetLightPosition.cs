using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TriInspector;

namespace ThunderRoad.AI.Get
{
    [DeclareBoxGroup("Output", Title = "Output")]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class GetLightPosition : ActionNode
    {
        public float offsetHeightRatioWhenOnFloor = 1;
        public float raycastOffset = 0.1f;

        [Group("Output")]
        public string outputPositionVariableName = "LightPosition";
        [Group("Output")]
        public string outputOffsetPositionVariableName = "LightOffsetPosition";

        public GetLightPosition() { }

    }
}
