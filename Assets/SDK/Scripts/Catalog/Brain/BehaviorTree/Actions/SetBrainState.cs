using TriInspector;

namespace ThunderRoad.AI.Action
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class SetBrainState : ActionNode
    {
        public Brain.State brainState = Brain.State.Idle;
    }
}
