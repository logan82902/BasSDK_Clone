using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TriInspector;
using UnityEngine;

namespace ThunderRoad
{
    [System.Serializable]
    [InlineProperty]
    public abstract class UnitySerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        [HideInInspector]
        private List<TKey> keyData = new();

        [SerializeField]
        [HideInInspector]
        private List<TValue> valueData = new();

#if UNITY_EDITOR
        [System.Serializable]
        [DeclareHorizontalGroup("KVP")]
        public class EditorKeyValuePair<K, V>
        {
            [Group("KVP")]
            [InlineProperty]
            [AutoLabelWidth]
            [GUIColor("$" + nameof(GetKeyColor))]
            public K Key;

            [Group("KVP")]
            [AutoLabelWidth]
            [SerializeField]
            public V Value;

            [HideInInspector]
            [NonSerialized]
            public bool KeySet = false;

            [HideInInspector]
            [NonSerialized]
            public bool KeyCollision;

            public Color GetKeyColor() => KeyCollision ? Color.red : Color.white;
        }

        private bool hasCollision = false;

        [ShowInInspector]
        [UseParentLabel]
        [HideReferencePicker]
        [InlineProperty]
        [JsonIgnore]
        [InfoBox("Key Collision detected! This Dictionary won't save its updated values until resolved!", TriMessageType.Error, nameof(hasCollision))]
        public List<EditorKeyValuePair<TKey, TValue>> displayList
        {
            get
            {
                if (internalPairList.IsNullOrEmpty())
                {
                    internalPairList = new List<EditorKeyValuePair<TKey, TValue>>();
                    foreach (KeyValuePair<TKey, TValue> kvp in this)
                    {
                        internalPairList.Add(new EditorKeyValuePair<TKey, TValue>()
                        {
                            Key = kvp.Key,
                            Value = kvp.Value,
                            KeySet = true,
                            KeyCollision = false,
                        });
                    }
                }
                return internalPairList;
            }
            set
            {
                internalPairList = value;
                hasCollision = false;
                Dictionary<TKey, TValue> testDict = new Dictionary<TKey, TValue>();
                for (int i = 0; i < internalPairList.Count; i++)
                {
                    EditorKeyValuePair<TKey, TValue> kvp = internalPairList[i];
                    kvp.KeyCollision = false;
                    if (!testDict.TryAdd(kvp.Key, kvp.Value))
                    {
                        kvp.KeyCollision = true;
                        hasCollision = true;
                    }
                }
                if (hasCollision) return;
                Clear();
                foreach (EditorKeyValuePair<TKey, TValue> kvp in value)
                {
                    if (!TryAdd(kvp.Key, kvp.Value)) Debug.LogError($"Tried to update Dictionary and encountered Key Collision on {kvp.Key.ToString()}! This should have been prevented already. Duplicate key skipped.");
                }
            }
        }

        private List<EditorKeyValuePair<TKey, TValue>> internalPairList;
#endif

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Clear();
            for (int i = 0; i < keyData.Count && i < valueData.Count; i++)
            {
                this[keyData[i]] = valueData[i];
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            keyData.Clear();
            valueData.Clear();

            foreach (KeyValuePair<TKey, TValue> item in this)
            {
                keyData.Add(item.Key);
                valueData.Add(item.Value);
            }
        }
    }
}
