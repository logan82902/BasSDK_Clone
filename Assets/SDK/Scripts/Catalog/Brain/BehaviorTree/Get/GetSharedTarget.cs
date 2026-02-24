using System.Collections.Generic;
using UnityEngine;
using TriInspector;

namespace ThunderRoad.AI.Get
{
    [DeclareBoxGroup("General", Title = "General")]
    [DeclareBoxGroup("Outputs", Title = "Outputs")]
    [DeclareHorizontalGroup("Outputs/Horiz", Sizes = new float[] { 215f, 0f })]
    [DeclareHorizontalGroup("Outputs/Horiz2")]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class GetSharedTarget : ActionNode
    {
        [Group("General")]
        public bool checkRoomAlert = true;
        [Group("General")]
        public bool checkCloseFriends = true;
        [Group("General")]
        public float closeFriendMaxDistance = 10;
        [Group("General")]
        public bool checkShortestPath = false;

        [Group("Outputs/Horiz")]
        [LabelWidth(200)]
        public bool outputCreatureAsCurrentTarget = true;
        [Group("Outputs/Horiz")]
        [LabelWidth(200)]
        [DisableIf(nameof(outputCreatureAsCurrentTarget))]
        public string outputCreatureVariableName = "";
        [Group("Outputs/Horiz2")]
        [LabelWidth(200)]
        public string outputTransformVariableName = "";

        public GetSharedTarget() { }

    }
}
