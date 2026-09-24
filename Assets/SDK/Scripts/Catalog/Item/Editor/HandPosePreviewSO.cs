using System.IO;
using System.Linq;
using TriInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace ThunderRoad
{
    /// <summary>ScriptableObject that stores hand pose preview mappings.</summary>
    [CreateAssetMenu(menuName = "ThunderRoad/Editor/Hand Pose Preview Asset")]
    public class HandPosePreviewSO : ScriptableObject
    {
        [Dropdown(nameof(GetAllCreatureIDs))]
        public string creatureId;
        public GameObject previewPrefabLeft;
        public GameObject previewPrefabRight;

        /// <summary>Returns a dropdown list of all creature IDs in the catalog.</summary>
        public TriDropdownList<string> GetAllCreatureIDs() => Catalog.GetDropdownAllID(Category.Creature);
#if UNITY_EDITOR

        /// <summary>Finds the hand pose preview SO for a given creatureId.</summary>
        public static HandPosePreviewSO FindHandPosePreview(string creatureId)
        {
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(HandPosePreviewSO).Name}");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                HandPosePreviewSO asset = AssetDatabase.LoadAssetAtPath<HandPosePreviewSO>(path);

                if (asset != null && asset.creatureId == creatureId)
                    return asset;
            }
            return null;
        }

        /// <summary>Gets the preview prefab for a side.</summary>
        public GameObject GetPrefab(Side side) => side == Side.Left ? previewPrefabLeft : previewPrefabRight;

        /// <summary>Generates hand preview prefabs from the creature prefab.</summary>
        [Button("Generate Hand Preview Prefabs")]
        public void GenerateHandPreviewPrefabs()
        {
            GenerateHandPreviewPrefab(Side.Left);
            GenerateHandPreviewPrefab(Side.Right);
            AssetDatabase.SaveAssets();
        }

        /// <summary>Generates the hand preview prefab for a side from the creature prefab.</summary>
        private void GenerateHandPreviewPrefab(Side side)
        {
            CreatureData creatureData = Catalog.GetData<CreatureData>(creatureId);
            if (creatureData == null)
                return;
            Creature creature = GameObject.Instantiate(Catalog.EditorLoad<GameObject>(creatureData.prefabAddress).GetComponent<Creature>());
            RagdollHand hand = creature.GetComponentsInChildren<RagdollHand>().FirstOrDefault(r => r.side == side);

            GameObject previewPrefab = new GameObject($"{creatureId}_Hand{hand.side}_Preview");
            // move RagdollHand to prefab
            hand.transform.SetParent(previewPrefab.transform);
            // delete things we don't need
            Object.DestroyImmediate(hand.GetComponent<CharacterJoint>());
            Object.DestroyImmediate(hand.GetComponent<Rigidbody>());
            Object.DestroyImmediate(hand.GetComponent<WristRelaxer>());
            // disable any scripts
            foreach (Behaviour behaviour in hand.GetComponentsInChildren<Behaviour>())
                behaviour.enabled = false;

            GameObject mesh = new GameObject("Mesh");
            mesh.transform.SetParent(hand.transform);
            // move rig to prefab
            creature.GetComponentInChildren<Ragdoll>().meshRig.SetParent(mesh.transform);
            // move all renderers to prefab. Ideally just the hand mesh but can't know for sure which one it is
            LODGroup lodGroup = creature.GetComponentInChildren<LODGroup>();
            if (lodGroup != null)
            {
                LOD[] lods = lodGroup.GetLODs();
                foreach (Renderer renderer in lods[0].renderers)
                {
                    renderer.transform.SetParent(mesh.transform);
                    foreach (Component component in renderer.GetComponentsInChildren<Component>())
                    {
                        if (component is not SkinnedMeshRenderer && component is not Transform)
                            Object.DestroyImmediate(component);
                    }
                }
            }
            else
            {
                foreach (SkinnedMeshRenderer renderer in creature.GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    renderer.transform.SetParent(mesh.transform);
                    foreach (Component component in renderer.GetComponentsInChildren<Component>())
                    {
                        if (component is not SkinnedMeshRenderer && component is not Transform)
                            Object.DestroyImmediate(component);
                    }
                }
            }

            var existingPrefab = hand.side == Side.Left ? previewPrefabLeft : previewPrefabRight;
            var assetPath = AssetDatabase.GetAssetPath(existingPrefab ? existingPrefab : this);
            var folderPath = Path.GetDirectoryName(assetPath);
            if (string.IsNullOrEmpty(folderPath))
                folderPath = "Assets/";
            var path = EditorUtility.SaveFilePanel("Save Hand Preview Prefab", folderPath, previewPrefab.name, "prefab");
            if (string.IsNullOrEmpty(path))
            {
                Object.DestroyImmediate(creature.gameObject);
                Object.DestroyImmediate(previewPrefab);
                return;
            }

            path = FileUtil.GetProjectRelativePath(path);

            existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (existingPrefab != null)
            {
                AssetDatabase.DeleteAsset(path);
            }

            if (hand.side == Side.Left)
                previewPrefabLeft = PrefabUtility.SaveAsPrefabAsset(previewPrefab, path);
            else
                previewPrefabRight = PrefabUtility.SaveAsPrefabAsset(previewPrefab, path);
            EditorUtility.SetDirty(this);

            Object.DestroyImmediate(creature.gameObject);
            Object.DestroyImmediate(previewPrefab);
        }
#endif
    }
}
