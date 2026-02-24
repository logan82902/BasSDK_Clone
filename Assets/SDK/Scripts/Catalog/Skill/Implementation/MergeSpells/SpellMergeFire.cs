using System;
using System.Collections;
using System.Collections.Generic;
using ThunderRoad.Skill.Spell;
using UnityEngine;
using Random = UnityEngine.Random;
using TriInspector;

namespace ThunderRoad.Skill.SpellMerge
{
    [Serializable]
    [DeclareBoxGroup("Meteor", Title = "Meteor")]
    [DeclareBoxGroup("Status Effect", Title = "Status Effect")]
    public class SpellMergeFire : SpellMergeData
    {
        [Group("Meteor")]
        [Dropdown(nameof(GetAllEffectID))]
        public string meteorEffectId;

        protected EffectData meteorEffectData;
        [NonSerialized]
        public EffectData overrideMeteorEffect;

        [Group("Meteor")]
        [Dropdown(nameof(GetAllEffectID))]
        public string meteorExplosionEffectId;

        protected EffectData meteorExplosionEffectData;
        [NonSerialized]
        public EffectData overrideExplosionEffect;

        [Group("Meteor")]
        [Dropdown(nameof(GetAllItemID))]
        public string meteorItemId;

        protected ItemData meteorItemData;

        [Group("Meteor")]
        public float meteorVelocity = 7;

        [Group("Meteor")]
        public bool meteorUseGravity = true;

        [Group("Meteor")]
        public float meteorExplosionForce = 20;

        [Group("Meteor")]
        public float meteorExplosionPlayerForce = 5f;

        [Group("Meteor")]
        public float meteorExplosionDamage = 20;

        [Group("Meteor")]
        public float meteorExplosionPlayerDamage = 20;

        [Group("Meteor")]
        public float meteorExplosionRadius = 10;

        [Group("Meteor")]
        public LayerMask explosionLayerMask;

        [Group("Meteor")]
        [Dropdown(nameof(GetAllSpellID))]
        public string meteorImbueSpellId;

        [Group("Meteor")]
        public AnimationCurve meteorIntensityCurve = AnimationCurve.EaseInOut(0, 0, 0.5f, 1);

        [Group("Status Effect")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string statusEffectId;
        [NonSerialized]
        public StatusData statusEffect;

        [Group("Status Effect")]
        public float statusDuration = 5;

        protected SpellCastProjectile fire;

        public delegate void ExplodeEvent(SpellMergeFire fire, Vector3 position, float radius, EventTime time);
        public event ExplodeEvent OnExplodeEvent;

        protected SpellCastCharge meteorImbueSpellData;

        private HashSet<Creature> hitCreatures;
        

        public override void OnCatalogRefresh()
        {
            base.OnCatalogRefresh();
            meteorEffectData = Catalog.GetData<EffectData>(meteorEffectId);
            meteorExplosionEffectData = Catalog.GetData<EffectData>(meteorExplosionEffectId);
            meteorItemData = Catalog.GetData<ItemData>(meteorItemId);
            meteorImbueSpellData = Catalog.GetData<SpellCastCharge>(meteorImbueSpellId);
            statusEffect = Catalog.GetData<StatusData>(statusEffectId);
        }

    }
}
