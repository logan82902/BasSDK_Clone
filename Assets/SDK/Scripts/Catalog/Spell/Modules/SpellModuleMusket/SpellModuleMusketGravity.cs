using ThunderRoad.Skill.Spell;
using TriInspector;
using UnityEngine;

namespace ThunderRoad
{
    public class SpellModuleMusketGravity : SpellModuleMusket<SpellCastGravity>
    {
        public float musketRadius = 2;
        public LayerMask musketDecapitateLayerMask;
        public float musketForce = 50;
        public float musketExplosionRadius = 5;
        public float musketExplosionForce = 2;
        public float musketHitCrystalDamage = 60;
        public float musketExplosionCrystalDamage = 15;
        [Dropdown(nameof(GetAllItemID))]
        public string musketProjectileId;
        protected ItemData musketProjectileData;
        [Dropdown(nameof(GetAllDamagerID))]
        public string musketProjectileDamagerId;
        protected DamagerData musketProjectileDamagerData;
        public float musketShootSpeed = 40;
        public float musketPlayerForce = 10f;
        public string musketProjectileExplosionEffectId;
        protected EffectData musketProjectileExplosionEffectData;
        public string musketProjectileEffectId;
        protected EffectData musketProjectileEffectData;

    }
}