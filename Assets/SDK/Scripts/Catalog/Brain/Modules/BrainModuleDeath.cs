using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

using TriInspector;

namespace ThunderRoad
{
    [System.Serializable]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class BrainModuleDeath : BrainData.Module
    {
        public AnimationCurve curve = AnimationCurve.Linear(0, 1, 1, 0);
        public float maxMovingVelocity = 3f;
        public float unpinDistance = 1f;
        public float unpinMaxAngle = 70f;
        public Vector2 dropObjectsDelay = new Vector2(0, 0.5f);
        public bool useDamagerDyingAnimMaxVelocity = false;
        public float grabThrowMinVelocity = 2.0f;

        [Header("Animation")]
        [Dropdown(nameof(GetAllAnimationID))]
        public string defaultAnimationId;
        [NonSerialized]
        public AnimationData defaultAnimationData;

        [Dropdown(nameof(GetAllAnimationID))]
        public string stabAnimationId;
        [NonSerialized]
        public AnimationData stabAnimationData;

        [Dropdown(nameof(GetAllAnimationID))]
        public string fireAnimationId = "HumanFireDeaths";
        [NonSerialized]
        public AnimationData fireAnimationData;

        [Dropdown(nameof(GetAllAnimationID))]
        public string lightningAnimationId = "HumanLightningDeaths";
        [NonSerialized]
        public AnimationData lightningAnimationData;

        public enum DeathType
        {
            Default,
            Stab,
            Fire,
            Lightning
        }

        public TriDropdownList<string> GetAllAnimationID() => Catalog.GetDropdownAllID(Category.Animation);

    }
}