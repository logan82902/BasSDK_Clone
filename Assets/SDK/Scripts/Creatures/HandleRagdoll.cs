using System;
using ThunderRoad.Skill.SpellPower;
using UnityEngine;
using System.Collections;

using TriInspector;

namespace ThunderRoad
{
    [HelpURL("https://kospy.github.io/BasSDK/Components/ThunderRoad/HandleRagdoll")]
    [AddComponentMenu("ThunderRoad/Creatures/Handle ragdoll")]
    [DeclareBoxGroup("Ragdoll Handle")]
    public class HandleRagdoll : Handle
    {
        
        public static float combatScaleChangeTime = 0.75f;

        [Group("Ragdoll Handle")]
        public bool canBeEscaped = false;
        [Group("Ragdoll Handle")]
        public int grappleEscapeParameterValue = 0;
        [Group("Ragdoll Handle")]
        public float escapeDelay = 0f;
    }
}
