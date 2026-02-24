using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Net;
using UnityEngine.Profiling;
using UnityEngine.Serialization;


using TriInspector;

namespace ThunderRoad
{
    [HelpURL("https://kospy.github.io/BasSDK/Components/ThunderRoad/Levels/Zone.html")]
    [DeclareTabGroup("tabs")]
    [DeclareToggleGroup("tabs/effects", Title = "Effects")]
    [DeclareFoldoutGroup("tabs/multipliers", Title = "Force Multipliers")]
    [DeclareToggleGroup("tabs/linear", Title = "Linear Force")]
    [DeclareToggleGroup("tabs/radial", Title = "Radial Force")]
    [DeclareToggleGroup("tabs/swirl", Title = "Swirl Force")]
    [DeclareToggleGroup("tabs/resistive", Title = "Resistive Force")]
    [DeclareFoldoutGroup("tabs/player", Title = "player")]
    [DeclareFoldoutGroup("tabs/creature", Title = "creature")]
    [DeclareFoldoutGroup("tabs/item", Title = "item")]
    [DeclareFoldoutGroup("tabs/golem", Title = "golem")]
    [DeclareFoldoutGroup("tabs/entity", Title = "entity")]
    public class Zone : ThunderBehaviour
    {
        [System.Serializable]
        public class ZoneEvent : UnityEvent<UnityEngine.Object> { }

        [Tooltip("Causes the Player Exit event to be invoked immediately when the zone is loaded into the level.")]
        public bool invokePlayerExitOnAwake;

        [GroupNext("tabs")]
        [Header("Creature settings")]
        [Tab("General")]
        [Tooltip("If enabled, this zone will not work with the player creature.")]
        public bool ignorePlayerCreature = false;
        [Tab("General")]
        [Tooltip("If enabled, the zone will only react to the root bone of creatures (hip bone).")]
        public bool ignoreNonRootParts = true;
        [Tab("General")]
        [Tooltip("If enabled, this zone will not react to creatures who are idle.")]
        public bool ignoreIdleCreatures = false;
        [Tab("General")]
        [Tooltip("If enabled, creatures in or touching this zone will have physics culling forced off.")]
        public bool blockPhysicsCulling = false;
        [Tab("General")]
        public Ragdoll.State physicalCreatureState = Ragdoll.State.Destabilized;

        [Header("Kill")]
        [Tab("General")]
        [Tooltip("When player enter the zone, the player dies.")]
        public bool killPlayer;
        [Tab("General")]
        [Tooltip("When NPC enter the zone, the NPC dies.")]
        public bool killNPC;

        [Header("Despawn")]
        [Tab("General")]
        [Tooltip("When an Item enters the zone, the item despawns.")]
        public bool despawnItem;
        [Tab("General")]
        [Tooltip("When NPC enters the zone, the NPC despawns.")]
        public bool despawnNPC;
        [Tab("General")]
        [Tooltip("Delay of which the NPC/Item despawns once they enter the zone.")]
        public float despawnDelay = 0;

        [Header("Statuses")]
        [Tab("General")]
        [Tooltip("If true, inflicts the status effects constantly to anything in the zone.")]
        public bool constantStatus;

        [Tab("General")]
        [Tooltip("Status Effect IDs to inflict/remove on entry/exit")]
        [ShowInInspector]
        public List<StatusEntry> statusIDs;

        [Tab("General")]
        [Tooltip("When NPC/Item is in zone, does it apply a status effect?")]
        public bool playStatusEffects = true;
        [Tab("General")]
        [Tooltip("Does the status affect Players?")]
        public bool statusOnPlayer;
        [Tab("General")]
        [Tooltip("Does the status affect Creatures?")]
        public bool statusOnCreature;
        [Tab("General")]
        [Tooltip("Does the status affect Items?")]
        public bool statusOnItem;
        [Tab("General")]
        [Tooltip("When enabled, it will disable armor detection on NPCs inside the zone.")]
        public bool disableArmorDetection = false;
        [NonSerialized]
        public Dictionary<StatusData, (float, float?)> statuses;

        [Header("Breakables")]
        [Tab("General")]
        public bool breakBreakables = false;

        [Space]
        [Header("Portals")]
        [Tab("General")]
        public List<ZonePortal> portals = new();

        [Space]
        [Header("Effects")]
        [GroupNext("tabs/effects")]
        [Tab("General")]
        [Tooltip("Spawns Effect when the zone is entered by Player/NPC/Item")]
        public bool spawnEffect;
        [Tab("General")]
        [Tooltip("ID of the effect spawned.")]
        public string effectID;
        [Tab("General")]
        [Tooltip("This curve maps the (mass * velocity) of the interactor at the moment of entry to the intensity of the effect.")]
        public AnimationCurve effectMassVelocityCurve;
        [Tab("General")]
        [Tooltip("The euler rotation of the spawned effect.")]
        public Vector3 effectOrientation = Vector3.forward;
        [Tab("General")]
        [FormerlySerializedAs("orientToEntranceXZNormal")]
        [Tooltip("Whether to orient the effect to point outwards from the point of exit, projected onto the XZ plane.")]
        public bool orientEffectToEntranceXZNormal = false;

        [GroupNext("tabs")]
        [Tab("Movement")]
        [Header("Movement speed")]
        [Tooltip("When ticked, Adjusts the speed of the player/NPC when inside the zone.")]
        public bool navSpeedModifier;
        [Tab("Movement")]
        [Tooltip("Speed adjustment of the player/NPC when inside the zone")]
        public float runSpeed = 0;

        [Tab("Movement")]
        [Header("Teleportation")]
        [Tooltip("Teleports player to the Custom Teleport Target when player enters zone.")]
        public bool teleportPlayer;
        [Tab("Movement")]
        [Tooltip("Whether or not to keep the player's velocity when they teleport.")]
        public bool keepPlayerVelocity;
        [Tab("Movement")]
        [Tooltip("Teleports item(s) to the Custom Teleport Target when the item(s) enter the zone")]
        public bool teleportItem;
        [Tab("Movement")]
        [Tooltip("Whether or not to keep an item's velocity when it teleports.")]
        public bool keepItemVelocity;
        [Tab("Movement")]
        [Tooltip("GameObject of which the position is where the player/item teleports to.")]
        public Transform customTeleportTarget;

        public enum CreatureForceMode
        {
            NoForce,
            ForceRoot,
            ForceParts,
        }

        [Tab("Force")]
        [Tooltip("Does the applied force affect non-creatures?")]
        public bool forceNonCreatures;
        [Tab("Force")]
        public List<string> forceIgnoredItemIds;
        [Tab("Force")]
        public List<ItemData.Type> forceIgnoredItemTypes;
        [Tab("Force")]
        [Tooltip("Does the applied force affect the player?")]
        public bool forcePlayer;
        [Tab("Force")]
        [Tooltip("How should the zone apply force to NPCs?\n\nNo Force: Applies no force\n\nForce Root: Applies force on the root of the creature\n\nForce Parts: Applies force to all parts of the creature. This will distabilize creatures beforehand")]
        public CreatureForceMode creatureForceMode = CreatureForceMode.NoForce;
        [Tab("Force")]
        [Tooltip("Sets the origin point and orientation for forces applied by this zone")]
        public Transform forceTransform;
        [Tab("Force")]
        [Tooltip("A secondary zone; if an object is in this zone, it doesn't receive force from this zone")]
        public Zone forceExclusionZone;

        [GroupNext("tabs/multipliers")]
        [Tab("Force")]
        [Tooltip("Cancel the existing velocity of incoming items")]
        public bool cancelItemVelocity = false;
        [Tab("Force")]
        [Tooltip("Cancel the existing velocity of incoming NPCs")]
        public bool cancelCreatureVelocity = false;
        [Tab("Force")]
        [Tooltip("Cancel the existing velocity of incoming player")]
        public bool cancelPlayerVelocity = false;

        [Tab("Force")]
        [Tooltip("Multiply the velocity of an item on enter")]
        public float itemVelocityMultOnEnter = 1;
        [Tab("Force")]
        [Tooltip("Multiply the velocity of an NPC on enter")]
        public float creatureVelocityMultOnEnter = 1;
        [Tab("Force")]
        [Tooltip("Multiply the velocity of the player on enter")]
        public float playerVelocityMultOnEnter = 1;

        [Tab("Force")]
        [Tooltip("An additional multiplier on force applied to items")]
        public float itemForceMult = 1;
        [Tab("Force")]
        [Tooltip("An additional multiplier on force applied to NPCs")]
        public float creatureForceMult = 1;
        [Tab("Force")]
        [Tooltip("An additional multiplier on force applied to the player")]
        public float playerForceMult = 1;

        [GroupNext("tabs/linear")]
        [Tab("Force")]
        [Tooltip("Does the force push the object in a linear direction? (X, Y, Z)")]
        public bool linearForceActive;
        [Tab("Force")]
        [Tooltip("What force mode does the linear force apply?")]
        public ForceMode linearForceMode = ForceMode.Force;
        [Tab("Force")]
        [Tooltip("What direction (relative to the force transform) does the linear force push the object?")]
        public Vector3 linearForce;

        [GroupNext("tabs/radial")]
        [Tab("Force")]
        [Tooltip("Does the force push outwards from the force transform point?")]
        public bool radialForceActive;
        [Tab("Force")]
        [Tooltip("Should the radial vector be projected onto a plane with normal of the Swirl Local Axis")]
        public bool radialForceProjectOnSwirlPlane;
        [Tab("Force")]
        [Tooltip("What force mode does the radial force apply?")]
        public ForceMode radialForceMode = ForceMode.Force;
        [Tab("Force")]
        [Tooltip("Output radial force by distance from origin\n\nVertical axis: Force\n\nHorizontal axis: Distance")]
        public AnimationCurve radialForce = new(new Keyframe(0f, 1f));
        [Tab("Force")]
        [Tooltip("Output radial force gets multiplied by this value")]
        public float radialForceMultiplier = 1f;
        [Tab("Force")]
        [Tooltip("Should downwards force be negated when applying radial force?")]
        public bool noDownwardsForce = false;

        [GroupNext("tabs/swirl")]
        [Tab("Force")]
        [Tooltip("Does the force try to swirl objects around the force transform point?")]
        public bool swirlForceActive;
        [Tab("Force")]
        [Tooltip("What force mode does the swirl force force apply?")]
        public ForceMode swirlForceMode = ForceMode.Force;
        [Tab("Force")]
        [Tooltip("Output swirl force by distance from origin\n\nVertical axis: Force\n\nHorizontal axis: Distance")]
        public AnimationCurve swirlForce = new(new Keyframe(0f, 1f));
        [Tab("Force")]
        [Tooltip("Output swirl force gets multiplied by this value")]
        public float swirlForceMultiplier = 1f;
        [Tab("Force")]
        [Tooltip("Picks a random direction (forwards or backwards) for every object that gets pushed by the swirl force")]
        public bool swirlRandomDirection;
        [Tab("Force")]
        [Tooltip("How far objects should be pulled when swirled")]
        public float swirlForceDegrees;
        [Tab("Force")]
        [Tooltip("Determines the axis around which objects are swirled, relative to the force transform.")]
        public Vector3 swirlLocalAxis = Vector3.up;

        [GroupNext("tabs/resistive")]
        [Tab("Force")]
        [Tooltip("Does the force resist an object's velocity?")]
        public bool resistiveForceActive;
        [Tab("Force")]
        [Tooltip("The amount of force to apply opposite to an object's movement")]
        public float resistiveForce;
        [Tab("Force")]
        [Tooltip("What force mode does the resistive force apply?")]
        public ForceMode resistiveForceMode = ForceMode.Force;
        [Tab("Force")]
        [Tooltip("A minimum velocity for the resistive force to activate")]
        public float minimumResistedVelocity = 0f;
        [Tab("Force")]
        [Tooltip("Should the resistive force only work in one direction?" + "\n\nIf true, this allows the resistance force direction to update with the force transform direction" + "\n\nIf false, use the direction specified in resistive force direction")]
        public bool resistInForceTransformForward;
        [Tooltip("Should the resistive force resist all movement, no matter the direction?")]
        [Tab("Force")]
        public bool resistInAllDirections = false;
        [Tab("Force")]
        [Tooltip("Should the resistive force only work in one direction? Only works if the magnitude of this vector is greater than 0")]
        public Vector3 resistiveForceDirection = Vector3.zero;

        [GroupNext("tabs/player")]
        [Tab("Events")]
        public ZoneEvent playerEnterEvent = new();
        [Tab("Events")]
        public ZoneEvent playerExitEvent = new();
        [GroupNext("tabs/creature")]
        [Tab("Events")]
        public ZoneEvent creatureEnterEvent = new();
        [Tab("Events")]
        public ZoneEvent creatureExitEvent = new();
        [Tab("Events")]
        public ZoneEvent firstCreatureEnterEvent = new();
        [Tab("Events")]
        public ZoneEvent lastCreatureExitEvent = new();
        [GroupNext("tabs/item")]
        [Tab("Events")]
        public ZoneEvent itemEnterEvent = new();
        [Tab("Events")]
        public ZoneEvent itemExitEvent = new();
        [Tab("Events")]
        public ZoneEvent firstItemEnterEvent = new();
        [Tab("Events")]
        public ZoneEvent lastItemExitEvent = new();
        [GroupNext("tabs/golem")]
        [Tab("Events")]
        public ZoneEvent golemEnterEvent = new();
        [Tab("Events")]
        public ZoneEvent golemExitEvent = new();
        [GroupNext("tabs/entity")]
        [Tab("Events")]
        public ZoneEvent firstEntityEnterEvent = new();
        [Tab("Events")]
        public ZoneEvent lastEntityExitEvent = new();

        protected Collider mainCollider;

        public void ForceCreatureEnterZone(Creature creature)
        {
        }

        public void ForceCreatureExitZone(Creature creature)
        {
        }

        public void ForceCreatureEnterZoneRagdoll(Creature creature)
        {
        }

        public void ForceCreatureExitZoneRagdoll(Creature creature)
        {
        }

        public void ForceItemEnterZone(Item item)
        {
        }

        public void ForceItemExitZone(Item item)
        {
        }

    }
}