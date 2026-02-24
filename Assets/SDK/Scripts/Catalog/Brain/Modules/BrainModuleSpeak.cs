using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;




using TriInspector;

namespace ThunderRoad
{
    [System.Serializable]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class BrainModuleSpeak : BrainData.Module
    {
        public bool audioEnabled = true;
        public float audioVolume = 1f;
        public Vector2 audioPitchRange = new Vector2(1.0f, 1.0f);
        public float audioFallVelocityThreshold = 5;
        public float audioAttackChance = 0.7f;
        public float audioHitChance = 1;
        public float audioDeathChance = 1;
        public bool moveMouth = true;
        public float audioLipSyncMaxValue = 2f;
        public float lipSyncSpeed = 5f;

        public bool speakOnAttackMelee = true;
        public bool speakOnAttackCast = false;
        public bool speakOnAttackBow = false;
        public bool speakOnChoke = false;
        // 0 means no silence, 1 means replace death and hit with muffle, 2 means no sound
        public int neckStabSilencelevel = 1;
        // 0 means no silence, 1 means no death sound, 2 means no death and no hit
        public int headStabSilenceLevel = 1;
        // 0 means no silence, 1 means no death sound, 2 means no death and no hit
        public int backStabSilenceLevel = 1;

        [Header("Random speak")]
        public float randomSpeakIdleChance = 0;
        public float randomSpeakAlertChance = 0;
        public float randomSpeakInvestigateChance = 0;
        public float randomSpeakCombatChance = 0;
        public float randomSpeakDelay = 5f;
        public float muffleSpeakDelay = 1.5f;

    }
}
