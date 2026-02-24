using System.Collections.Generic;
using ThunderRoad.Skill.SpellPower;
using TriInspector;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Status", Title = "Status")]
    [DeclareBoxGroup("Effect", Title = "Effect")]
    public class SkillEmergencyExit : SkillData
    {
        [Group("Status")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string statusId;

        protected StatusData statusData;
        [Group("Status")]
        public float radius;
        [Group("Status")]
        public float duration;
        [Group("Effect")]
        [Dropdown("GetAllEffectID")]
        public string effectId;

        protected EffectData effectData;
        
        public override void OnCatalogRefresh()
        {
            base.OnCatalogRefresh();
            statusData = Catalog.GetData<StatusData>(statusId);
            effectData = Catalog.GetData<EffectData>(effectId);
        }

    }
}
