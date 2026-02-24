using UnityEngine;
using TriInspector;

namespace ThunderRoad.AI.Get
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class GetCreaturePart : ActionNode
    {
        public Target target = Target.CurrentTarget;
        [ShowIf("target", Target.InputCreature)]
        public string inputCreatureVariableName = "";

        public enum Target
        {
            CurrentTarget,
            InputCreature,
        }

        public TargetPart targetPart = TargetPart.CreatureRoot;
        public string outputTransformVariableName = "CreatureTransform";

        public enum TargetPart
        {
            CreatureRoot,
            RagdollRoot,
            RagdollTarget,
            RagdollHead,
            RagdollEyes,
            RagdollHandRight,
            RagdollHandLeft,
        }

        public GetCreaturePart() { }

    }
}
