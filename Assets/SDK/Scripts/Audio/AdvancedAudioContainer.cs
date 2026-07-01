using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

using TriInspector;

namespace ThunderRoad
{
    [HelpURL("https://kospy.github.io/BasSDK/Components/ThunderRoad/AdvancedAudioContainer")]
    [CreateAssetMenu(menuName = "ThunderRoad/Audio/Advanced audio container")]
    [DeclareHorizontalGroup("Bools")]
    [DeclareVerticalGroup("Clips")]
    [DeclareHorizontalGroup("Clips/Horiz", Sizes = new float[] { -45f })]
    [DeclareBoxGroup("Clips/Horiz/Box", HideTitle = true)]
    [DeclareHorizontalGroup("CurveGen")]
    [DeclareFoldoutGroup("AmpCurveSettings", Title = "Amplitude Curve Generation Settings")]
    [DeclareHorizontalGroup("AmpCurveSettings/Horiz1")]
    [DeclareHorizontalGroup("AmpCurveSettings/Horiz2")]
    [DeclareHorizontalGroup("AmpCurveSettings/Horiz3")]
    [DeclareHorizontalGroup("IDs")]
    public class AdvancedAudioContainer : ScriptableObject
    {
        [Serializable]
        [DeclareVerticalGroup("Audio")]
        [DeclareHorizontalGroup("Audio/Weight")]
        [DeclareVerticalGroup("Animation")]
        [DeclareVerticalGroup("Subtitle")]
        public class AudioClipData
        {
            [HideInInspector]
            public bool UseAmplitudeCurves = false;

            [ReadOnly]
            [Group("Audio")]
            [HideLabel]
            public AudioClip audio;
            [Group("Animation")]
            [HideLabel]
            [HideIf(nameof(UseAmplitudeCurves))]
            public AnimationClip animation;
            [Group("Animation")]
            [HideLabel]
            [ShowIf(nameof(UseAmplitudeCurves))]
            public AnimationCurve amplitudeCurve;
            [Group("Animation")]
            [LabelText("Expression")]
            [AutoLabelWidth]
            public FaceAnimator.Expression dialogExpression;
            [Group("Subtitle")]
            [LabelText("ID")]
            [AutoLabelWidth]
            public string transcriptionTextId;
# if UNITY_EDITOR
            [OnValueChanged(nameof(WeightUpdated))]
#endif
            [Group("Audio/Weight")]
            [LabelText("Weight")]
            [AutoLabelWidth]
            public int weight = 1;
# if UNITY_EDITOR
            [ShowInInspector]
            [Group("Audio/Weight")]
            [HideLabel]
            public string percent => $"{Mathf.RoundToInt(1000f * ((float)weight / (Mathf.Approximately(totalWeight, 0f) ? weight : totalWeight))) / 10f}%";
            [ShowInInspector]
            [Group("Subtitle")]
            [LabelText("Text")]
            [AutoLabelWidth]
            [ReadOnly]
            public string transcriptionText
            {
                get
                {
                    if (_transcriptionText.IsNullOrEmptyOrWhitespace())
                    {
                        if (!Catalog.IsJsonLoaded()) Catalog.EditorLoadAllJson(true, true, true);
                        _transcriptionText = Catalog.GetData<TextData>("English").textGroups.FirstOrDefault(group => group.id == "Dialog")?.texts.FirstOrDefault(text => text.id == transcriptionTextId)?.text;
                    }
                    return _transcriptionText;
                }
            }
            private string _transcriptionText;
            [NonSerialized]
            public int totalWeight;
            public Action weightUpdate;

            private void WeightUpdated()
            {
                weightUpdate?.Invoke();
            }
#endif

            public AudioClipData(AudioClip aud) => audio = aud;
        }
#if UNITY_EDITOR
        [Group("Bools")]
        [AutoLabelWidth(AddedWidth = 6f)]
        [Tooltip("If true, this audio container is considered diegetic (in-world sound source). If false, it's non-diegetic (UI or background sound).")]
        public bool isDiagetic = false;
#endif

        [Group("Clips")]
        [TableList(AlwaysExpanded = true, HideAddButton = true, IgnoreButtonSpacing = true)]
        public List<AudioClipData> clips = new();
        [Group("Bools")]
        [AutoLabelWidth(AddedWidth = 6f)]
        public bool useShuffle;
        [Group("Bools")]
        [AutoLabelWidth(AddedWidth = 6f)]
        public bool showSubtitles = true;
#if UNITY_EDITOR
        [InlineButton(nameof(GetExpressionOptions), "Set all expressions to...", DropdownButton = true, ButtonWidth = 220f)]
        [NonSerialized]
        [ShowInInspector]
        [Group("Clips/Horiz/Box")]
        [LabelText("Add new clip: ")]
        [AutoLabelWidth]
        [OnValueChanged(nameof(RunValidation))]
        public AudioClip newClip;

        [AutoLabelWidth(AddedWidth = 6f)]
        [Group("CurveGen")]
        [Tooltip("If true, an amplitude curve will be generated for each audio clip in this container for use with haptics or lip syncing")]
        public bool UseAmplitudeCurves = false;

        private TriDropdownList<Action> GetExpressionOptions()
        {
            TriDropdownList<Action> result = new TriDropdownList<Action>();
            foreach (FaceAnimator.Expression expression in Enum.GetValues(typeof(FaceAnimator.Expression)))
            {
                result.Add($"{expression.ToString()}", () =>
                {
                    foreach (var clip in clips)
                    {
                        clip.dialogExpression = expression;
                    }
                    EditorUtility.SetDirty(this);
                    AssetDatabase.SaveAssetIfDirty(this);
                });
            }
            return result;
        }

        [ShowIf(nameof(UseAmplitudeCurves))]
        [Group("AmpCurveSettings")]
        [AutoLabelWidth(AddedWidth = 6f)]
        [Tooltip("If greater than zero, amplitudes below this amount will be flattened to zero.")]
        [Range(0, 1)]
        public float amplitudeGate = 0;

        [ShowIf(nameof(UseAmplitudeCurves))]
        [Group("AmpCurveSettings/Horiz1")]
        [AutoLabelWidth(AddedWidth = 6f)]
        [Range(0, 1)]
        public float fadeInDuration = 0;
        [ShowIf(nameof(UseAmplitudeCurves))]
        [Group("AmpCurveSettings/Horiz1")]
        [AutoLabelWidth(AddedWidth = 6f)]
        [Range(0, 1)]
        public float fadeOutDuration = 0;

        [ShowIf(nameof(UseAmplitudeCurves))]
        [Group("AmpCurveSettings/Horiz2")]
        [AutoLabelWidth(AddedWidth = 6f)]
        [Range(0, 10)]
        public float curvePower = 1;

        [ShowIf(nameof(UseAmplitudeCurves))]
        [Group("AmpCurveSettings/Horiz3")]
        [AutoLabelWidth(AddedWidth = 6f)]
        [Range(0, 10)]
        public float amplify = 1;

        [ShowIf(nameof(UseAmplitudeCurves))]
        [Group("AmpCurveSettings/Horiz3")]
        [AutoLabelWidth(AddedWidth = 6f)]
        [Range(0, 1)]
        public float boost = 0;

        [ShowIf(nameof(UseAmplitudeCurves))]
        [Group("AmpCurveSettings/Horiz2")]
        public bool smoothCurve = true;

        [ShowIf(nameof(UseAmplitudeCurves))]
        [Group("AmpCurveSettings")]
        [AutoLabelWidth(AddedWidth = 6f)]
        [Range(0, 1)]
        public float stopTime = 1;
        [ShowIf(nameof(UseAmplitudeCurves))]
        [Group("AmpCurveSettings")]
        [AutoLabelWidth(AddedWidth = 6f)]
        [Range(1, 120)]
        public int SamplesPerSecond = 10;

        [Button]
        [Group("CurveGen")]
        public void GenerateAmplitudeCurves()
        {
            UseAmplitudeCurves = true;
            var settings = new AudioContainer.CurveParams
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
            foreach (var audioClip in clips)
            {
                audioClip.amplitudeCurve = AudioContainer.GenerateCurveFromAudio(audioClip.audio, settings);
            }
        }

        [Button]
        public void ApplyLabels()
        {
            foreach (var data in clips)
            {
                AudioContainer.SetLabels(data.audio, isDiagetic);
            }
        }
        
        public void UpdateWeights()
        {
            int totalWeight = 0;
            foreach (AudioClipData clipData in clips)
            {
                clipData.weightUpdate = UpdateWeights;
                totalWeight += clipData.weight;
            }
            foreach (AudioClipData clipData in clips)
            {
                clipData.totalWeight = totalWeight;
            }
        }

        public void RefreshDialogIDs()
        {
            foreach (AudioClipData data in clips)
            {
                if (string.IsNullOrEmpty(data.transcriptionTextId))
                {
                    //set the transcription text id to the audio name
                    string cleanedName = data.audio.name;
                    cleanedName = cleanedName.Replace("_TRIMMED", "");
                    //remove "Audio_" from the start of the name
                    cleanedName = cleanedName.Replace("Audio_", "");
                    data.transcriptionTextId = cleanedName;
                }
            }
        }

        [Button("Find and replace in Transcription IDs")]
        [Group("IDs")]
        public void IDFindAndReplace(string find = "", string replace = "")
        {
            foreach (AudioClipData data in clips)
            {
                if (string.IsNullOrEmpty(data.transcriptionTextId))
                {
                    //set the transcription text id to the audio name
                    string cleanedName = data.audio.name;
                    cleanedName = cleanedName.Replace("_TRIMMED", "");
                    //remove "Audio_" from the start of the name
                    cleanedName = cleanedName.Replace("Audio_", "");
                    data.transcriptionTextId = cleanedName;
                }
                data.transcriptionTextId = data.transcriptionTextId.Replace(find, replace);
            }
        }

#endif

        private void OnValidate()
        {
            clips ??= new List<AudioClipData>();
#if UNITY_EDITOR
            RunValidation();
#endif
        }

#if UNITY_EDITOR
        private bool isValidating = false;

        private void RunValidation()
        {
            if (isValidating) return;
            isValidating = true;
            UpdateWeights();
            if (newClip != null)
            {
                foreach (AudioClipData data in clips)
                {
                    if (data.audio == newClip)
                    {
                        newClip = null;
                        return;
                    }
                }
                clips.Add(new AudioClipData(newClip));
            }
            newClip = null;
            var settings = new AudioContainer.CurveParams
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
            foreach (var clip in clips)
            {
                clip.weightUpdate = UpdateWeights;
                clip.UseAmplitudeCurves = UseAmplitudeCurves;
                if (UseAmplitudeCurves)
                {
                    clip.amplitudeCurve = AudioContainer.GenerateCurveFromAudio(clip.audio, settings);
                }
                else
                {
                    clip.amplitudeCurve = null;
                }
            }
            AssetDatabase.SaveAssetIfDirty(this);
            isValidating = false;
        }
#endif

        public AudioContainer audioContainer
        {
            get
            {
                if (_audioContainer != null)
                {
                    return _audioContainer;
                }
                _audioContainer = CreateInstance<AudioContainer>();
                _audioContainer.sounds = new List<AudioClip>();
                _audioContainer.amplitudeCurves = new List<AnimationCurve>();
                foreach (AudioClipData data in clips)
                {
                    for (int i = 0; i < data.weight; i++)
                    {
                        _audioContainer.sounds.Add(data.audio);
                        if (data.amplitudeCurve != null) _audioContainer.amplitudeCurves.Add(data.amplitudeCurve);
                    }
                }
                _audioContainer.useShuffle = useShuffle;
                return _audioContainer;
            }
        }

        [NonSerialized]
        private AudioContainer _audioContainer;

        public Dictionary<int, AudioClipData> matchedClipData
        {
            get
            {
                if (!_matchedClipData.IsNullOrEmpty())
                {
                    return _matchedClipData;
                }
                Dictionary<AudioClip, AudioClipData> clipData = new Dictionary<AudioClip, AudioClipData>();
                foreach (AudioClipData clip in clips) clipData.Add(clip.audio, clip);
                _matchedClipData = new Dictionary<int, AudioClipData>();
                for (int i = 0; i < audioContainer.sounds.Count; i++)
                {
                    AudioClip clip = audioContainer.sounds[i];
                    //we use the index instead of a hash because they are unique per audio container and the clip order is retained
                    _matchedClipData.Add(i, clipData[clip]);
                }
                return _matchedClipData;
            }
        }

        [NonSerialized]
        private Dictionary<int, AudioClipData> _matchedClipData;

#if UNITY_EDITOR
        [MenuItem("Assets/ThunderRoad/AudioClips + AnimClips > AdvancedAudioContainers")]
        public static void CreateFromClips(MenuCommand menuCommand)
        {
            AdvancedAudioContainer advancedAudioContainer = new();
            advancedAudioContainer.clips = new List<AudioClipData>();
            advancedAudioContainer.name = "NewAdvancedAudioContainer";
            string rootPath = null;
            Dictionary<string, UnityEngine.Object> collectedObjects = new();
            foreach (string guid in Selection.assetGUIDs)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
                if (rootPath == null)
                {
                    rootPath = path.Split('.')[0].Replace(asset.name, "");
                }
                AudioClipData clipData = null;
                if (asset is AnimationClip anim)
                {
                    string audioKey = asset.name.Replace("Animation", "Audio");
                    if (collectedObjects.ContainsKey(audioKey))
                    {
                        clipData = new AudioClipData((AudioClip)collectedObjects[audioKey]) { animation = anim };
                    }
                }
                if (asset is AudioClip audio)
                {
                    string animKey = asset.name.Replace("Audio", "Animation");
                    if (collectedObjects.ContainsKey(animKey))
                    {
                        clipData = new AudioClipData(audio) { animation = (AnimationClip)collectedObjects[animKey] };
                    }
                }
                if (clipData == null)
                {
                    collectedObjects.Add(asset.name, asset);
                }
                else
                {
                    advancedAudioContainer.clips.Add(clipData);
                }
            }
            advancedAudioContainer.clips = advancedAudioContainer.clips.OrderBy(clip => clip.audio.name).ToList();
            advancedAudioContainer.RefreshDialogIDs();
            AssetDatabase.CreateAsset(advancedAudioContainer, $"{rootPath}/{advancedAudioContainer.name}.asset");
        }

        public void MatchFilenamesToTranscriptionIDs()
        {
            foreach (AudioClipData data in clips)
            {
                string audioPath = AssetDatabase.GetAssetPath(data.audio);
                string newAudioPath = $"{audioPath.Split('.')[0].Replace(data.audio.name, "Audio_" + data.transcriptionTextId)}.{audioPath.Split('.')[1]}";
                AssetDatabase.CreateAsset(data.audio, newAudioPath);
                AssetDatabase.DeleteAsset(audioPath);
                string animPath = AssetDatabase.GetAssetPath(data.animation);
                string newAnimPath = $"{animPath.Split('.')[0].Replace(data.animation.name, "Audio_" + data.transcriptionTextId)}.{animPath.Split('.')[1]}";
                Debug.Log($"{newAudioPath}, {newAnimPath}");
                AssetDatabase.CreateAsset(data.animation, newAnimPath);
                AssetDatabase.DeleteAsset(animPath);
            }
        }
#endif
    }
}
