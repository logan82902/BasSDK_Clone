using UnityEngine;
using TriInspector;

namespace ThunderRoad.AI.Action
{
    [DeclareBoxGroup("Inputs", Title = "Inputs")]
    [DeclareHorizontalGroup("Inputs/Horiz")]
    [DeclareBoxGroup("StuckDetection", Title = "StuckDetection")]
    [DeclareHorizontalGroup("StuckDetection/Horiz")]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class MoveInDirection : ActionNode
    {
        [Group("Inputs/Horiz")]
        [LabelWidth(100)]
        public Vector2 moveDirection = new();
        [Group("Inputs/Horiz")]
        [LabelWidth(100)]
        public float moveSpeedRatio = 1f;
        [Group("Inputs/Horiz")]
        [LabelWidth(100)]
        public float turnSpeed = 0f;
        [Group("Inputs/Horiz")]
        [LabelWidth(100)]
        public float moveMinDuration = 0.5f;

        [Group("StuckDetection/Horiz")]
        [LabelWidth(100)]
        public float stuckRadius = 0.1f;
        [Group("StuckDetection/Horiz")]
        [LabelWidth(150)]
        public float stuckMaxDuration = 5f;

    }
}
