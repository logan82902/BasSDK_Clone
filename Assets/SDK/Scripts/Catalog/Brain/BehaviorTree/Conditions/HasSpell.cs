using TriInspector;

namespace ThunderRoad.AI.Condition
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public partial class HasSpell : ConditionNode
    {
        public string spellID;
        public float spellMinDistance = 8;
        public float spellMaxDistance = 30;

    }
}