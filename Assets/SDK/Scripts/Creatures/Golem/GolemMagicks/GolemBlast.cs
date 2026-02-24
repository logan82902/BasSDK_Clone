using System;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;

namespace ThunderRoad
{
    [CreateAssetMenu(menuName = "ThunderRoad/Creatures/Golem/Blast config")]
    [DeclareBoxGroup("General", Title = "General")]
    [DeclareBoxGroup("Blast Placement", Title = "Blast Placement")]
    [DeclareBoxGroup("Blast Effects", Title = "Blast Effects")]
    [DeclareBoxGroup("Blast Damage", Title = "Blast Damage")]
    public class GolemBlast : GolemAbility
    {
#if UNITY_EDITOR
        [Group("General")]
#endif
        public GolemController.AttackMotion motion;
#if UNITY_EDITOR
        [Group("Blast Placement")]
#endif
        public Vector3 blastLocalPosition;
#if UNITY_EDITOR
        [Group("Blast Placement")]
#endif
        public bool attachToBone = false;
#if UNITY_EDITOR
        [Group("Blast Placement")]
#endif
        public HumanBodyBones blastLinkedBone = HumanBodyBones.UpperChest;
#if UNITY_EDITOR
        [Group("Blast Placement")]
#endif
        public float blastRadius = 5f;
#if UNITY_EDITOR
        protected TriDropdownList<string> GetAllEffectID() => Catalog.GetDropdownAllID(Category.Effect);

        [Group("Blast Effects")]
        [Dropdown(nameof(GetAllEffectID))]
#endif
        public string blastEffectID = "";
#if UNITY_EDITOR
        [Group("Blast Effects")]
#endif
        public Vector3 effectEulers = Vector3.zero;
#if UNITY_EDITOR
        [Group("Blast Damage")]
#endif
        public bool kickPlayerOff = false;
#if UNITY_EDITOR
        [Group("Blast Damage")]
#endif
        public bool damageBreakables = false;
#if UNITY_EDITOR
        [Group("Blast Damage")]
#endif
        public LayerMask blastMask;
#if UNITY_EDITOR
        [Group("Blast Damage")]
#endif
        public float blastDamage = 5f;
#if UNITY_EDITOR
        [Group("Blast Damage")]
#endif
        public float blastForce = 5f;
#if UNITY_EDITOR
        [Group("Blast Damage")]
#endif
        public float blastForceUpwardMult = 1.5f;
#if UNITY_EDITOR
        [Group("Blast Damage")]
#endif
        public ForceMode blastForceMode = ForceMode.VelocityChange;
#if UNITY_EDITOR
        [Group("Blast Damage")]
#endif
        public List<Golem.InflictedStatus> appliedStatuses = new();
#if UNITY_EDITOR
        [InlineProperty]
        [HideLabel]
        [ShowIf("$type", GolemAbilityType.Melee)]
#endif
        public Golem.AttackRange attackRange;

    }
}
