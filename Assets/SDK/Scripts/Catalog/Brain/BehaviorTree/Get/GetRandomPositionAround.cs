using UnityEngine;
using UnityEngine.AI;
using TriInspector;

namespace ThunderRoad.AI.Get
{
    [DeclareBoxGroup("Inputs", Title = "Inputs")]
    [DeclareBoxGroup("Output", Title = "Output")]
    [DeclareBoxGroup("Instance", Title = "Instance")]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class GetRandomPositionAround : ActionNode
    {
        [Group("Inputs")]
        [LabelWidth(100)]
        public Target target = Target.CurrentTarget;
        [Group("Inputs")]
        [DisableIf("target", Target.CurrentTarget)]
        [LabelWidth(200)]
        public string inputTargetVariableName = "";
        public bool scaleMinMaxByHeightDiff = false;
        public float maxHeightDiff = 10f;

        public enum Target
        {
            CurrentTarget,
            InputTransform,
            InputCreature,
            InputPosition,
        }

        [Group("Inputs")]
        [LabelWidth(100)]
        public float minRadius = 0;
        [Group("Inputs")]
        [LabelWidth(100)]
        public float maxRadius = 3;
        [Group("Inputs")]
        [LabelWidth(100)]
        public int attempts = 1;

        [Group("Output")]
        public string outputPositionVariableName = "RandomPosition";

        public GetRandomPositionAround() { }

    }
}
