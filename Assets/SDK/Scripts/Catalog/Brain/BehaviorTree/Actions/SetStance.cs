using TriInspector;

namespace ThunderRoad.AI.Action
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class SetStance : ActionNode
    {
        public BrainModuleStance.Stance stance = BrainModuleStance.Stance.Idle;
        public bool onlyOnce = false;


    }
}
