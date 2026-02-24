using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TriInspector;
using UnityEngine;

namespace ThunderRoad
{
    [Serializable]
    [AutoLabelWidth(MinWidth = 0f)]
    [DeclareHorizontalGroup("standaloneData")]
    [DeclareBoxGroup("standaloneData/Data", Title = "Data")]
    [DeclareHorizontalGroup("standaloneData/Data/Split")]
    [DeclareBoxGroup("showOwnershipFields/Modding", Title = "Modding")]
    public class CatalogData
    {
        [HideInInspector]
        [JsonProperty(Order = -2)]
        public string id;

        public virtual string SortKey() => id;

        [HideInInspector]
        [JsonProperty(Order = -2)]
        public BuildSettings.ContentFlag sensitiveContent = BuildSettings.ContentFlag.None;

        [HideInInspector]
        [JsonProperty(Order = -2)]
        public BuildSettings.ContentFlagBehaviour sensitiveFilterBehaviour = BuildSettings.ContentFlagBehaviour.Discard;

        [NonSerialized]
        [JsonProperty(Order = -2)]
        public string sourceFolders;

        public string GetLatestOverrideFolder()
        {
            if (string.IsNullOrEmpty(sourceFolders))
            {
                return GameSettings.loadDefaultFolders.Last();
            }
            return sourceFolders.Split('+').Last();
        }

        [NonSerialized]
        [JsonProperty(Order = -2)]
        public int hashId;

        [HideInInspector]
        [JsonProperty(Order = -2)]
        public int version;

        [HideInInspector]
        [JsonProperty(Order = -2)]
        public string groupPath;

        [NonSerialized]
        public string realFilePath;

        [NonSerialized]
        public string filePath;

        [NonSerialized]
        public ModManager.ModData owner;
        [NonSerialized]
        public List<ModManager.ModData> changers = new();

#if UNITY_EDITOR
        [NonSerialized]
        [JsonIgnore]
        public string editorGuid;
        [NonSerialized]
        [JsonIgnore]
        public string overrideID;
        [NonSerialized]
        [JsonIgnore]
        public List<CatalogData> overriderDatas;

        [JsonIgnore]
        public bool isOverrider => !overrideID.IsNullOrEmptyOrWhitespace();

        public string GetOwnerString() => owner == null ? "Vanilla" : $"{owner.Name} - {owner.Author}";

        public virtual string GetCatalogPath()
        {
            string categoryName = Catalog.GetCategory(GetType()).ToString();
            if (categoryName.ToLowerInvariant()[^1] == 's') categoryName += "e";
            string result = $"{categoryName}s";
            if (!groupPath.IsNullOrEmptyOrWhitespace()) result += $"/{groupPath}";
            if (result[result.Length - 1] == '/') result = result.Substring(0, result.Length - 1);
            return result;
        }

        public virtual string GetDataName()
        {
            return id;
        }

        public GUIContent GetCatalogEditorButtonContent()
        {
            Texture2D icon = Icon();
            // only display the first four characters of the editor GUID here since we don't need additional information
            GUIContent buttonContent = new GUIContent($"{(icon != null ? " " : "")}{GetDataName()}{(isOverrider ? $" ({editorGuid.Substring(0, 4)})" : "")}", icon);
            return buttonContent;
        }

        public virtual Texture2D Icon() => null;

        public virtual void OnEditorSave() { }

        public virtual string GetFileNameString()
        {
            Category category = Category.Custom;
            Catalog.TryGetCategory(GetType(), out category);
            return $"{category}_{id}";
        }
#endif

        public virtual int GetCurrentVersion() => 0;

        public virtual void Init()
        {
            hashId = Animator.StringToHash(id.ToLower());
#if UNITY_EDITOR
            if (Catalog.loadOverridesSeparately && isOverrider)
            {
                hashId = Animator.StringToHash(editorGuid.ToLower());
            }
#endif
        }

        public virtual void OnCatalogRefresh()
        {
            version = GetCurrentVersion();
        }

        public virtual CatalogData Clone()
        {
            return MemberwiseClone() as CatalogData;
        }

        public virtual IEnumerator OnCatalogRefreshCoroutine()
        {
            return null;
        }

        public virtual IEnumerator LoadAddressableAssetsCoroutine()
        {
            return null;
        }

        public virtual void ReleaseAddressableAssets()
        {

        }

        public virtual void OnLanguageChanged(string language)
        {

        }
#if UNITY_EDITOR
        /// <summary>
        /// Called by odin when the catalog editor is refreshed
        /// </summary>
        public virtual void CatalogEditorRefresh()
        {

        }
#endif
    }
}