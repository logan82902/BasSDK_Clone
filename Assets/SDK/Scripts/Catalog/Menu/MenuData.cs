using System;
using System.Collections;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceLocations;


using TriInspector;

namespace ThunderRoad
{
    [Serializable]
#if UNITY_EDITOR
    [EditorLoadPriority(10)]
#endif
    [DeclareBoxGroup("Menu", Title = "Menu")]
    [DeclareBoxGroup("Instance", Title = "Instance")]
    public class MenuData : CatalogData
    {
        [Group("Menu")]
        public float order;

        [Group("Menu")]
        public bool isDefault;

        [Group("Menu")]
        public string nameId;

        [Group("Menu")]
        public string iconAddress;
        [NonSerialized]
        public IResourceLocation iconLocation;

        [Group("Menu")]
        public string iconRollhoverAddress;
        [NonSerialized]
        public IResourceLocation iconRollhoverLocation;

        [Group("Menu")]
        public string prefabAddress;
        [NonSerialized]
        public IResourceLocation prefabLocation;

        [Group("Menu")]
        [ShowInInspector]
        public MenuModule module;

        [Group("Instance")]
        [ShowInInspector]
        [ReadOnly]
        [NonSerialized]
        public Transform navigationPane;
        [Group("Instance")]
        [ShowInInspector]
        [ReadOnly]
        [NonSerialized]
        public Transform contentArea;

        public override int GetCurrentVersion()
        {
            return 1;
        }
    }
}
