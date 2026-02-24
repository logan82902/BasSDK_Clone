using TriInspector;
using UnityEngine;
using Newtonsoft.Json;

namespace ThunderRoad
{
    [DeclareBoxGroup("Effects")]
    public class ItemModuleCrystalMusket : ItemModule
    {
        public float hammerDuration = 0.2f;
        public float hammerDelay = 0.2f;
        public float fireDelay = 0.5f;
        public float recoil = 1000;
        public AnimationCurve recoilCurve;
        public float angularRecoil = 1000;
        public AnimationCurve angularRecoilCurve;
        public float recoilDuration = 0.3f;
        public GameData.HapticClip hapticClip;

        public float imbueDrainOnFire = 100;

        [Group("Effects")]
        [Dropdown(nameof(GetAllEffectID))]
        public string hammerImpactEffectId;
        [JsonIgnore]
        public EffectData hammerImpactEffectData;

        [Group("Effects")]
        [Dropdown(nameof(GetAllEffectID))]
        public string dryFireEffectId;
        [JsonIgnore]
        public EffectData dryFireEffectData;

        [Group("Effects")]
        [Dropdown(nameof(GetAllEffectID))]
        public string chargeIncreaseEffectId;
        [JsonIgnore]
        public EffectData chargeIncreaseEffectData;

        [Group("Effects")]
        [Dropdown(nameof(GetAllEffectID))]
        public string fullyChargedEffectId;
        [JsonIgnore]
        public EffectData fullyChargedEffectData;

        [Group("Effects")]
        [Dropdown(nameof(GetAllEffectID))]
        public string shootEffectId;
        [JsonIgnore]
        public EffectData shootEffectData;
        public float heldRecoilMultiplier = 3;
        [Dropdown(nameof(GetAllColliderGroupNames))]
        public string crystalColliderGroup;

        public TriDropdownList<string> GetAllEffectID()
        {
            return Catalog.GetDropdownAllID(Category.Effect);
        }

        public TriDropdownList<string> GetAllColliderGroupNames()
        {
            TriDropdownList<string> result = new TriDropdownList<string>
            {
                { "None", "" }
            };
            foreach (var colliderGroup in itemData.colliderGroups)
            {
                result.Add(colliderGroup.transformName, colliderGroup.transformName);
            }
            return result;
        }

        public override void OnItemDataRefresh(ItemData data)
        {
            base.OnItemDataRefresh(data);
            shootEffectData = Catalog.GetData<EffectData>(shootEffectId);
            dryFireEffectData = Catalog.GetData<EffectData>(dryFireEffectId);
            hammerImpactEffectData = Catalog.GetData<EffectData>(hammerImpactEffectId);
            dryFireEffectData = Catalog.GetData<EffectData>(dryFireEffectId);
            chargeIncreaseEffectData = Catalog.GetData<EffectData>(chargeIncreaseEffectId);
            fullyChargedEffectData = Catalog.GetData<EffectData>(fullyChargedEffectId);
        }

    }
}