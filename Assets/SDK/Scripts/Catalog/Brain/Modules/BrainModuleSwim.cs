using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TriInspector;


#if UNITY_EDITOR
using UnityEditor;
#endif
namespace ThunderRoad
{
    [DeclareBoxGroup("Recovery", Title = "Destabilized recovery")]
    [DeclareBoxGroup("Forces", Title = "Vertical swimming forces")]
    [DeclareBoxGroup("Endurance", Title = "Swimming endurance")]
    [DeclareBoxGroup("Navigation", Title = "Swim navigation")]
    public class BrainModuleSwim : BrainData.Module
    {
        public bool active = true;
        [Group("Recovery")]
        [Tooltip("How long it takes the creature to stabilize itself to upright before it can swim upwards.")]
        public float timeToStabilize = 3f;
        [Group("Recovery")]
        [Tooltip("How fast (in degrees per second) the creature can rotate its body to become upright.")]
        public float rightingAngularSpeed = 15f;
        [Group("Recovery")]
        [Tooltip("The minimum angle difference from upright required for the creature to fully stabilize and change ragdoll state.")]
        public float angleToChangeState = 6f;
        [Group("Recovery")]
        [Tooltip("The minimum impact velocity required to reset the swim state.")]
        public float impactVelocityToResetSwim = 2f;
        [Group("Forces")]
        [Tooltip("The amount of force upwards (in newtons) applied to the creature to swim up to the surface.")]
        public float surfacingForce = 250f;
        [Group("Forces")]
        [Tooltip("The amount of force upwards (in newtons) applied to the creature to swim at the surface of the water.")]
        public float treadWaterForce = 110f;
        [Group("Forces")]
        [Tooltip("The maximum vertical velocity this creature should be at when treading water, to prevent it from pushing itself up out of the water like a dolphin.")]
        public float treadMaxVerticalVelocity = 0.21f;
        [Group("Forces")]
        [Tooltip("The amount of force downwards (in newtons) applied to the creature if it should sink and drown.")]
        public float sinkingForce = 50f;
        [Group("Endurance")]
        [Tooltip("How long the creature can tread in place before it sinks.")]
        public float maxTreadTime = 30f;
        [Group("Endurance")]
        [Tooltip("Whether or not the creature can swim for an unlimited time while swimming to a location.")]
        public bool unlimitedSwimWhilePathing = true;
        [Group("Endurance")]
        [Tooltip("Whether or not the creature's swimming time is limited if it's a wave creature and no action has happened recently.")]
        public bool limitedByNoAction = true;
        [Group("Navigation")]
        [Tooltip("Whether or not a creature with this brain can swim to shore, or if they just surface and tread water.")]
        public bool canSwimToLand = true;
        [Group("Navigation")]
        [Tooltip("How long after trying to find a path the creature will try to find a new one.")]
        public float repathDelay = 1f;
        [Group("Navigation")]
        [Tooltip("The force with which this creature swims in its desired direction.")]
        public float swimDirectionForce = 170f;
        [Group("Navigation")]
        [Tooltip("How fast the creature rotates to face its swim direction.")]
        public float swimRotationSpeed = 60f;

    }
}
