using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceLocations;
using ThunderRoad.Manikin;

using TriInspector;

namespace ThunderRoad
{
    [HelpURL("https://kospy.github.io/BasSDK/Components/ThunderRoad/Items/Wearable.html")]
    [AddComponentMenu("ThunderRoad/Wearable")]
    [DeclareBoxGroup("WARDROBE", Title = "WARDROBE")]
    public class Wearable : Interactable
    {
        public delegate void OnItemEquipped(Item item);
        public delegate void OnItemUnEquipped(string layer, Item item);
        public delegate void OnEditModeChanged(bool state);

#if UNITY_EDITOR
        [PropertySpace(0, 10)]
        [Group("WARDROBE")]
        [Dropdown(nameof(GetAllChannels))]
#endif
        public string wardrobeChannel;

#if UNITY_EDITOR
        [Group("WARDROBE")]
        [DropdownList(nameof(GetAllChannelLayers))]
#endif
        public WearableEntry[] wardrobeLayers;

        // Odin won't display the list without this...
        // Forgive me
        [System.Serializable]
        public class WearableEntry
        {
            [ReadOnly]
            public string layer;
            [ReadOnly]
            public int layerIndex;

            public override bool Equals(object obj)
            {
                if (obj is not WearableEntry wearableEntry) return false;
                return layerIndex == wearableEntry.layerIndex;
            }
        }

        #region EDITOR

        ///////////////////////////////////////////////////////////////////////////////////////////////
        /// This is for QOL, no need to create an editor script since this will be stripped anyway. //
        //////////////////////////////////////////////////////////////////////////////////////////////

#if UNITY_EDITOR
        [Button("Reset Layers")]
        private void GetLayers()
        {
            TriDropdownList<WearableEntry> entries = GetAllChannelLayers();
            wardrobeLayers = new WearableEntry[entries.Count];
            for (int i = 0; i < entries.Count; i++)
            {
                wardrobeLayers[i] = entries[i].Value;
            }
        }

        /// <summary>
        /// This will allow for easier channel selection instead of manually typing strings.
        /// </summary>
        public TriDropdownList<string> GetAllChannels()
        {
            // Cache.
            TriDropdownList<string> tmp = new();

            ManikinEditorLocationLabels manikinChannelLayers = ManikinEditorLocationLabels.Get();
            if (manikinChannelLayers != null)
            {
                var channelNames = manikinChannelLayers.GetChannelNames();

                // This automates the ID addition so we can just add to the array above.
                for (int i = 0; i < channelNames.Length; i++)
                {
                    string channelName = channelNames[i];
                    tmp.Add(channelName, channelName);
                }
            }
            return tmp;
        }

        public TriDropdownList<WearableEntry> GetAllChannelLayers()
        {
            // Cache.
            TriDropdownList<WearableEntry> tmp = new();
            //need a valid channel
            if (string.IsNullOrEmpty(wardrobeChannel))
                return tmp;

            ManikinEditorLocationLabels manikinChannelLayers = ManikinEditorLocationLabels.Get();
            if (manikinChannelLayers != null)
            {
                //get the layers for this channel
                var layers = manikinChannelLayers.GetLayerNamesByChannelName(wardrobeChannel);

                // This automates the ID addition so we can just add to the array above.
                for (int i = 0; i < layers.Length; i++)
                {
                    string layer = layers[i];
                    WearableEntry entry = new() { layer = layer, layerIndex = i };
                    tmp.Add(layer, entry);
                }
            }
            return tmp;
        }

#endif

        #endregion
    }
}
