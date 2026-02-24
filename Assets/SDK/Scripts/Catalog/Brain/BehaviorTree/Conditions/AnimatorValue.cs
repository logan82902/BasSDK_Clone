using UnityEngine;
using TriInspector;

namespace ThunderRoad.AI.Condition
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class AnimatorValue : ConditionNode
    {
        public string parameterName;
        public AnimatorControllerParameterType parameterType = AnimatorControllerParameterType.Bool;
        [HideIf("parameterType", AnimatorControllerParameterType.Bool)]
        public Comparator variableComparator;
        [HideIf("parameterType", AnimatorControllerParameterType.Bool)]
        public float comparedValue;

        public enum Comparator
        {
            Equal,
            GreaterThan,
            GreaterThanOrEqual,
            LowerThan,
            LowerThanOrEqual,
        }

    }
}