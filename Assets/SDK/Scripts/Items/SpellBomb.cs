using System;
using System.Collections;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;
using UnityEngine.Events;

namespace ThunderRoad
{
    [AutoLabelWidth(AddedWidth = 5f)]
    [DeclareBoxGroup("Spell")]
    [DeclareBoxGroup("Arming")]
    [DeclareHorizontalGroup("Arming/FuseVFX")]
    [DeclareHorizontalGroup("Arming/Timer", Sizes = new float[] { 0f, 125f })]
    [DeclareBoxGroup("Detonating")]
    [DeclareHorizontalGroup("Detonating/Timer", Sizes = new float[] { 0f, 125f, 280f, 120f })]
    [DeclareHorizontalGroup("Detonating/Impact")]
    [DeclareHorizontalGroup("Detonating/Proximity", Sizes = new float[] { 125f, 0f })]
    [DeclareHorizontalGroup("Detonating/AltCauses", Sizes = new float[] { 195f, 0f })]
    [DeclareBoxGroup("Breakable")]
    public class SpellBomb : ThunderBehaviour
    {
        public static List<SpellBomb> activeBombs = new List<SpellBomb>();


        [AutoLabelWidth(MinWidth = 0f)]
        public Item item;

        [Group("Spell")]
        public bool getExplosionSpellFromImbue;
        [Group("Spell")]
        [ShowIf(nameof(getExplosionSpellFromImbue))]
        public ColliderGroup sourceGroup;
        [Group("Spell")]
        [HideIf(nameof(getExplosionSpellFromImbue))]
        [Dropdown(nameof(GetAllSpellCastChargeID))]
        public string explosionSpellId;

        [Group("Arming")]
        public ArmButton triggerAction = ArmButton.UseButton;
        [Group("Arming/FuseVFX")]
        public FuseVFXType playFuseVfx;
        [Group("Arming/FuseVFX")]
        public EffectSpawner fuseEffectSpawner;
        [Group("Arming")]
        public UnityEvent onTrigger;
        [Group("Arming")]
        public ArmDelay triggerToArmDelay = ArmDelay.Timer;
        [Group("Arming/Timer")]
        [ShowIf(nameof(triggerToArmDelay), ArmDelay.Timer)]
        public float armTimer = 3f;
        [Group("Arming/Timer")]
        [ShowIf(nameof(triggerToArmDelay), ArmDelay.Timer)]
        [LabelText("Pause while held")]
        public bool pauseArmTimerWhileHeld = true;
        [Group("Arming")]
        public UnityEvent onArm;
        [Group("Arming")]
        public UnityEvent onDisarm;

        [Group("Detonating")]
        public DetonateType detonateTrigger;
        [Group("Detonating/Timer")]
        [ShowIf(nameof(detonateTrigger), DetonateType.Timer)]
        public float detonateTimer = 3f;
        [Group("Detonating/Timer")]
        [ShowIf(nameof(detonateTrigger), DetonateType.Timer)]
        [LabelText("Pause while held")]
        public bool pauseDetonateTimerWhileHeld = true;
        [Group("Detonating/Timer")]
        [ShowIf(nameof(detonateTrigger), DetonateType.Timer)]
        public EffectSpawner timerTickEffect;
        [Group("Detonating/Timer")]
        [ShowIf(nameof(detonateTrigger), DetonateType.Timer)]
        public float tickLength = 1f;
        [Group("Detonating/Impact")]
        [ShowIf(nameof(detonateTrigger), DetonateType.Impact)]
        public float impactVelocity = 0f;
        [Group("Detonating/Impact")]
        [ShowIf(nameof(detonateTrigger), DetonateType.Impact)]
        public ObjectClassification impactObjects;
        [Group("Detonating/Proximity")]
        [ShowIf(nameof(detonateTrigger), DetonateType.CreatureProximity)]
        public bool includePlayer = false;
        [Group("Detonating/Proximity")]
        [ShowIf(nameof(detonateTrigger), DetonateType.CreatureProximity)]
        public float proximityDistance = 1f;
        [Group("Detonating/AltCauses")]
        public bool explodeInstantlyIfBroken = true;
        [Group("Detonating/AltCauses")]
        public float forceExplodeTime = 0f;
        [Group("Detonating")]
        public UnityEvent onExplode;

        [Group("Breakable")]
        [ShowIf(nameof(isBreakable))]
        public bool explodeBreakable = true;
        [Group("Breakable")]
        [ShowIf(nameof(isBreakable))]
        public float breakableExplodeForce = 50f;
        [Group("Breakable")]
        [ShowIf(nameof(isBreakable))]
        public ForceMode breakableExplodeForceMode = ForceMode.Impulse;
        private bool isBreakable => item?.breakable != null || item.GetComponent<SimpleBreakable>() != null;

        public SpellBombState currentState { get; protected set; } = SpellBombState.Idle;

        public enum SpellBombState
        {
            Idle,
            Triggered,
            Armed,
            Exploded,
        }

        public enum ArmButton
        {
            None,
            UseButton,
            AltUseButton,
        }

        public enum ArmDelay
        {
            Instant,
            Timer,
            Manual,
        }

        public enum FuseVFXType
        {
            Never,
            OnTrigger,
            OnArm,
        }

        public enum DetonateType
        {
            Timer,
            Impact,
            CreatureProximity,
            Manual,
        }

        public TriDropdownList<string> GetAllSpellCastChargeID() => Catalog.GetDropdownAllID<SpellCastCharge>();

        public TriDropdownList<string> GetAllEffectID() => Catalog.GetDropdownAllID<EffectData>();

        private void OnValidate()
        {
            item ??= GetComponent<Item>();
        }

        public void TriggerArm()
        {
        }

        public void SetArmed()
        {
        }

        public void SetArmedAfter(float time)
        {
        }

        public void Disarm()
        {
        }

        public void TryExplode()
        {
        }

        public void ForceExplode()
        {
        }


    }
}
