using System;
using ThunderRoad.Skill.Spell;
using TriInspector;
using UnityEngine;

namespace ThunderRoad
{
    [DeclareFoldoutGroup("SpellBomb", Title = "Spell Bomb Explode")]
    [DeclareHorizontalGroup("SpellBomb/Affects")]
    [DeclareHorizontalGroup("SpellBomb/BoostForce")]
    [DeclareHorizontalGroup("SpellBomb/ImplodeForce")]
    [DeclareHorizontalGroup("SpellBomb/Status")]
    public class SpellModuleBombGravity : SpellModuleBomb<SpellCastGravity>
    {
        [Group("SpellBomb")]
        public float bombAoeRadius;

        [Group("SpellBomb/Affects")]
        [AutoLabelWidth]
        public bool bombAoeAffectPlayer = true;

        [Group("SpellBomb/Affects")]
        [AutoLabelWidth]
        public bool bombAoeAffectCreatures = true;

        [Group("SpellBomb/Affects")]
        [AutoLabelWidth]
        public bool bombAoeRagdollCreatures = true;

        [Group("SpellBomb/Affects")]
        [AutoLabelWidth]
        public bool bombAoeAffectItems = true;

        [Group("SpellBomb"), Dropdown(nameof(GetAllEffectID))]
        public string bombBoostEffectId;
        protected EffectData bombBoostEffect;

        [Group("SpellBomb/BoostForce")]
        [AutoLabelWidth]
        public float bombBoostForce;

        [Group("SpellBomb/BoostForce")]
        [AutoLabelWidth]
        public ForceMode bombBoostForceMode;

        [Group("SpellBomb/BoostForce")]
        [AutoLabelWidth]
        public float bombBoostForceDuration;

        [Group("SpellBomb/Status"), Dropdown(nameof(GetAllStatusEffectID))]
        public string bombAoeStatusId;
        protected StatusData bombAoeStatus;

        [Group("SpellBomb/Status")]
        public float bombAoeStatusDuration;

        public override void OnCatalogRefresh()
        {
            bombBoostEffect = Catalog.GetData<EffectData>(bombBoostEffectId);
            bombAoeStatus = Catalog.GetData<StatusData>(bombAoeStatusId);
        }

    }
}