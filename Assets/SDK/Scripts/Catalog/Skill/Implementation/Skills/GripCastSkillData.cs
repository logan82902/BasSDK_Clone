using TriInspector;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Spell", Title = "Spell")]
    [DeclareBoxGroup("Damage", Title = "Damage")]
    public class GripCastSkillData : SpellSkillData
    {
        [Group("Spell")]
        [Dropdown(nameof(GetAllSpellID))]
        public string spellId;
        protected SpellCastCharge spellData;
        [Group("Damage")]
        public float gripCastDamageInterval = 0.5f;
        [Group("Damage")]
        public float gripCastDamageAmount = 4f;

        public override void OnCatalogRefresh()
        {
            base.OnCatalogRefresh();
            spellData = Catalog.GetData<SpellCastCharge>(spellId);
        }

        public override void OnSpellLoad(SpellData spell, SpellCaster caster = null)
        {
            base.OnSpellLoad(spell, caster);
            if (spellData == null || spell == null || spell.hashId != spellData.hashId || spell is not SpellCastCharge charge) return;
            charge.allowGripCast = true;
            charge.gripCastDamageAmount = gripCastDamageAmount;
            charge.gripCastDamageInterval = gripCastDamageInterval;
        }

        public override void OnSpellUnload(SpellData spell, SpellCaster caster = null)
        {
            base.OnSpellUnload(spell, caster);
            if (spellData == null || spell == null || spell.hashId != spellData.hashId || spell is not SpellCastCharge charge) return;
            charge.allowGripCast = false;
        }
    }
}
