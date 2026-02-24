using System;
using System.Collections;
using System.Collections.Generic;
using ThunderRoadVRKBSharedData;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;


using TriInspector;

namespace ThunderRoad
{
    [Serializable]
#if UNITY_EDITOR
    [EditorLoadPriority(15)]
#endif
    [DeclareBoxGroup("Prefab", Title = "Prefab")]
    [DeclareBoxGroup("Supported Languages", Title = "Supported Languages")]
    [DeclareBoxGroup("Keyboard Configuration", Title = "Keyboard Configuration")]
    public class KeyboardData : CatalogData
    {
        public TriDropdownList<string> GetAllTextId() => Catalog.GetDropdownAllID(Category.Text);
        // PREFAB
        [Group("Prefab")]
        public string prefabAddress;
        [NonSerialized]
        public IResourceLocation prefabLocation;
#if UNITY_EDITOR
        [Group("Prefab")]
        [LabelText("EditorPrefab")]
        [NonSerialized]
        [ShowInInspector]
        [DisableInPlayMode]
        [OnValueChanged(nameof(EditorUpdatePrefabAddress))]
        //[PreviewField]
        public GameObject editorPrefab;
#endif
        [Group("Supported Languages")]
        [Dropdown(nameof(GetAllTextId))]
        public HashSet<string> localizationIds;
        [Group("Keyboard Configuration")]
        public int RepeatDelayMilliseconds = 500;
        [Group("Keyboard Configuration")]
        public int RepeatRatePerSecond = 15;
        [Group("Keyboard Configuration")]
        public bool AllowSimultaneousKeyPresses = false;
        [Group("Keyboard Configuration")]
        public string placeholderText = "Enter Text..";
        [Group("Keyboard Configuration")]
        public bool PasswordMode = false;
        [Group("Keyboard Configuration")]
        public KeyboardConfiguration keyboardConfig;
#if UNITY_EDITOR
        public override void CatalogEditorRefresh()
        {
            editorPrefab = Catalog.EditorLoad<GameObject>(prefabAddress);
        }
        protected void EditorUpdatePrefabAddress()
        {
            if (editorPrefab) prefabAddress = Catalog.GetAddressFromPrefab(editorPrefab);
        }

#endif // UNITY_EDITOR
    }
}
