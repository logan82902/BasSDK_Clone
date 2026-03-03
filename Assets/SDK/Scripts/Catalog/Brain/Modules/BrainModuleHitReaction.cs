using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TriInspector;

namespace ThunderRoad
{
    [System.Serializable]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    [DeclareBoxGroup("General", Title = "General")]
    [DeclareBoxGroup("Behaviours", Title = "Behaviours")]
    [DeclareBoxGroup("Resistances", Title = "Resistances")]
    public class BrainModuleHitReaction : BrainData.Module
    {
        [Group("General")]
        public bool ignoreDamagingCreature = false;
        [Group("General")]
        public float recoverySpeed = 1f;
        [Group("General")]
        public float parryRecoilCooldown = 1f;
        [Group("General")]
        public float waitPhysicDelay = 0.2f;
        [Group("General")]
        public float speedMultWhileWait = -0.5f;
        [Group("General")]
        public float staggerLight = 0.2f;
        [Group("General")]
        public float staggerMedium = 0.6f;
        [Group("General")]
        public float jumpFootHeightThreshold = 0.1f;
        [Group("General")]
        public string stunAnimationID = "HumanStun";
        [Group("General")]
        public bool cancelGetUp = true;

        [Group("Behaviours")]
        [HideReferencePicker]
        [SerializeReference]
        [TableList(AlwaysExpanded = true)]
        public List<PushBehaviour> pushMagicBehaviors = new();
        [Group("Behaviours")]
        [HideReferencePicker]
        [SerializeReference]
        [TableList(AlwaysExpanded = true)]
        public List<PushBehaviour> pushGrabThrowBehaviors = new();
        [Group("Behaviours")]
        [HideReferencePicker]
        [SerializeReference]
        [TableList(AlwaysExpanded = true)]
        public List<PushBehaviour> pushHitBehaviors = new();

        [NonSerialized]
        public float maxPushMagicBehaviourLevel;
        [NonSerialized]
        public float maxPushGrabThrowBehaviourLevel;
        [NonSerialized]
        public float maxPushHitBehaviourLevel;

        [Group("Resistances")]
        public bool resistDestabilizeFromMagic;

        [Serializable]
        public class PushBehaviour
        {
            [HideInInspector]
            [JsonMergeKey]
            public string mergeKey
            {
                // for the merge key, we want it to be the level + a representation of all different part types in the list
                // if the list has every body type, it should be -1, which would result in a merge key like "1:-1", which is fine since it's just a string and isn't used anywhere else
                get
                {
                    RagdollPart.Type combinedTypeEnum = 0;
                    foreach (var type in onlyForBodyParts)
                    {
                        combinedTypeEnum |= type;
                    }
                    return $"{level}-{(int)combinedTypeEnum}";
                }
            }

            [HideInInspector]
            [JsonIgnore]
            public int sortKey
            {
                // push level is multiplied by 100 so it's the primary sort component, then we add the number of parts to act as a secondary sort component
                // the parts count should never be greater than 99, but we clamp it just in case something bad happens
                get => (level * 100) + Mathf.Clamp(onlyForBodyParts.Count, 0, 99);
            }

            public int level;

            public List<RagdollPart.Type> onlyForBodyParts = new();

            public Effect duringIdle = Effect.Destabilize;
            public Effect duringCombat = Effect.StaggerLight;
            public Effect duringStagger = Effect.StaggerMedium;
            public Effect duringStaggerFull = Effect.StaggerFull;
            public Effect duringAttack = Effect.StaggerMedium;
            public Effect duringAttackJump = Effect.Destabilize;

            public enum Effect
            {
                None,
                StaggerLight,
                StaggerMedium,
                StaggerFull,
                Destabilize,
            }
        }

    }
}