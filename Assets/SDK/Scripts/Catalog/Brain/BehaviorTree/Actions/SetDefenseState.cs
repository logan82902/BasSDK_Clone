using TriInspector;

namespace ThunderRoad.AI.Action
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class SetDefenseState : ActionNode
    {
        public bool parry;
        public bool dodge;


    }
}
