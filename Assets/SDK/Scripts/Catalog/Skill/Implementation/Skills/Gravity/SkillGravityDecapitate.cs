using TriInspector;
using System.Collections.Generic;
using UnityEngine;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Push Decapitate", Title = "Push Decapitate")]
    [DeclareBoxGroup("Effect", Title = "Effect")]
    [DeclareBoxGroup("Backup Push Force", Title = "Backup Push Force")]
    public class SkillGravityDecapitate : SpellSkillData
    {
        protected static Collider[] colliders = new Collider[200];
        [Group("Push Decapitate")]
        public float minCharge = 0.95f;
        [Group("Push Decapitate")]
        public LayerMask layerMask;
        [Group("Push Decapitate")]
        public float maxDistance = 0.2f;

        [Group("Push Decapitate")]
        public float pushedPartVelocityMult = 0.4f;
        [Group("Effect")]
        [Dropdown(nameof(GetAllEffectID))]
        public string effectId = "SpellGravityPushDecapitate";
        protected EffectData effectData;

        [Group("Backup Push Force")]
        public float pushForce = 3f;

        [Group("Backup Push Force")]
        public ForceMode pushForceMode = ForceMode.VelocityChange;

    }
}
