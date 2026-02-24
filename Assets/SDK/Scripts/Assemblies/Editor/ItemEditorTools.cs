using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ThunderRoad
{
    public class ItemEditorTools
    {
        
        [MenuItem("ThunderRoad (SDK)/Items/OnValidate and Resave")]
        static void ValidateItems()
        {
            Catalog.EditorLoadAllJson(true, includeMods: false);
            //get all the items
            List<ItemData> itemDatas = Catalog.GetDataList<ItemData>();
            //disable asset imports to batch it
            try
            {
                AssetDatabase.StartAssetEditing();
                foreach (ItemData itemData in itemDatas)
                {
                    if (string.IsNullOrEmpty(itemData.prefabAddress))
                    {
                        Debug.LogError($"Item {itemData.id} has no prefab address, skipping.");
                        continue;
                    }
                    var assetPath = Catalog.GetAssetPathFromAddress(itemData.prefabAddress);
                    if (string.IsNullOrEmpty(assetPath))
                    {
                        Debug.LogError($"Item {itemData.id} has no asset path, skipping.");
                        continue;
                    }
                    try
                    {
                        using (var editingScope = new PrefabUtility.EditPrefabContentsScope(assetPath))
                        {
                            var root = editingScope.prefabContentsRoot;

                            //get all Item components to handle breakables
                            var items = root.GetComponentsInChildren<Item>(true);
                            foreach (var it in items)
                            {
                                it.SetupDefaultComponents();
                            }
                            //mark the prefab as dirty to save changes
                            EditorUtility.SetDirty(root);
                            Debug.Log($"Validated Item {itemData.id} and saved prefab.");
                        }
                    } catch (Exception e){
                        Debug.LogError($"Error validating Item {itemData.id}: {e.Message}");
                    }
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
        }
        
        [MenuItem("ThunderRoad (SDK)/Items/Remove Preview References")]
        static void RemovePreviewReferences()
        {
            // Get the selected objects from the Project window
            Object[] selectedPrefabs = Selection.GetFiltered(typeof(GameObject), SelectionMode.Assets);

            // Check if any prefabs are selected
            if (selectedPrefabs.Length > 0)
            {
                // Iterate over each selected prefab
                foreach (Object prefab in selectedPrefabs)
                {
                    if (prefab is not GameObject gameObjectPrefab) continue;
                    string assetPath = AssetDatabase.GetAssetPath(gameObjectPrefab);
 
                    using (var editingScope = new PrefabUtility.EditPrefabContentsScope(assetPath))
                    {
                        var prefabRoot = editingScope.prefabContentsRoot;
                        if(!prefabRoot.TryGetComponent(out Item item)) continue;
                        var previews = prefabRoot.GetComponentsInChildren<Preview>();
                        foreach (var preview in previews)
                        {
                            if (preview.generatedIcon == null) continue;
                            Debug.Log($"{prefab.name} has preview component and a generated icon - removing icon reference");
                            preview.generatedIcon = null;
                        }
                    }
                }
            }
        }
        
        [MenuItem("ThunderRoad (SDK)/Items/Bake Item Preview Icons")]
        private static void BakePreviews()
        {
            Catalog.EditorLoadAllJson(true, includeMods: false);
            //get all the items
            List<ItemData> itemDatas = Catalog.GetDataList<ItemData>();
            foreach (ItemData itemData in itemDatas)
            {
                itemData.CatalogEditorRefresh();
                //get the item prefab
                if(itemData.editorPrefab == null) continue;
                //get the path to the item prefab
                AssetDatabase.OpenAsset(itemData.editorPrefab);
                //get the PrefabStage
                PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                
                //search for the preview components in the item prefab scene
                Preview[] preview = prefabStage.prefabContentsRoot.GetComponentsInChildren<Preview>();
                foreach (Preview p in preview)
                {
                    p.updateCatalog = true;
                    p.GenerateIcon();
                }
                //close the asset
                StageUtility.GoBackToPreviousStage();
            }
        }
    }
}
