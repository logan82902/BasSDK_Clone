using System.Collections;
using TriInspector;
using ThunderRoad.Skill.SpellMerge;
using UnityEngine;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Status Effect", Title = "Status Effect")]
    [DeclareBoxGroup("Velocity", Title = "Velocity")]
    public class SkillDilationBubble : SpellSkillData
    {
        [Group("Status Effect")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string statusId;

        [Group("Velocity")]
        public float storedVelocityMult = 1.5f;

    }

    public class VelocityStorer : ThunderBehaviour
    {
        public Item item;
        public Vector3 velocity;
        public float multiplier = 1;
        public bool active;
        public bool waitingForThrow;

    }
}