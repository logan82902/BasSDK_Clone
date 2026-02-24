using UnityEngine;
using UnityEngine.Audio;
using System;
using System.IO;
#if !UNITY_EDITOR    
using UnityEngine.AddressableAssets;
#endif

using TriInspector;

namespace ThunderRoad
{
    [HideMonoScript]
    [DeclareBoxGroup("General", Title = "General")]
    [DeclareBoxGroup("References", Title = "References")]
    [DeclareBoxGroup("Runtime", Title = "Runtime")]
    public class ThunderRoadSettings : ScriptableObject
    {
        const string address = "ThunderRoad.Settings";
        public static bool initialized { get; private set; }

        public static ThunderRoadSettings current
        {
            get
            {
                if (!initialized)
                {
                    ThunderRoadSettings thunderRoadSettings = null;
                    if (Application.isBatchMode)
                    {
                        // Seem that Addressables.LoadAssetAsync or op.WaitForCompletion() don't work in batch mode 
                        thunderRoadSettings = Catalog.EditorLoad<ThunderRoadSettings>(address);
                    }
                    else
                    {
#if UNITY_EDITOR
                        thunderRoadSettings = Catalog.EditorLoad<ThunderRoadSettings>(address);
#else
#if TESTINGLOCALLY
            return null;
#endif
                        var op = Addressables.LoadAssetAsync<ThunderRoadSettings>(address);
                        thunderRoadSettings = op.WaitForCompletion();
#endif
                    }

                    if (thunderRoadSettings)
                    {
                        thunderRoadSettings.Init();
                    }
                    else
                    {
#if !TESTINGLOCALLY
                        Debug.LogError("Could not load ThunderRoad settings from address: " + address);
                        Application.Quit();
#endif
                    }
                }
                return _current;
            }
        }

        private static ThunderRoadSettings _current;

#if UNITY_EDITOR
        [Group("General")]
        public string addressableEditorPath = "BuildStaging/AddressableAssets";
        [Group("General")]
        public string catalogsEditorPath = "BuildStaging/Catalogs";
#endif
        [Group("General")]
        public string areaSceneAddress = "Level.Areas";
        [Group("General")]
        public bool overrideData = true;
        [Group("General")]
        public LayerMask groundLayer = 1 << 0;
        [Group("General")]
        public GameSettings game;

        private void Init()
        {
            _current = this;
#if UNITY_EDITOR
            // Create folders if don't exist
            string catalogFullPath = Path.Combine(Directory.GetCurrentDirectory(), catalogsEditorPath);
            if (!Directory.Exists(catalogFullPath))
            {
                Directory.CreateDirectory(catalogFullPath);
                Debug.Log("Created folder " + catalogFullPath);
            }
            string buildFullPath = Path.Combine(Directory.GetCurrentDirectory(), addressableEditorPath);
            if (!Directory.Exists(buildFullPath))
            {
                Directory.CreateDirectory(buildFullPath);
                Debug.Log("Created folder " + buildFullPath);
            }
#endif


            initialized = true;
        }


        public bool TryGetStoreSettings(out StoreSettings storeSettings)
        {
            storeSettings = null;
            return storeSettings != null;
        }
    }

}