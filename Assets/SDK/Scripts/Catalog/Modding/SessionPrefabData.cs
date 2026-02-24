using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace ThunderRoad
{
#if UNITY_EDITOR
    [EditorLoadPriority(5)]
#endif
    public class SessionPrefabData : CustomData
    {
        public string prefabAddress;
        [NonSerialized]
        private IResourceLocation prefabLocation;
        private GameObject prefab;
    }
}
