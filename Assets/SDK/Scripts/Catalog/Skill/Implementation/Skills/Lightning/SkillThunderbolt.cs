using TriInspector;
using System;

using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Thunderbolt", Title = "Thunderbolt")]
    public class SkillThunderbolt : SpellSkillData
    {
        [Group("Thunderbolt")]
        public float chainRadius = 4;
        [Group("Thunderbolt")]
        public float range = 10;
        [Group("Thunderbolt")]
        public float angle = 15;
        [FormerlySerializedAs("damage")]
        [Group("Thunderbolt")]
        public float damageNpc = 20;
        [Group("Thunderbolt")]
        public float damagePlayer = 10;
        [Group("Thunderbolt")]
        public float damageCrystal = 40;

        [FormerlySerializedAs("allowBounce")]
        [Group("Thunderbolt")]
        public bool allowSurfaceBounce = false;

        [Group("Thunderbolt")]
        public float itemForce = 10f;
        [Group("Thunderbolt")]
        public float creatureForce = 10f;

        [Group("Thunderbolt")]
        public float breakForce = 20f;

        [Group("Thunderbolt")]
        public LayerMask layerMask;

        [Group("Thunderbolt")]
        [Dropdown(nameof(GetAllEffectID))]
        public string mainBoltEffectId;

        protected EffectData mainBoltEffectData;
        [Group("Thunderbolt")]
        [Dropdown(nameof(GetAllEffectID))]
        public string impactEffectId;

        [Group("Thunderbolt")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string statusEffectId;

        protected StatusData statusData;

        [Group("Thunderbolt")]
        protected float statusDuration;

        [Group("Thunderbolt")]
        public Gradient defaultBoltGradient;

        protected EffectData impactEffectData;

    }
}