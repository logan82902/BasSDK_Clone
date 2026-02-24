using System;
using UnityEngine;


using TriInspector;

namespace ThunderRoad
{
    [DeclareBoxGroup("General", Title = "General")]
    public abstract class GolemAbility : ScriptableObject
    {
        [Group("General")]
        public GolemAbilityType type = GolemAbilityType.Ranged;
        [Group("General")]
        public float weight = 1f;
        [Group("General")]
        public RampageType rampageType;

        [Group("General")]
        public Golem.Tier abilityTier = Golem.Tier.Any;

        [Group("General")]
        public bool stunOnExit;
        [Group("General")]
        [ShowIf(nameof(stunOnExit))]
        public float stunDuration = 0;

    }

    public enum GolemAbilityType
    {
        Climb, // Anything that triggers when something is climbing on the golem
        Melee, // Anything that triggers if the player is nearby the golem
        Ranged // Anything that triggers when the player is too far, and typically uses some kind of targeting
    }

    public enum RampageType
    {
        None,
        Melee,
        Ranged,
    }
}
