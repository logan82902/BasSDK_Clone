using Newtonsoft.Json;
using TriInspector;
using UnityEngine;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Imbue Use", Title = "Imbue Use")]
    public class SkillGravityHammer : SpellSkillData
    {
        [Group("Imbue Use")]
        [Dropdown(nameof(GetAllEffectID))]
        public string whooshEffectId;
        [JsonIgnore]
        public EffectData whooshEffectData;

        [Group("Imbue Use")]
        public float minWhooshVelocity = 5;
        [Group("Imbue Use")]
        public float maxWhooshVelocity = 12;
        [Group("Imbue Use")]
        public float throwMult = 1f;
        [Group("Imbue Use")]
        public float jointPositionSpring = 1000;
        [Group("Imbue Use")]
        public float jointPositionDamper = 150;
        [Group("Imbue Use")]
        public float jointPositionMaxForce = 100000;
        [Group("Imbue Use")]
        public float jointRotationSpring = 1000;
        [Group("Imbue Use")]
        public float jointRotationDamper = 50;
        [Group("Imbue Use")]
        public float jointRotationMaxForce = 10000;
        [Group("Imbue Use")]
        public float ragdollMassModifier = 8f;
        [Group("Imbue Use")]
        public float imbueDrainItem = 3;
        [Group("Imbue Use")]
        public float imbueDrainRagdoll = 7;
        [Group("Imbue Use")]
        public float minDistance = 1f;

        public float minThrowVelocity = 2;

    }

    public class GravityHammer : ThunderBehaviour
    {
        public ThunderEntity heldEntity;
        public ConfigurableJoint joint;
        public SkillGravityHammer skill;
        public bool orgDisallowDespawn;
        public float heldEntitySize;
        public Rigidbody jointPoint;

    }
}
