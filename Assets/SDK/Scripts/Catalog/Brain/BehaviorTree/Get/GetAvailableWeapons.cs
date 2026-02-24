using System.Collections.Generic;
using System.Linq;
using TriInspector;

namespace ThunderRoad.AI.Get
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class AvailableWeapons
    {
        public Item rightMelee1h;
        public Item leftMelee1h;
        public float meleeDistance1h;

        public Item melee2h;
        public float meleeDistance2h;

        public Item bow;

        public Item shield;
        public SpellData spell;
    }

    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class GetAvailableWeapons : ActionNode
    {
        public string outputAvailableAttacksVariableName = "AvailableWeapons";

        [DropdownList(nameof(GetAllSubTreesID))]
        public List<string> priorityTreeID = new();

        public TriDropdownList<string> GetAllSubTreesID()
        {
            TriDropdownList<string> dropdownList = new();
            dropdownList.Add("None", "");
            List<string> list2 = Catalog.GetDataList(Category.BehaviorTree).OfType<BehaviorTreeData>().Where(x => x.type == BehaviorTreeData.Type.SubTree).Select(x => x.id).ToList();
            foreach (string id in list2)
            {
                dropdownList.Add(id, id);
            }
            return dropdownList;
        }

        public GetAvailableWeapons() { }

    }
}
