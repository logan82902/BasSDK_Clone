using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TriInspector;

namespace ThunderRoad.AI.Get
{
    [DeclareBoxGroup("General", Title = "General")]
    [DeclareBoxGroup("Output", Title = "Output")]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class GetTelegrabbedObject : ActionNode
    {
        public bool ignoreFriendlyTK = true;
        public float offsetHeightRatioWhenOnFloor = 1;
        public float raycastOffset = 0.1f;
        [Group("General")]
        public float maxDistance = 0;
        [Group("General")]
        public bool debugLines;

        [Group("Output")]
        public string outputPositionVariableName = "TKGrabPosition";
        [Group("Output")]
        public string outputOffsetPositionVariableName = "TKGrabOffsetPosition";

        public GetTelegrabbedObject() { }

    }
}
