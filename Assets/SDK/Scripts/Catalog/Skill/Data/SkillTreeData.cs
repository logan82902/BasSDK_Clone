using System;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;

namespace ThunderRoad
{
    [Serializable]
    public class SkillTreeData : CatalogData
    {
        public string localizationGroupID;
        public string displayName;
        public string description;
        public int maxTier = 3;
        public bool showInInfuser = true;

        public Color color = Color.white;
        [ColorPickerField(Alpha = true, HDR = true)]
        public Color emissionColor = Color.white;

        public float orbPitch = 1;
        public float costMultiplier = 1;

        [Dropdown(nameof(GetAllItemIds))]
        public string crystalItemId;

        public string orbIconAddress;
        public string infuserTopParticleAddress;
        public string infuserBeamAudioAddress;
        public string musicAddress;
        public string videoAddress;
        public string iconEnabledAddress;
        public string iconDisabledAddress;

        public List<string> tierBlockers;

        [NonSerialized]
        public ItemData crystalItemData;

        public TriDropdownList<string> GetAllItemIds() => Catalog.GetDropdownAllID(Category.Item);

        public override void OnCatalogRefresh()
        {
            base.OnCatalogRefresh();
            tierBlockers = new List<string>();
            crystalItemData = Catalog.GetData<ItemData>(crystalItemId);
        }
    }
}
