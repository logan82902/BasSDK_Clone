using TriInspector;

namespace ThunderRoad.AI.Action
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class SetAnimator : ActionNode
    {
        public string parameterID;
        public Parameter parameterType = Parameter.Trigger;

        public enum Parameter
        {
            Trigger,
            Bool,
            Float,
        }

        [EnableIf(nameof(parameterType), Parameter.Bool)]
        public bool boolValue;
        [EnableIf(nameof(parameterType), Parameter.Float)]
        public float floatValue;

    }
}
