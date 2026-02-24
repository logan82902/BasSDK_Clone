using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TriInspector;


namespace ThunderRoad
{
    [DeclareTabGroup("Tab")]
    [DeclareBoxGroup("Tab/Audio/General", Title = "General")]
    [DeclareBoxGroup("Tab/Audio/Smoothing", Title = "Smoothing")]
    [DeclareBoxGroup("Tab/Audio/Volume", Title = "Volume")]
    [DeclareBoxGroup("Tab/Audio/AI", Title = "AI")]
    [DeclareBoxGroup("Tab/Audio/Dynamic Music", Title = "Dynamic Music")]
    [DeclareBoxGroup("Tab/Audio/Random play", Title = "Random play")]
    [DeclareBoxGroup("Tab/Audio/Random Time", Title = "Random Time")]
    [DeclareBoxGroup("Tab/Audio/Pitch", Title = "Pitch")]
    [DeclareBoxGroup("Tab/Audio/Low pass filter", Title = "Low pass filter")]
    [DeclareBoxGroup("Tab/Audio/High pass filter", Title = "High pass filter")]
    [DeclareBoxGroup("Tab/Audio/Reverb filter", Title = "Reverb filter")]
    [DeclareBoxGroup("Tab/Audio/Distance", Title = "Distance")]
    [DeclareBoxGroup("Tab/Steam Audio/Direct Sound", Title = "Direct Sound")]
    [DeclareBoxGroup("Tab/Steam Audio/Directivity", Title = "Directivity")]
    [DeclareBoxGroup("Tab/Steam Audio/Indirect Sound", Title = "Indirect Sound")]
    public class EffectModuleAudio : EffectModule
    {
        [Group("Tab/Audio/General")]
        public string audioContainerAddress;
        [NonSerialized]
        public AudioContainer audioContainer;
        [Group("Tab/Audio/General")]
        public float fadeInTime = 0;
        [Group("Tab/Audio/General")]
        public float loopFadeDelay = 0;
        [Group("Tab/Audio/General")]
        public AudioMixerName audioMixer = AudioMixerName.Effect;
        [Group("Tab/Audio/General")]
        [Range(0, 1)]
        public float spatialBlend = 1;
        [Group("Tab/Audio/General")]
        public bool globalOnPlayerOnly = false;
        [Group("Tab/Audio/General")]
        public float playDelay;
        [Group("Tab/Audio/General")]
        public float dopplerLevel = 0;
        [Group("Tab/Audio/General")]
        public bool useAudioForHaptic;
        [Group("Tab/Audio/Smoothing")]
        public int intensitySmoothingSampleCount = 0;
        [Group("Tab/Audio/Smoothing")]
        public int speedSmoothingSampleCount = 0;

        [Group("Tab/Audio/Volume")]
        public float cullMinVolume = 0.1f;
        [Group("Tab/Audio/Volume")]
        public float volumeDb = 0;
        [Group("Tab/Audio/Volume")]
        public bool useVolumeIntensity;
        [Group("Tab/Audio/Volume")]
        [ShowIf("useVolumeIntensity")]
        public AnimationCurve volumeIntensityCurve;
        [Group("Tab/Audio/Volume")]
        public bool useVolumeSpeed;
        [Group("Tab/Audio/Volume")]
        [ShowIf("useVolumeSpeed")]
        public AnimationCurve volumeSpeedCurve;
        [Group("Tab/Audio/Volume")]
        [ShowIf("useVolumeIntensity")]
        [ShowIf("useVolumeSpeed")]
        public BlendMode volumeBlendMode = BlendMode.Min;
        [Group("Tab/Audio/Volume")]
        public float reverbZoneMix = 1.0f;

        [Group("Tab/Audio/AI")]
        public bool isNoiseForAI;

        [Group("Tab/Audio/Dynamic Music")]
        public bool onDynamicMusic;
        [Group("Tab/Audio/Dynamic Music")]
        [ShowIf("onDynamicMusic")]
        public Music.MusicTransition.TransitionType dynamicMusicTiming;

        [Group("Tab/Audio/Random play")]
        public bool randomPlay;
        [Group("Tab/Audio/Random play")]
        [ShowIf(nameof(randomPlay))]
        public float randomMinTime = 2;
        [Group("Tab/Audio/Random play")]
        [ShowIf(nameof(randomPlay))]
        public float randomMaxTime = 5;

        [Group("Tab/Audio/Random Time")]
        public bool randomTime;

        [Group("Tab/Audio/Pitch")]
        public bool linkEffectPitch = true;
        [Group("Tab/Audio/Pitch")]
        public bool randomPitch;
        [Group("Tab/Audio/Pitch")]
        public EffectLink pitchEffectLink = EffectLink.Intensity;
        [Group("Tab/Audio/Pitch")]
        public AnimationCurve pitchCurve;

        [Group("Tab/Audio/Low pass filter")]
        public bool useLowPassFilter;
        [Group("Tab/Audio/Low pass filter")]
        [ShowIf("useLowPassFilter")]
        public EffectLink lowPassEffectLink = EffectLink.Intensity;
        [Group("Tab/Audio/Low pass filter")]
        [ShowIf("useLowPassFilter")]
        public AnimationCurve lowPassCutoffFrequencyCurve;
        [Group("Tab/Audio/Low pass filter")]
        [ShowIf("useLowPassFilter")]
        public AnimationCurve lowPassResonanceQCurve;

        [Group("Tab/Audio/High pass filter")]
        public bool useHighPassFilter;
        [Group("Tab/Audio/High pass filter")]
        [ShowIf("useHighPassFilter")]
        public EffectLink highPassEffectLink = EffectLink.Intensity;
        [Group("Tab/Audio/High pass filter")]
        [ShowIf("useHighPassFilter")]
        public AnimationCurve highPassCutoffFrequencyCurve;
        [Group("Tab/Audio/High pass filter")]
        [ShowIf("useHighPassFilter")]
        public AnimationCurve highPassResonanceQCurve;

        [Group("Tab/Audio/Reverb filter")]
        public bool useReverbFilter;
        [Group("Tab/Audio/Reverb filter")]
        [ShowIf("useReverbFilter")]
        public EffectLink reverbEffectLink = EffectLink.Intensity;
        [Group("Tab/Audio/Reverb filter")]
        [ShowIf("useReverbFilter")]
        public AnimationCurve reverbDryLevelCurve;

        [Group("Tab/Audio/Distance")]
        public float minDistance = 1;
        [Group("Tab/Audio/Distance")]
        public float maxDistance = 500;



    }

}
