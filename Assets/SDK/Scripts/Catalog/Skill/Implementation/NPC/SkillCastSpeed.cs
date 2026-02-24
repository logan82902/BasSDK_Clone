
using TriInspector;

namespace ThunderRoad.Skill
{
    [DeclareBoxGroup("Charge Speed", Title = "Charge Speed")]
    public class SkillCastSpeed : AISkillData
    {
        [Group("Charge Speed")]
        public float chargeSpeedMultiplier = 1;

        public override void OnSpellLoad(SpellData spell, SpellCaster caster = null)
        {
            base.OnSpellLoad(spell, caster);
            spell?.AddModifier(this, Modifier.ChargeSpeed, chargeSpeedMultiplier);
        }

        public override void OnSpellUnload(SpellData spell, SpellCaster caster = null)
        {
            base.OnSpellUnload(spell, caster);
            spell?.RemoveModifier(this, Modifier.ChargeSpeed);
        }
    }
}
