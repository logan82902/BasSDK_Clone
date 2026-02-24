
using TriInspector;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Chaining", Title = "Chaining")]
    public class SkillChainLightning : SpellSkillData
    {
        [Group("Chaining")]
        public float chainRangeMultiplier = 3f;
        public override void OnSpellLoad(SpellData spell, SpellCaster caster = null)
        {
            base.OnSpellLoad(spell, caster);
            if (spell is not SpellCastLightning lightning) return;
            lightning.allowChainItems = true;
            lightning.allowChainEnemies = true;
            lightning.AddModifier(this, SpellCastLightning.modifierChainRange, chainRangeMultiplier);
        }

        public override void OnSpellUnload(SpellData spell, SpellCaster caster = null)
        {
            base.OnSpellUnload(spell, caster);
            if (spell is not SpellCastLightning lightning) return;
            lightning.allowChainItems = false;
            lightning.allowChainEnemies = false;
            lightning.RemoveModifier(this, SpellCastLightning.modifierChainRange);
        }
    }
}
