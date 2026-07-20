using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using TriInspector;

#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
#endif

namespace ThunderRoad
{
    [HelpURL("https://kospy.github.io/BasSDK/Components/ThunderRoad/Items/AudioContainer.html")]
    [CreateAssetMenu(menuName = "ThunderRoad/Audio/Audio container")]
    [DeclareHorizontalGroup("Horiz")]
    [DeclareVerticalGroup("Horiz/Sounds")]
    [DeclareHorizontalGroup("Horiz/Sounds/Settings")]
    [DeclareVerticalGroup("Horiz/Curves")]
    [DeclareHorizontalGroup("Horiz/Curves/Controls")]
    [DeclareFoldoutGroup("AmpCurveSettings", Title = "Amplitude Curve Generation Settings")]
    [DeclareHorizontalGroup("AmpCurveSettings/Horiz1")]
    [DeclareHorizontalGroup("AmpCurveSettings/Horiz2")]
    [DeclareHorizontalGroup("AmpCurveSettings/Horiz3")]
    public class AudioContainer : ScriptableObject
    {
#if UNITY_EDITOR
        [AutoLabelWidth(AddedWidth = 6f)]
        [Group("Horiz/Sounds/Settings")]
        [Tooltip("If true, this audio container is considered diegetic (in-world sound source). If false, it's non-diegetic (UI or background sound).")]
        public bool isDiagetic = false;
#endif
        [AutoLabelWidth(AddedWidth = 6f)]
        [Group("Horiz/Sounds/Settings")]
        public bool useShuffle = false;
        [Group("Horiz/Sounds")]
        [ListDrawerSettings(AlwaysExpanded = true)]
        public List<AudioClip> sounds;
#if UNITY_EDITOR
        [AutoLabelWidth]
        [Group("Horiz/Curves/Controls")]
        [Tooltip("If true, an amplitude curve will be generated for each audio clip in this container for use with haptics or lip syncing")]
        public bool GenerateAmplitudeCurve = false;
#endif
        [Group("Horiz/Curves")]
        [ReadOnly]
        [ListDrawerSettings(AlwaysExpanded = true, Draggable = false, HideAddButton = true, HideRemoveButton = true)]
        public List<AnimationCurve> amplitudeCurves;
#if UNITY_EDITOR
        #region IMPORT PROPERTIES
        
        public const float ShortAudioClipDuration = 1.0f; // footsteps, ui sounds, etc.
        public const float LongAudioClipDuration = 10.0f; // ambient sounds, music, etc.
        public const string Diegetic = "Audio.Diegetic";
        public const string NonDiegetic = "Audio.NonDiegetic";
        public const string Short = "Audio.Short";
        public const string Medium = "Audio.Medium";
        public const string Long = "Audio.Long";

        #endregion
        
        #region AMPLITUDE CURVES SETTINGS
        [ShowIf(nameof(GenerateAmplitudeCurve))]
        [Group("AmpCurveSettings")]
        [AutoLabelWidth(AddedWidth = 6f)]
        [Tooltip("If greater than zero, amplitudes below this amount will be flattened to zero.")]
        [Range(0, 1)]
        public float amplitudeGate = 0;

        [ShowIf(nameof(GenerateAmplitudeCurve))]
        [Group("AmpCurveSettings/Horiz1")]
        [AutoLabelWidth(AddedWidth = 6f)]
        [Range(0, 1)]
        public float fadeInDuration = 0;
        [ShowIf(nameof(GenerateAmplitudeCurve))]
        [Group("AmpCurveSettings/Horiz1")]
        [AutoLabelWidth(AddedWidth = 6f)]
        [Range(0, 1)]
        public float fadeOutDuration = 0;

        [ShowIf(nameof(GenerateAmplitudeCurve))]
        [Group("AmpCurveSettings/Horiz2")]
        [AutoLabelWidth(AddedWidth = 6f)]
        [Range(0, 10)]
        public float curvePower = 1;

        [ShowIf(nameof(GenerateAmplitudeCurve))]
        [Group("AmpCurveSettings/Horiz3")]
        [AutoLabelWidth(AddedWidth = 6f)]
        [Range(0, 10)]
        public float amplify = 1;

        [ShowIf(nameof(GenerateAmplitudeCurve))]
        [Group("AmpCurveSettings/Horiz3")]
        [AutoLabelWidth(AddedWidth = 6f)]
        [Range(0, 1)]
        public float boost = 0;

        [ShowIf(nameof(GenerateAmplitudeCurve))]
        [Group("AmpCurveSettings/Horiz2")]
        public bool smoothCurve = true;

        [ShowIf(nameof(GenerateAmplitudeCurve))]
        [Group("AmpCurveSettings")]
        [AutoLabelWidth(AddedWidth = 6f)]
        [Range(0, 1)]
        public float stopTime = 1;
        [ShowIf(nameof(GenerateAmplitudeCurve))]
        [Group("AmpCurveSettings")]
        [AutoLabelWidth(AddedWidth = 6f)]
        [Range(1, 120)]
        public int SamplesPerSecond = 10;
        #endregion
#endif
        [NonSerialized]
        protected ShuffleOrder _shuffleOrder = null;
        [NonSerialized]
        protected int lastPlayedClipHash = -1;

        [HideInInspector, NonSerialized]
        public Dictionary<int, AudioClip> hashesToClips;
        [HideInInspector, NonSerialized]
        public Dictionary<AudioClip, int> clipToHashes;
        
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
        public struct CurveParams
        {
            public float samplesPerSecond, amplitudeGate, stopTime, fadeInDuration, fadeOutDuration, power, amplify, boost;
            public bool smoothCurve;
        }

        [Group("Horiz/Curves/Controls")]
        [Button]
        public void GenerateAmplitudeCurves()
        {
            GenerateAmplitudeCurve = true;
            amplitudeCurves ??= new List<AnimationCurve>(sounds.Count);
            amplitudeCurves.Clear();
            var settings = new CurveParams
            {
                amplitudeGate = amplitudeGate,
                fadeInDuration = fadeInDuration,
                fadeOutDuration = fadeOutDuration,
                samplesPerSecond = SamplesPerSecond,
                smoothCurve = smoothCurve,
                power = curvePower,
                amplify = amplify,
                boost = boost,
                stopTime = stopTime
            };
            foreach (var audioClip in sounds)
            {
                AnimationCurve curve = GenerateCurveFromAudio(audioClip, settings);
                amplitudeCurves.Add(curve);
            }
        }
        
        public static AnimationCurve GenerateCurveFromAudio(AudioClip audioClip, CurveParams settings)
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
            int curveSampleCount = Mathf.CeilToInt(audioClip.length * settings.samplesPerSecond);
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
                if (t >= settings.stopTime)
                {
                    curve.AddKey(t, 0);
                    continue;
                }
                
                // Normalize to 0-1
                float normalizedAmplitude = 0f;
                if (maxAmplitude > minAmplitude)
                {
                    normalizedAmplitude = (amplitudes[i] - minAmplitude) / (maxAmplitude - minAmplitude);
                }

                if (normalizedAmplitude <= settings.amplitudeGate) normalizedAmplitude = 0;
                normalizedAmplitude = Mathf.Clamp01(Mathf.Pow(normalizedAmplitude * 2, settings.power) / 2);

                float fadeMult = 1;
                if (t <= settings.fadeInDuration)
                    fadeMult *= SCurve(t / settings.fadeInDuration);
                if (t > settings.stopTime - settings.fadeOutDuration)
                    fadeMult *= 1 - SCurve(t - (settings.stopTime - settings.fadeOutDuration));

                normalizedAmplitude *= settings.amplify;
                normalizedAmplitude += settings.boost;
                curve.AddKey(t, (normalizedAmplitude * fadeMult).Clamp01());
            }

            // Smooth the curve
            if (settings.smoothCurve)
            {
                for (int i = 0; i < curve.length; i++)
                {
                    curve.SmoothTangents(i, 1f);
                }
            }

            return curve;
        }

        public static float SCurve(float t)
        {
            if (t <= 0.5f) return 2 * t * t;
            return 1 - 2 * (1 - t) * (1 - t);
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
