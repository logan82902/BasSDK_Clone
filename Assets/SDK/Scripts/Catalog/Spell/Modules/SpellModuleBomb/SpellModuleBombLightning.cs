using System.Collections.Generic;
using ThunderRoad.Skill.Spell;
using TriInspector;
using UnityEngine;

namespace ThunderRoad
{
    [DeclareFoldoutGroup("SpellBomb", Title = "Spell Bomb Explode")]
    public class SpellModuleBombLightning : SpellModuleBomb<SpellCastLightning>
    {
        [Group("SpellBomb"), Dropdown(nameof(GetAllEffectID))]
        public string explodeEffectId;
        protected EffectData explodeEffectData;

        [Group("SpellBomb")]
        public float explodeElectrocuteRadius = 2f;

        [Group("SpellBomb")]
        public float explodeElectrocuteDuration = 5f;

        [Group("SpellBomb"), Dropdown(nameof(GetAllSkillID))]
        public string skillArcwireId = "Arcwire";
        protected SkillArcwire arcwireData;

        [Group("SpellBomb")]
        public int explodeArcCount = 7;

        [Group("SpellBomb")]
        public int nodesPerArc = 5;

        [Group("SpellBomb")]
        public float arcAngle = 0.8f;

        [Group("SpellBomb")]
        public Vector2 minMaxArcLifetime = new(0.4f, 0.8f);

        [Group("SpellBomb")]
        public Vector2 minMaxArcSpeed = new(3f, 4f);

        [Group("SpellBomb")]
        public bool arcsTargetCreatures = true;

        public override void OnCatalogRefresh()
        {
            base.OnCatalogRefresh();
            arcwireData = Catalog.GetData<SkillArcwire>(skillArcwireId);
            explodeEffectData = Catalog.GetData<EffectData>(explodeEffectId);
        }

    }
}