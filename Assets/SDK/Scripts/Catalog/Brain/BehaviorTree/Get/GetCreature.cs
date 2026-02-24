using System.Collections.Generic;
using UnityEngine;
using TriInspector;

namespace ThunderRoad.AI.Get
{
    [DeclareBoxGroup("General", Title = "General")]
    [DeclareBoxGroup("Outputs", Title = "Outputs")]
    [DeclareHorizontalGroup("Outputs/Horiz", Sizes = new float[] { 215f, 0f })]
    [DeclareHorizontalGroup("Outputs/Horiz2", Sizes = new float[] { 230f, 0f })]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class GetCreature : ActionNode
    {
        [Group("General")]
        public Target target = Target.ClosestEnemy;
        [Group("General")]
        public bool checkFov = true;
        [Group("General")]
        public bool checkSight = true;
        [Group("General")]
        public bool checkShortestPath = false;
        [Group("General")]
        public float rememberDuration = 5;
        [Group("General")]
        public float focusDuration = 5;
        [Group("General")]
        public float maxDistance = 0;
        [Group("General")]
        public bool debugLines;

        [Group("Outputs/Horiz")]
        [LabelWidth(200)]
        public bool outputCreatureAsCurrentTarget = true;
        [Group("Outputs/Horiz")]
        [LabelWidth(200)]
        [DisableIf(nameof(outputCreatureAsCurrentTarget))]
        public string outputCreatureVariableName = "";
        [Group("Outputs/Horiz2")]
        [LabelWidth(120)]
        public OutputTransform outputTransform = OutputTransform.SightedPart;
        [Group("Outputs/Horiz2")]
        [LabelWidth(200)]
        [DisableIf("outputTransform", OutputTransform.None)]
        public string outputTransformVariableName = "";

        public enum Target
        {
            ClosestEnemy,
            ClosestFriend,
            ClosestFollowable,
            ClosestThreatened,
            ClosestPlayer,
            ClosestBody,
            Self,
        }

        public enum OutputTransform
        {
            None,
            CreatureRoot,
            SightedPart,
        }

        public enum Sight
        {
            Nothing,
            Creature,
            Target,
        }

        public GetCreature() { }

    }
}
