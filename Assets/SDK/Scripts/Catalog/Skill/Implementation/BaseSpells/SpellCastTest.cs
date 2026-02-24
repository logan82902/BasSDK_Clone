using System;
using TriInspector;
using UnityEngine;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Item", Title = "Item")]
    [DeclareBoxGroup("Explosion", Title = "Explosion")]
    public class SpellCastTest : SpellCastCharge
    {

        [Group("Item")]
        [Dropdown("GetAllItemID")]
        public string itemId;

        [Group("Explosion")]
        [Dropdown(nameof(GetAllSkillID))]
        public string remoteDetonationId = "RemoteDetonation";

        public static SkillRemoteDetonation remoteDetonationData;

        [NonSerialized]
        public ItemData itemData;

        public override void OnCatalogRefresh()
        {
            base.OnCatalogRefresh();
            remoteDetonationData = Catalog.GetData<SkillRemoteDetonation>(remoteDetonationId);
            itemData = Catalog.GetData<ItemData>(itemId);
        }

        protected Item cup;

    }
}
