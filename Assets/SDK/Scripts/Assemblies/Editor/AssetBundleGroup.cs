using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEditor.AddressableAssets.Settings;


using TriInspector;
using UnityEditor;

namespace ThunderRoad
{
    
    [DeclareVerticalGroup("folder")]
    [DeclareHorizontalGroup("folder/folderButtons")]
    [DeclareHorizontalGroup("manifest")]
    [DeclareVerticalGroup("manifest/fields")]
    [DeclareHorizontalGroup("assets")]
    [DeclareVerticalGroup("assets/fields")]
    [CreateAssetMenu(menuName = "ThunderRoad/Editor/Asset bundles group")]
    public class AssetBundleGroup : ScriptableObject
    {
        [ReadOnly]
        [Group("folder")]
        [Title("Mod Folder")]
        [HideLabel]
        public string folderName;

        [HideInInspector]
        public bool isDefault;
        [Title("Asset Groups")]
        [Group("assets/fields")]
        public List<AddressableAssetGroup> addressableAssetGroups;

        [HideInInspector]
        public bool selected;
        [HideInInspector]
        public bool exportAfterBuild;
        [HideInInspector]
        public AssetBundleBuilderGUI.ExportMode exportMode = AssetBundleBuilderGUI.ExportMode.ToGame;
        
        [HideInInspector]
        public bool isMod => !isDefault;
        [HideInInspector]
        public bool zipCatalogFiles = false;
        
        [Title("Manifest")]
        [Group("manifest/fields")]
        [ShowIf("isMod")]
        public string modName;
        [Group("manifest/fields")]
        [ShowIf("isMod")]
        public string modDescription = "Unknown";
        [Group("manifest/fields")]
        [ShowIf("isMod")]
        public string modAuthor = "Unknown";
        [Group("manifest/fields")]
        [ShowIf("isMod")]
        public string modVersion = "1.0";
        [Group("manifest/fields")]
        [ShowIf("isMod")]
        public string modThumbnail;

        public void OnValidate()
        {
            // Make sure folder name is valid
            if (string.IsNullOrEmpty(folderName))
            {
                folderName = this.name;
            }
            if(isMod && string.IsNullOrEmpty(modName))
            {
                modName = this.name;
            }
        }
        
        [Button]
        [Group("folder/folderButtons")]
        public void RenameFolder()
        {
            //display a dialog to ask for the new folder name
            if (!EditorInputDialog.Show("Folder Name", "Please type the folder name to use for the mod", folderName, "OK", "Cancel", out string newFolderName))
            {
                return;
            }
            if (string.IsNullOrEmpty(newFolderName)) return;
            newFolderName = Path.GetFileNameWithoutExtension(newFolderName);
            string oldPath = Path.Combine(Application.dataPath, $"../BuildStaging/Catalogs/Mods/{folderName}");
            string newPath = Path.Combine(Application.dataPath, $"../BuildStaging/Catalogs/Mods/{newFolderName}");
            
            if (!Directory.Exists(oldPath))
            {
                CreateModCatalogFolder();
            }
            else
            {
                if (oldPath != newPath)
                {
                    Directory.Move(oldPath, newPath);
                    Debug.Log($"Renamed Catalog folder renamed from {oldPath} to {newPath}");
                }
            }
            //need to rename the addressable asset folders as well, for Windows and Android
            string oldWindowsPath = Path.Combine(Application.dataPath, $"../BuildStaging/AddressableAssets/Windows/{folderName}");
            if (Directory.Exists(oldWindowsPath))
            {
                string newWindowsPath = Path.Combine(Application.dataPath, $"../BuildStaging/AddressableAssets/Windows/{newFolderName}");
                if (oldWindowsPath != newWindowsPath)
                {
                    Directory.Move(oldWindowsPath, newWindowsPath);
                    Debug.Log($"Renamed Windows addressable assets folder from {oldWindowsPath} to {newWindowsPath}");
                }
            }
            string oldAndroidPath = Path.Combine(Application.dataPath, $"../BuildStaging/AddressableAssets/Android/{folderName}");
            if (Directory.Exists(oldAndroidPath))
            {
                string newAndroidPath = Path.Combine(Application.dataPath, $"../BuildStaging/AddressableAssets/Android/{newFolderName}");
                if (oldAndroidPath != newAndroidPath)
                {
                    Directory.Move(oldAndroidPath, newAndroidPath);
                    Debug.Log($"Renamed Android addressable assets folder from {oldAndroidPath} to {newAndroidPath}");
                }
            }
            //rename this asset to match the new folder name
            string assetPath = AssetDatabase.GetAssetPath(this);
            AssetDatabase.RenameAsset(assetPath, $"{newFolderName}.asset");
            Debug.Log($"Renamed asset from {assetPath} to {newFolderName}.asset");
            AssetDatabase.SaveAssets();
            folderName = newFolderName;
            
        }
        [Button("Update manifest")]
        [Group("manifest/fields")]
        [ShowIf("isMod")]
        public void CreateModCatalogFolder()
        {
            string path = Path.Combine(Application.dataPath, $"../BuildStaging/Catalogs/Mods/{folderName}");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string manifestTempFolderPath = AssetBundleBuilderGUI.GenerateManifest(this);
            AssetBundleBuilder.CopyDirectory(manifestTempFolderPath, path);
            Debug.Log($"Mod manifest created at {path}");
        }
        [Button]
        [Group("folder/folderButtons")]
        [ShowIf("isMod")]
        public void OpenCatalogFolder()
        {
            string path = Path.Combine(Application.dataPath, $"../BuildStaging/Catalogs/Mods/{folderName}");
            if (!Directory.Exists(path))
            {
                //display a dialog to ask if the user wants to create the folder
                if (EditorUtility.DisplayDialog("Folder does not exist", "The folder does not exist. Do you want to create it?", "Yes", "No"))
                {
                    CreateModCatalogFolder();
                }
                else
                {
                    return;
                }
            }
            //open the folder in explorer
            EditorUtility.RevealInFinder(path);
        }
        
        [Button]
        [Group("assets/fields")]
        public void CheckLabels()
        {
            CheckAddressableLabels(out string message);
            //do a popup with the message
            if (!string.IsNullOrEmpty(message))
            {
                EditorUtility.DisplayDialog("Missing labels", $"The following entries are missing Android or Windows labels:\n{message}", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("All good", "All entries have Android or Windows labels.", "OK");
            }
        }
        public bool CheckAddressableLabels(out string message)
        {
            message = string.Empty;
            bool missingLabels = false;
            StringBuilder sb = new StringBuilder();
            foreach (AddressableAssetGroup group in addressableAssetGroups)
            {
                foreach (AddressableAssetEntry entry in group.entries)
                {
                    if (!entry.labels.Contains("Android") && !entry.labels.Contains("Windows"))
                    {
                        Debug.LogWarning($"Entry {entry.address} in group {group.Name} has no Android or Windows label");
                        sb.AppendLine($"[{group.name}][{entry.address}]");
                        missingLabels = true;
                    }
                }
            }
            if (!missingLabels) return true;
            message = sb.ToString();
            return false;
        }
    }
}