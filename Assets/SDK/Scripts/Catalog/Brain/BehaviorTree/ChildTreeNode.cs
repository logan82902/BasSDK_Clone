using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using TriInspector;

namespace ThunderRoad.AI
{
    [AutoLabelWidth(AddedWidth = 5f)]
    [DeclareHorizontalGroup("Reference")]
    public class ChildTreeNode : Node
    {
        public enum ChildNodeRef
        {
            ForceID,
            DynamicID,
        }

        [Group("Reference")]
        [EnumToggleButtons]
        [HideLabel]
        public ChildNodeRef reference = ChildNodeRef.ForceID;
        [EnableIf(nameof(reference), ChildNodeRef.ForceID)]
        [Dropdown(nameof(GetAllBehaviorTreeID))]
        public string childTreeID;
        [EnableIf(nameof(reference), ChildNodeRef.DynamicID)]
        public string childTreeName;

        public TriDropdownList<string> GetAllBehaviorTreeID() => Catalog.GetDropdownAllID(Category.BehaviorTree);

    }

}