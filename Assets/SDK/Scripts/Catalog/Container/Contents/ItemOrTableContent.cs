using System.Collections.Generic;
using UnityEngine;


using TriInspector;

namespace ThunderRoad
{
    [SkipInTypeDropdown]
    [System.Serializable]
    [DeclareHorizontalGroup("Fields", Sizes = new float[] { 150f, 200f, 70f, 0f, 0f })]
    [DeclareBoxGroup("Fields/Quantity", Title = "Quantity")]
    [DeclareBoxGroup("Fields/State", Title = "State")]
    [DeclareBoxGroup("Fields/Custom data", Title = "Custom data")]
    public abstract class ItemOrTableContent<J, K> : ContainerContent<J, K> where J : CatalogData, IContainerLoadable<J> where K : ItemOrTableContent<J, K>
    {
        [Group("Fields/Quantity")]
        [HideLabel]
        [PropertyOrder(10)]
        public int quantity = 1;

        [Group("Fields/State")]
        [HideLabel]
        [PropertyOrder(11)]
        public ContentState state;

        [Group("Fields/Custom data")]
        [HideLabel]
        [PropertyOrder(12)]
        public List<ContentCustomData> customDataList;

    }
}
