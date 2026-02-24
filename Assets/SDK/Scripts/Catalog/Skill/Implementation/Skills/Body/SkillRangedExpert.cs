using UnityEngine;
using System.Collections;

using TriInspector;

namespace ThunderRoad.Skill
{
    [DeclareBoxGroup("Piercing", Title = "Piercing")]
    [DeclareBoxGroup("Effect", Title = "Effect")]
    [DeclareBoxGroup("Ricochet", Title = "Ricochet")]
    [DeclareBoxGroup("Aim Assist", Title = "Aim Assist")]
    public class SkillRangedExpert : SkillData
    {
        [Group("Piercing")]
        public float penetrationVelocity = 0.3f;
        [Group("Piercing")]
        public float penetrationMaxVelocity = 30;
        [Group("Piercing")]
        public bool pierceOnlyRigidbodies = true;

        [Group("Effect")]
        [Dropdown("GetAllEffectID")]
        public string pierceEffectId = "RangedExpertisePierce";
        protected EffectData pierceEffectData;

        [Group("Ricochet")]
        public bool bounceOffTerrain = true;
        [Group("Ricochet")]
        public float maxRicochetVelocity = 20;
        [Group("Ricochet")]
        public float velocityHitMultiplier = 0.9f;
        [Group("Ricochet")]
        public float nonAimAssistVelocityMult = 0.8f;
        [Group("Aim Assist")]
        public float maxAngle = 30;

        [Group("Aim Assist")]
        public float maxDistance = 12;

        [Group("Aim Assist")]
        public bool aimAssistOnThrow = false;

        [Group("Aim Assist")]
        [ShowIf(nameof(aimAssistOnThrow))]
        public float aimAssistMaxDistance = 5;

        [Group("Aim Assist")]
        [ShowIf(nameof(aimAssistOnThrow))]
        public float aimAssistMaxAngle = 20;

    }
}
