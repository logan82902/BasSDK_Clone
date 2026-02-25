using System.Collections.Generic;
using Newtonsoft.Json;
using TriInspector;
using UnityEngine;
using System;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ThunderRoad
{
    [SkipInTypeDropdown]
    [System.Serializable]
    [DeclareHorizontalGroup("Fields", Sizes = new float[] { 150f, 0f })]
    [DeclareBoxGroup("Fields/Type", Title = "Content Type")]
    [DeclareBoxGroup("Fields/Reference ID", Title = "Reference ID")]
    public abstract class ContainerContent
    {
        [JsonMergeKey(MergeKeyValueProvider = nameof(GetMergeKey))]
        [HideInInspector]
        public string mergeKey = "";

        [JsonIgnore]
        [ReadOnly]
        [ShowInInspector]
        [Group("Fields/Type")]
        [HideLabel]
        [PropertyOrder(0)]
        public string type => GetTypeString();
        [Dropdown(nameof(DropdownOptions))]
        [Group("Fields/Reference ID")]
        [HideLabel]
        [PropertyOrder(1)]
        public string referenceID;

        [JsonIgnore]
        public abstract CatalogData catalogData { get; }

        public ContainerContent()
        {
            mergeKey = GetMergeKey();
        }

        public static string GetMergeKey()
        {
            return Guid.NewGuid().ToString().Substring(0, 8);
        }

        public virtual ContainerContent GetEndContent() => Clone();

        public abstract ContainerContent Clone();

        public abstract bool OnCatalogRefresh();

        public virtual string GetOutput() => referenceID;

        public abstract string GetTypeString();

        public abstract TriDropdownList<string> DropdownOptions();
    }
}
