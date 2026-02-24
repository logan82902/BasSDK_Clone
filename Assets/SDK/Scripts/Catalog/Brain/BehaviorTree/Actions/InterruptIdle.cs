using TriInspector;

namespace ThunderRoad.AI.Action
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class InterruptIdle : ActionNode
    {
        public bool alwaysFullInterrupt = false;
        public bool changeBrainStateBeforeInterrupt = false;
        [ShowIf("changeBrainStateBeforeInterrupt")]
        public Brain.State brainState = Brain.State.Alert;

    }
}
