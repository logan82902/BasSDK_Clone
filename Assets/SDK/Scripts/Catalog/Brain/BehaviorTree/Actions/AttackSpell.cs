using TriInspector;

namespace ThunderRoad.AI.Action
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class AttackSpell : ActionNode
    {
        public Target target = Target.CurrentTarget;
        [HideIf("target", Target.CurrentTarget)]
        public string inputTransformVariableName = "";

        public enum Target
        {
            CurrentTarget,
            InputTransform,
        }

        public bool castLeft = true;
        public bool castRight = false;

        public enum CastSide
        {
            None,
            CastRight,
            CastLeft,
            CastBoth,
        }

    }
}

