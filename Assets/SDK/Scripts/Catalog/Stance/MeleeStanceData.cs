using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections;
using TriInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ThunderRoad
{
    [DeclareBoxGroup("Attacking", Title = "Attacking")]
    [DeclareFoldoutGroup("Parrying", Expanded = true)]
    [DeclareHorizontalGroup("Parrying/Upper")]
    [DeclareHorizontalGroup("Parrying/Mid")]
    [DeclareHorizontalGroup("Parrying/Lower")]
    [DeclareFoldoutGroup("Parrying", Expanded = false)]
    public class MeleeStanceData : StanceIdlesData<MeleeIdle>
    {
        [Group("Attacking")]
        public bool alternateAttackSides = false;
        [Group("Attacking")]
        public BodyChainBehaviour chainAttacksOnBody = BodyChainBehaviour.Disabled;
        [Group("Attacking")]
        [HideReferencePicker]
        [InlineProperty]
        [OnValueChanged(nameof(SetOpenNoMovement))]
        public List<AttackMotion> openingAttacks = new();
        [Group("Attacking")]
        [HideReferencePicker]
        [InlineProperty]
        [OnValueChanged(nameof(SetChainNoMovement))]
        public List<AttackMotion> chainAttacks = new();
        [Group("Parrying")]
        public BrainModuleDefense.StanceDefenseSettings defenseSettings;
        [Group("Parrying/Upper")]
        [HideLabel]
        [HideReferencePicker]
        [InlineProperty]
        public GuardPose upperLeftGuard = new() { id = "GuardUpperLeft" };
        [Group("Parrying/Upper")]
        [HideLabel]
        [HideReferencePicker]
        [InlineProperty]
        public GuardPose upperRightGuard = new() { id = "GuardUpperRight" };
        [Group("Parrying/Mid")]
        [HideLabel]
        [HideReferencePicker]
        [InlineProperty]
        public GuardPose midLeftGuard = new() { id = "GuardMidLeft" };
        [Group("Parrying/Mid")]
        [HideLabel]
        [HideReferencePicker]
        [InlineProperty]
        public GuardPose neutralGuard = new() { id = "GuardNeutral" };
        [Group("Parrying/Mid")]
        [HideLabel]
        [HideReferencePicker]
        [InlineProperty]
        public GuardPose midRightGuard = new() { id = "GuardMidRight" };
        [Group("Parrying/Lower")]
        [HideLabel]
        [HideReferencePicker]
        [InlineProperty]
        public GuardPose lowerLeftGuard = new() { id = "GuardLowerLeft" };
        [Group("Parrying/Lower")]
        [HideLabel]
        [HideReferencePicker]
        [InlineProperty]
        public GuardPose lowerRightGuard = new() { id = "GuardLowerRight" };
        [Group("Parrying")]
        [HideReferencePicker]
        [OnValueChanged(nameof(SetRiposteNoMovement))]
        public List<AttackMotion> ripostes = new();

        public enum BodyChainBehaviour { Disabled, UseChance, Always }

        private int openersLastCount = 0;

        private int chainsLastCount = 0;

        private int ripostesLastCount = 0;

        protected void SetOpenNoMovement() => SetNewestNoMovement(openingAttacks, ref openersLastCount);

        protected void SetChainNoMovement() => SetNewestNoMovement(chainAttacks, ref chainsLastCount);

        protected void SetRiposteNoMovement() => SetNewestNoMovement(ripostes, ref ripostesLastCount);

        protected void SetNewestNoMovement(List<AttackMotion> list, ref int lastCount)
        {
            if (lastCount == 0 && list.Count > 1)
            {
                lastCount = list.Count;
            }
            if (lastCount < list.Count)
            {
                list[list.Count - 1].allowPlayAndMove = false;
            }
            lastCount = list.Count;
        }

#if UNITY_EDITOR
        [Button]
        public void SetOpeningAttackZMotionCurves()
        {
            foreach (AttackMotion motion in openingAttacks)
            {
                if (motion.animationClip == null)
                {
                    motion.animationClip = Catalog.EditorLoad<AnimationClip>(motion.address);
                }
                foreach (EditorCurveBinding curve in AnimationUtility.GetCurveBindings(motion.animationClip))
                {
                    if (curve.propertyName == "RootT.z")
                    {
                        motion.rootZCurve = AnimationUtility.GetEditorCurve(motion.animationClip, curve);
                        break;
                    }
                }
            }
        }

        [Button]
        public void NormalizeAllAttackTimings()
        {
            foreach (StanceNode node in AllStanceNodes())
            {
                if (node.animationClip == null)
                {
                    continue;
                }
                if (node is AttackMotion attackMotion)
                {
                    attackMotion.start = attackMotion.start / attackMotion.animationClip.length;
                    attackMotion.peak = attackMotion.peak / attackMotion.animationClip.length;
                    attackMotion.end = attackMotion.end / attackMotion.animationClip.length;
                }
            }
        }
#endif

        public override IEnumerable<StanceNode> AllStanceNodes()
        {
            foreach (var node in base.AllStanceNodes())
                yield return node;
            for (int i = 0; i < openingAttacks.Count; i++)
                yield return openingAttacks[i];
            for (int i = 0; i < chainAttacks.Count; i++)
                yield return chainAttacks[i];
            yield return upperLeftGuard;
            yield return upperRightGuard;
            yield return midLeftGuard;
            yield return neutralGuard;
            yield return midRightGuard;
            yield return lowerLeftGuard;
            yield return lowerRightGuard;
            for (int i = 0; i < ripostes.Count; i++)
                yield return ripostes[i];
        }

    }
}
