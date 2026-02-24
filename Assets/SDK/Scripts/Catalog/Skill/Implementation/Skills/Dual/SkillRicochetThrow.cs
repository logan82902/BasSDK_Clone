using System;
using TriInspector;
using ThunderRoad.Skill.SpellPower;
using UnityEngine;

namespace ThunderRoad.Skill
{
    [DeclareBoxGroup("Ricochet", Title = "Ricochet")]
    [DeclareBoxGroup("Throw", Title = "Throw")]
    [DeclareBoxGroup("Return", Title = "Return")]
    [DeclareBoxGroup("Effect", Title = "Effect")]
    public class SkillRicochetThrow : TkSkillData
    {
        [Group("Ricochet")]
        public float maxDistance = 10f;
        [Group("Ricochet")]
        public float maxAngle = 50f;
        [Group("Ricochet")]
        public int maxBounces = 1;
        [Group("Ricochet")]
        public float maxRicochetVelocity = 20;
        [Group("Ricochet")]
        public float velocityHitMultiplier = 0.9f;
        [Group("Ricochet")]
        public float additionalDamageMult = 2;
        [Group("Ricochet")]
        public float nonAimAssistVelocityMult = 0.8f;
        [Group("Throw")]
        public float throwVelocityMultiplier = 1.3f;
        [Group("Throw")]
        public float maxThrowDistance = 10f;
        [Group("Throw")]
        public float maxThrowAngle = 30f;

        [Group("Return")]
        public bool returnOnLastBounce = true;

        [Group("Return")]
        [ShowIf("returnOnLastBounce")]
        public float returnSpeed = 10f;

        [Group("Return")]
        [ShowIf("returnOnLastBounce")]
        public float autoGrabRadius = 2f;

        [Group("Return")]
        [ShowIf("returnOnLastBounce")]
        public float grabWindow = 0.6f;

        [Group("Return")]
        [Dropdown(nameof(GetAllEffectID))]
        public string returnEffectId;

        [Group("Effect")]
        [Dropdown(nameof(GetAllEffectID))]
        public string grabEffectId;

        [NonSerialized]
        public EffectData grabEffectData;
        
        [NonSerialized]
        public EffectData returnEffectData;
    }
}
