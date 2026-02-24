using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using System.Reflection;
using TriInspector;
using UnityEditor;
#endif

namespace ThunderRoad
{
    [HelpURL("https://kospy.github.io/BasSDK/Components/ThunderRoad/Items/AudioContainer.html")]
    [CreateAssetMenu(menuName = "ThunderRoad/Audio/Audio container")]
    public class AudioContainer : ScriptableObject
    {
#if UNITY_EDITOR
        public const float ShortAudioClipDuration = 1.0f; // footsteps, ui sounds, etc.
        public const float LongAudioClipDuration = 10.0f; // ambient sounds, music, etc.
        public const string Diegetic = "Audio.Diegetic";
        public const string NonDiegetic = "Audio.NonDiegetic";
        public const string Short = "Audio.Short";
        public const string Medium = "Audio.Medium";
        public const string Long = "Audio.Long";
        
        [Tooltip("If true, an amplitude curve will be generated for each audio clip in this container for use with haptics or lip syncing")]
        public bool GenerateAmplitudeCurve = false;
        [Range(1, 120)]
        public int SamplesPerSecond = 10;
        [Header("Classification")]
        [Tooltip("If true, this audio container is considered diegetic (in-world sound source). If false, it's non-diegetic (UI or background sound).")]
        public bool isDiagetic = false;
#endif
        [Header("Audio Clips")]
        public List<AudioClip> sounds;
        public bool useShuffle = false;
        public List<AnimationCurve> amplitudeCurves;
        //this list contains all sounds except the last played
        [NonSerialized]
        protected int[] filteredSounds;
        [NonSerialized]
        protected bool filteredSoundsInitialized;
        [NonSerialized]
        protected ShuffleOrder _shuffleOrder = null;
        [NonSerialized]
        protected int lastPlayedClipHash = -1;
        [NonSerialized]
        protected int filteredSoundsCount = 0;

        [HideInInspector, NonSerialized]
        public Dictionary<int, AudioClip> hashesToClips;
        [HideInInspector, NonSerialized]
        public Dictionary<AudioClip, int> clipToHashes;
        [HideInInspector, NonSerialized]
        public int[] hashes;

        [NonSerialized]
        protected Dictionary<AudioClip, PcmData> pcmDataDic;
        [NonSerialized]
        protected bool pcmDataCached;

        [NonSerialized]
        public int maxPoolCountPerClip = 1;


        private void OnValidate()
        {
            //check if any audioclips are null and throw an exception
            if (sounds.Exists(s => s == null))
            {
                var exception = new Exception($"An AudioClip in the audioContainer: {this.name} is null!");
                Debug.LogException(exception, this);
                throw exception;
            }
#if UNITY_EDITOR
            if (GenerateAmplitudeCurve)
            {
                GenerateAmplitudeCurves();
            }
            else
            {
                amplitudeCurves.Clear();
            }
#endif
        }

#if UNITY_EDITOR

        [Button]
        public void GenerateAmplitudeCurves()
        {
            GenerateAmplitudeCurve = true;
            amplitudeCurves ??= new List<AnimationCurve>(sounds.Count);
            amplitudeCurves.Clear();
            foreach (var audioClip in sounds)
            {
                AnimationCurve curve = GenerateCurveFromAudio(audioClip, SamplesPerSecond);
                amplitudeCurves.Add(curve);
            }
        }
        public static AnimationCurve GenerateCurveFromAudio(AudioClip audioClip, int samplesPerSecond)
        {
            if (audioClip == null)
            {
                Debug.LogError("No audio clip selected!");
                return null;
            }

            // Get the asset path and importer
            string assetPath = AssetDatabase.GetAssetPath(audioClip);
            AudioImporter audioImporter = AssetImporter.GetAtPath(assetPath) as AudioImporter;
            
            if (audioImporter == null)
            {
                Debug.LogError("Could not get AudioImporter for the selected clip!");
                return null;
            }

            // Get min/max data using reflection (AudioUtil is internal)
            System.Type audioUtilType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.AudioUtil");
            if (audioUtilType == null)
            {
                Debug.LogError("Could not find AudioUtil type via reflection!");
                return null;
            }
            
            MethodInfo getMinMaxDataMethod = audioUtilType.GetMethod("GetMinMaxData", BindingFlags.Static | BindingFlags.Public);
            if (getMinMaxDataMethod == null)
            {
                Debug.LogError("Could not find AudioUtil.GetMinMaxData method via reflection!");
                return null;
            }
            
            float[] minMaxData = getMinMaxDataMethod.Invoke(null, new object[] { audioImporter }) as float[];
            
            if (minMaxData == null || minMaxData.Length == 0)
            {
                Debug.LogError("Failed to get audio data from AudioUtil.GetMinMaxData()");
                return null;
            }
            
            int numChannels = audioClip.channels;
            int numSamples = minMaxData.Length / (2 * numChannels);
            
            // Calculate curve sample count based on audio length and desired samples per second
            int curveSampleCount = Mathf.CeilToInt(audioClip.length * samplesPerSecond);
            curveSampleCount = Mathf.Max(10, curveSampleCount);
            
            // Extract amplitude data from the first channel
            float[] amplitudes = new float[curveSampleCount];
            
            for (int i = 0; i < curveSampleCount; i++)
            {
                // Map curve sample to minMax sample
                float t = (float)i / (curveSampleCount - 1);
                int sampleIndex = Mathf.Clamp(Mathf.FloorToInt(t * (numSamples - 1)), 0, numSamples - 1);
                
                // Get min/max for first channel (channel 0)
                int dataIndex = (sampleIndex * numChannels) * 2; // Each sample has 2 values (max, min) per channel
                float maxValue = minMaxData[dataIndex];
                float minValue = minMaxData[dataIndex + 1];
                
                // Calculate amplitude as the range between min and max
                amplitudes[i] = Mathf.Max(Mathf.Abs(maxValue), Mathf.Abs(minValue));
            }
            
            // Find min and max amplitude for normalization
            float minAmplitude = float.MaxValue;
            float maxAmplitude = float.MinValue;

            foreach (float amp in amplitudes)
            {
                if (amp < minAmplitude) minAmplitude = amp;
                if (amp > maxAmplitude) maxAmplitude = amp;
            }

            // Create animation curve
            AnimationCurve curve = new AnimationCurve();

            // Add normalized values to curve
            for (int i = 0; i < amplitudes.Length; i++)
            {
                float t = (float)i / (curveSampleCount - 1);
                
                // Normalize to 0-1
                float normalizedAmplitude = 0f;
                if (maxAmplitude > minAmplitude)
                {
                    normalizedAmplitude = (amplitudes[i] - minAmplitude) / (maxAmplitude - minAmplitude);
                }

                curve.AddKey(t, normalizedAmplitude);
            }

            // Smooth the curve
            for (int i = 0; i < curve.length; i++)
            {
                curve.SmoothTangents(i, 1f);
            }
            
            return curve;
        }
        
        
        [Button]
        public void ApplyLabelsAndReimport()
        {
            //force reimport of all audio clips to apply labels
            try
            {
                AssetDatabase.StartAssetEditing();
                foreach (var audioClip in sounds)
                {
                    SetLabels(audioClip, isDiagetic);
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(audioClip));
                }
            }  
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

        }
        public static string[] SetLabels(Object clip, bool isDiegetic)
        {
            bool isShort = false;
            bool isLong = false;
            switch (clip)
            {
                case AudioImporter audioImporter: {
                    var path = AssetDatabase.GetAssetPath(audioImporter);
                    var audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                    if (audioClip != null)
                    {
                        isShort = audioClip.length <= ShortAudioClipDuration;
                        isLong = audioClip.length >= LongAudioClipDuration;
                    }
                    break;
                }
                case AudioClip audioClip:
                    isShort = audioClip.length <= ShortAudioClipDuration;
                    isLong = audioClip.length >= LongAudioClipDuration;
                    break;
            }
            var labels = AssetDatabase.GetLabels(clip);
            var newLabels = labels
                .Where(l => l != Diegetic && l != NonDiegetic && l != Short && l != Medium && l != Long)
                .ToList();
            if (isShort)
            {
                newLabels.Add(Short);
            }
            else if (isLong)
            {
                newLabels.Add(Long);
            }
            else
            {
                newLabels.Add(Medium);
            }
            newLabels.Add(isDiegetic ? Diegetic : NonDiegetic);
            //only apply if labels have changed
            if (newLabels.Count != labels.Length || !newLabels.All(l => labels.Contains(l)))
            {
                labels = newLabels.ToArray();
                AssetDatabase.SetLabels(clip, labels);
            }
            return labels;
        }
#endif


        public AudioClip PickAudioClip()
        {
return default;
        }

    }
}
