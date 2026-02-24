using UnityEngine;
using TriInspector;

namespace ThunderRoad.AI.Condition
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class IsThreatened : ConditionNode
    {
        public Target target = Target.CurrentTarget;
        [ShowIf("target", Target.InputCreature)]
        public string inputCreatureVariableName = "";

        public enum Target
        {
            CurrentTarget,
            InputCreature,
        }

        public float threatMinSpeed = 2f;
        public float threatDuration = 2f;

    }
}