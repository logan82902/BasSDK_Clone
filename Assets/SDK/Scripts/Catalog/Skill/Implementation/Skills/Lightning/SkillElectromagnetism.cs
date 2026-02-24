
using UnityEngine;
using TriInspector;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Homing", Title = "Homing")]
    public class SkillElectromagnetism : SpellSkillData
    {
        [Group("Homing")]
        public float lerpAmount = 10;
        [Group("Homing")]
        public float homingForce = 1;
        [Group("Homing")]
        public float homingAngle = 15f;
        [Group("Homing")]
        public float homingDistance = 5f;
        [Group("Homing")]
        public float lerpAmountNpc = 10;

 //ProjectCore
    }

    public class Electromagnet : ThunderBehaviour
    {
        public override ManagedLoops EnabledManagedLoops => ManagedLoops.FixedUpdate | ManagedLoops.Update;
        public Item item;
        public bool attracting;
        public Transform target;
        public SkillElectromagnetism skill;
        public float lastThrow;
        public bool ensureLightningImbue = true;

    }
}