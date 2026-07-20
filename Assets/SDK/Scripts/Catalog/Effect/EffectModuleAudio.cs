using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TriInspector;


namespace ThunderRoad
{
    [DeclareBoxGroup("Audio/General", Title = "General")]
    [DeclareBoxGroup("Audio/Smoothing", Title = "Smoothing")]
    [DeclareBoxGroup("Audio/Volume", Title = "Volume")]
    [DeclareBoxGroup("Audio/AI", Title = "AI")]
    [DeclareBoxGroup("Audio/Dynamic Music", Title = "Dynamic Music")]
    [DeclareBoxGroup("Audio/Random play", Title = "Random play")]
    [DeclareBoxGroup("Audio/Random Time", Title = "Random Time")]
    [DeclareBoxGroup("Audio/Pitch", Title = "Pitch")]
    [DeclareBoxGroup("Audio/Low pass filter", Title = "Low pass filter")]
    [DeclareBoxGroup("Audio/High pass filter", Title = "High pass filter")]
    [DeclareBoxGroup("Audio/Reverb filter", Title = "Reverb filter")]
    [DeclareBoxGroup("Audio/Distance", Title = "Distance")]
    [DeclareBoxGroup("Steam Audio/Direct Sound", Title = "Direct Sound")]
    [DeclareBoxGroup("Steam Audio/Directivity", Title = "Directivity")]
    [DeclareBoxGroup("Steam Audio/Indirect Sound", Title = "Indirect Sound")]
    public class EffectModuleAudio : EffectModule
    {
        [Group("Audio/General")]
        public string audioContainerAddress;
        [NonSerialized]
        public AudioContainer audioContainer;
        [Group("Audio/General")]
        public float fadeInTime = 0;
        [Group("Audio/General")]
        public float loopFadeDelay = 0;
        [Group("Audio/General")]
        public AudioMixerName audioMixer = AudioMixerName.Effect;
        [Group("Audio/General")]
        [Range(0, 1)]
        public float spatialBlend = 1;
        [Group("Audio/General")]
        public bool globalOnPlayerOnly = false;
        [Group("Audio/General")]
        public float playDelay;
        [Group("Audio/General")]
        public float dopplerLevel = 0;
        [Group("Audio/General")]
        public bool useAudioForHaptic;
        [Group("Audio/Smoothing")]
        public int intensitySmoothingSampleCount = 0;
        [Group("Audio/Smoothing")]
        public int speedSmoothingSampleCount = 0;

        [Group("Audio/Volume")]
        public float cullMinVolume = 0.1f;
        [Group("Audio/Volume")]
        public float volumeDb = 0;
        [Group("Audio/Volume")]
        public bool useVolumeIntensity;
        [Group("Audio/Volume")]
        [ShowIf("useVolumeIntensity")]
        public AnimationCurve volumeIntensityCurve;
        [Group("Audio/Volume")]
        public bool useVolumeSpeed;
        [Group("Audio/Volume")]
        [ShowIf("useVolumeSpeed")]
        public AnimationCurve volumeSpeedCurve;
        [Group("Audio/Volume")]
        [ShowIf("useVolumeIntensity")]
        [ShowIf("useVolumeSpeed")]
        public BlendMode volumeBlendMode = BlendMode.Min;
        [Group("Audio/Volume")]
        public float reverbZoneMix = 1.0f;

        [Group("Audio/AI")]
        public bool isNoiseForAI;

        [Group("Audio/Dynamic Music")]
        public bool onDynamicMusic;
        [Group("Audio/Dynamic Music")]
        [ShowIf("onDynamicMusic")]
        public Music.MusicTransition.TransitionType dynamicMusicTiming;

        [Group("Audio/Random play")]
        public bool randomPlay;
        [Group("Audio/Random play")]
        [ShowIf(nameof(randomPlay))]
        public float randomMinTime = 2;
        [Group("Audio/Random play")]
        [ShowIf(nameof(randomPlay))]
        public float randomMaxTime = 5;

        [Group("Audio/Random Time")]
        public bool randomTime;

        [Group("Audio/Pitch")]
        public bool linkEffectPitch = true;
        [Group("Audio/Pitch")]
        public bool randomPitch;
        [Group("Audio/Pitch")]
        public EffectLink pitchEffectLink = EffectLink.Intensity;
        [Group("Audio/Pitch")]
        public AnimationCurve pitchCurve;

        [Group("Audio/Low pass filter")]
        public bool useLowPassFilter;
        [Group("Audio/Low pass filter")]
        [ShowIf("useLowPassFilter")]
        public EffectLink lowPassEffectLink = EffectLink.Intensity;
        [Group("Audio/Low pass filter")]
        [ShowIf("useLowPassFilter")]
        public AnimationCurve lowPassCutoffFrequencyCurve;
        [Group("Audio/Low pass filter")]
        [ShowIf("useLowPassFilter")]
        public AnimationCurve lowPassResonanceQCurve;

        [Group("Audio/High pass filter")]
        public bool useHighPassFilter;
        [Group("Audio/High pass filter")]
        [ShowIf("useHighPassFilter")]
        public EffectLink highPassEffectLink = EffectLink.Intensity;
        [Group("Audio/High pass filter")]
        [ShowIf("useHighPassFilter")]
        public AnimationCurve highPassCutoffFrequencyCurve;
        [Group("Audio/High pass filter")]
        [ShowIf("useHighPassFilter")]
        public AnimationCurve highPassResonanceQCurve;

        [Group("Audio/Reverb filter")]
        public bool useReverbFilter;
        [Group("Audio/Reverb filter")]
        [ShowIf("useReverbFilter")]
        public EffectLink reverbEffectLink = EffectLink.Intensity;
        [Group("Audio/Reverb filter")]
        [ShowIf("useReverbFilter")]
        public AnimationCurve reverbDryLevelCurve;

        [Group("Audio/Distance")]
        public float minDistance = 1;
        [Group("Audio/Distance")]
        public float maxDistance = 500;



    }

}
