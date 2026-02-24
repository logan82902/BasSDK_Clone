using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine;
using Newtonsoft.Json;

using TriInspector;

namespace ThunderRoad
{
    [Serializable]
#if UNITY_EDITOR
    [EditorLoadPriority(5)]
#endif
    [DeclareBoxGroup("Max Pool Sizes For Each Voice Clip", Title = "Max Pool Sizes For Each Voice Clip")]
    [DeclareBoxGroup("Voice", Title = "Voice")]
    public class VoiceData : CatalogData
    {
        [Group("Max Pool Sizes For Each Voice Clip")]
        public int pooledCount;

        [Group("Max Pool Sizes For Each Voice Clip")]
        public int androidPooledCount;

        [Group("Voice")]
        [HideReferencePicker]
        [TableList(AlwaysExpanded = true)]
        public List<Dialog> dialogs = new();

        [Serializable]
        public class Dialog
        {
            [JsonMergeKey]
            public string id;
            [NonSerialized]
            public int hashId;
            [Tooltip("Defaults to voice. Can be set to another channel if necessary.")]
            public AudioMixerName mixerChannel = AudioMixerName.Voice;

            public string audioGroupAddress;

            [JsonIgnore] public AudioContainer audioContainer => advancedAudioContainer != null ? advancedAudioContainer.audioContainer : basicAudioContainer;
            [NonSerialized]
            public AudioContainer basicAudioContainer;
            [NonSerialized]
            public AdvancedAudioContainer advancedAudioContainer;
            [NonSerialized]
            public VoiceData rootData;

            public virtual void ReleaseAddressableAssets()
            {
                if (audioContainer)
                {
                    Catalog.ReleaseAsset(audioContainer);
                }
                basicAudioContainer = null;
                advancedAudioContainer = null;
            }
            public virtual IEnumerator LoadAddressableAssetsCoroutine(VoiceData voiceData)
            {
yield break;
            }

            private void OnAudioGroupAssetLoaded(ScriptableObject scriptableObject)
            {
            }
        }

#if UNITY_EDITOR
        [NonSerialized]
        [ShowInInspector]
        [OnValueChanged(nameof(SplitAddresses))]
        public string concatenatedAddresses = "";
        private bool splitting = false;

        private void SplitAddresses()
        {
            if (splitting)
            {
                return;
            }
            splitting = true;
            HashSet<string> existingAddresses = new();
            foreach (Dialog dialog in dialogs)
            {
                existingAddresses.Add(dialog.audioGroupAddress);
            }
            foreach (string address in concatenatedAddresses.Split(','))
            {
                if (existingAddresses.Contains(address))
                {
                    continue;
                }
                string[] addressSplit = address.Split('.');
                dialogs.Add(new Dialog() { id = addressSplit[addressSplit.Length - 1], mixerChannel = AudioMixerName.Voice, audioGroupAddress = address });
            }
            concatenatedAddresses = "";
            splitting = false;
        }

        [Button]
        public void CrossCheckTranscriptionIDs(string groupID = "Dialog")
        {
            HashSet<string> combinedSet = new();
            HashSet<string> textSet = new();
            HashSet<string> transcriptionIDset = new();
            TextData english = Catalog.GetData<TextData>("English");
            if (english != null)
            {
                foreach (TextData.TextGroup group in english.textGroups)
                {
                    if (group.id.ToLower() != groupID.ToLower())
                    {
                        continue;
                    }
                    foreach (TextData.TextID text in group.texts)
                    {
                        combinedSet.Add(text.id.ToLower());
                        textSet.Add(text.id.ToLower());
                    }
                    break;
                }
            }
            foreach (Dialog dialog in dialogs)
            {
                ScriptableObject scriptable = Catalog.EditorLoad<ScriptableObject>(dialog.audioGroupAddress);
                if (scriptable is AdvancedAudioContainer advancedAudioContainer)
                {
                    foreach (AdvancedAudioContainer.AudioClipData clip in advancedAudioContainer.clips)
                    {
                        combinedSet.Add(clip.transcriptionTextId.ToLower());
                        transcriptionIDset.Add(clip.transcriptionTextId.ToLower());
                    }
                }
            }
            foreach (string id in combinedSet)
            {
                bool inText = textSet.Contains(id);
                bool inContainer = transcriptionIDset.Contains(id);
                if (!inText || !inContainer)
                {
                    Debug.LogError($"Found mismatch in dialog subtitles: {id} was{(inText ? "" : "n't")} found in TextData, was{(inContainer ? "" : "n't")} found in AdvancedAudioContainer");
                }
            }
        }
#endif


    }
}
