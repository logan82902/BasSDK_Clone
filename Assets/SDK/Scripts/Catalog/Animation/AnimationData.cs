using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using TriInspector;

namespace ThunderRoad
{
#if UNITY_EDITOR
    [EditorLoadPriority(5)]
#endif
    [Serializable]
    [DeclareBoxGroup("Death Settings", Title = "Death Settings")]
    [DeclareHorizontalGroup("Death Settings/Horiz")]
    public class AnimationData : CatalogData
    {
        public TriDropdownList<string> GetAllAnimationDataID() => Catalog.GetDropdownAllID(Category.Animation);

        [HideReferencePicker]
        [OnValueChanged(nameof(CalculateWeight))]
        public List<Clip> animationClips = new();

        [Group("Death Settings/Horiz")]
        [LabelWidth(100)]
        public bool useDeathCurve = true;

        [Group("Death Settings/Horiz")]
        [LabelWidth(110)]
        public bool allowRootMotion = false;

        private float probabilityRNGTotalWeight, probabilityDeltaTotalWeight;



        [Serializable]
        [DeclareHorizontalGroup("MasterHoriz")]
        [DeclareBoxGroup("MasterHoriz/Default", Title = "Default")]
        [DeclareHorizontalGroup("MasterHoriz/Default/Horiz")]
        [DeclareBoxGroup("MasterHoriz/Config", Title = "Config")]
        [DeclareHorizontalGroup("MasterHoriz/Config/Horiz1")]
        [DeclareHorizontalGroup("MasterHoriz/Config/Horiz2")]
        [DeclareBoxGroup("MasterHoriz/RNG", Title = "RNG")]
        [DeclareHorizontalGroup("MasterHoriz/RNG/Horiz1")]
        [DeclareHorizontalGroup("MasterHoriz/RNG/Horiz2")]
        public class Clip
        {
            [GUIColor(nameof(EditorGetColor))]
            [Group("MasterHoriz/Default")]
            [LabelWidth(50)]
            [JsonMergeKey]
            public string address;
            [GUIColor(nameof(EditorGetColor))]
            [Group("MasterHoriz/Default/Horiz")]
            [LabelWidth(100)]
            [ShowInInspector]
            [DisableInPlayMode]
#if UNITY_EDITOR
            [OnValueChanged(nameof(EditorUpdateAddress))]
#endif

            [NonSerialized]
            public AnimationClip animationClip;
            [Group("MasterHoriz/Default/Horiz")]
            [LabelWidth(30)]
            public Step step = Step.Start;
            [Group("MasterHoriz/Default/Horiz")]
            [LabelWidth(70)]
            [LabelText("Anim Speed")]
            public float animationSpeed = 1;

            [Group("MasterHoriz/Config/Horiz1")]
            [LabelWidth(100)]
            public Direction moveDirection = Direction.None;

            [Group("MasterHoriz/Config/Horiz1")]
            [LabelWidth(80)]
            public Direction aimDirection = Direction.None;
            [Group("MasterHoriz/Config/Horiz2")]
            [LabelWidth(70)]
            public Height aimHeight = Height.None;
            [Group("MasterHoriz/Config/Horiz2")]
            [LabelWidth(80)]
            public float aimEndTime;

            [Group("MasterHoriz/RNG/Horiz1")]
            [LabelWidth(50)]
#if UNITY_EDITOR
            [OnValueChanged(nameof(EditorCalculateWeight))]
#endif
            public float weight = 1;
            [Group("MasterHoriz/RNG/Horiz1")]
            [LabelWidth(50)]
            public float chance = 1;
            [Group("MasterHoriz/RNG/Horiz2")]
            [HideLabel]
            [ReadOnly]
            [ShowInInspector]
            //[ProgressBar(0, 100)]
            [NonSerialized]
            public float lastCalculatedProbabilityRNG;
            [Group("MasterHoriz/RNG/Horiz2")]
            [HideLabel]
            [ReadOnly]
            [ShowInInspector]
            //[ProgressBar(0, 100, 0.74f, 0.47f, 0.15f)]
            [NonSerialized]
            public float probabilityDelta;

            [NonSerialized]
            public AnimationData container;
            [NonSerialized]
            public bool lastPicked;
            [NonSerialized]
            public float lastCalculatedWeightFrom, lastCalculatedWeightTo;

            public enum Direction { None, Forward, Backward, Left, Right, Up, Bottom, }

            public enum Height { None, Lower, Mid, Up, }



            [NonSerialized]
            public bool isDuplicate = false;
            [NonSerialized]
            public bool isMissing = false;

            protected Color EditorGetColor() { return isMissing ? Color.yellow : (isDuplicate ? Color.red : (lastPicked ? Color.green : Color.white)); }
#if UNITY_EDITOR
            protected void EditorCalculateWeight()
            {
                List<CatalogData> list = Catalog.GetDataList(Category.Animation);
                for (int i = 0; i < list.Count; i++)
                {
                    AnimationData animationData = (AnimationData)list[i];
                    animationData.CalculateWeight();
                }
            }

            protected void EditorUpdateAddress()
            {
                if (animationClip)
                    address = Catalog.GetAddressFromPrefab(animationClip);
            }
#endif

            public enum Step
            {
                Start,
                Loop,
                End,
            }
        }

        public void CalculateWeight() => CalculateWeightStrict();

        public void CalculateWeightStrict(bool avoidLastPick = false)
        {
            if (animationClips == null || animationClips.Count == 0)
                return;

            float weightMaximum = 0f;
            // Sets the weight ranges of the selected items.
            for (int i = 0; i < animationClips.Count; i++)
            {
                Clip clip = animationClips[i];
                if (clip.lastPicked && avoidLastPick)
                {
                    clip.lastCalculatedWeightFrom = Mathf.Infinity;
                    clip.lastCalculatedWeightTo = Mathf.NegativeInfinity;
                    continue;
                }
                clip.probabilityDelta = 0;
                if (clip.weight < 0)
                {
                    // Prevent usage of negative weight.
                    clip.weight = 0;
                }
                else
                {
                    clip.lastCalculatedWeightFrom = weightMaximum;
                    weightMaximum += clip.weight;
                    clip.lastCalculatedWeightTo = weightMaximum;
                }
            }
            probabilityRNGTotalWeight = weightMaximum;
            // Calculate percentage of item drop select rate.
            for (int i = 0; i < animationClips.Count; i++)
            {
                Clip clip = animationClips[i];
                clip.lastCalculatedProbabilityRNG = 0;
                if (clip.lastPicked)
                {
                    clip.lastPicked = false;
                    if (avoidLastPick)
                        continue;
                }
                clip.lastCalculatedProbabilityRNG = ((clip.weight) / probabilityRNGTotalWeight) * 100;
            }
        }

    }
}