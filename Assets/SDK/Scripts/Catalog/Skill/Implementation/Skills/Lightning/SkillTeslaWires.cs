using System.Collections;
using TriInspector;
using UnityEngine;
namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Wire", Title = "Wire")]
    public class SkillTeslaWires : SpellSkillData
    {
        [Group("Wire")]
        public float duration = 3f;
        [Group("Wire")]
        [Dropdown(nameof(GetAllEffectID))]
        public string arcLoopEffectId = "LightningArcLoop";
        protected EffectData arcLoopEffectData;

        [Group("Wire")]
        [Dropdown(nameof(GetAllSkillID))]
        public string skillArcwireId = "Arcwire";

        protected SkillArcwire arcwireData;

        public override void OnCatalogRefresh()
        {
            base.OnCatalogRefresh();
            arcwireData = Catalog.GetData<SkillArcwire>(skillArcwireId);
            arcLoopEffectData = Catalog.GetData<EffectData>(arcLoopEffectId);
        }




 // ProjectCore        
    }

}
