using TriInspector;

namespace ThunderRoad.AI.Action
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class SetFollowTarget : ActionNode
    {
        public enum Action
        {
            Set,
            Clear,
        }
        public Action action = Action.Set;
        public string creatureVariableName = "";

    }
}