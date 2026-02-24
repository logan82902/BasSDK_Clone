using UnityEngine;


using TriInspector;

namespace ThunderRoad
{
    public class MeshColliderRagdollTool : MonoBehaviour
    {
        public PhysicMaterial defaultMaterial;

        [Button]
        public void SetColliders()
        {
            var smrs = GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer smr in smrs)
            {
                if (!smr.TryGetComponent(out MeshCollider meshCollider))
                {
                    meshCollider = smr.gameObject.AddComponent<MeshCollider>();
                }
                meshCollider.sharedMesh = smr.sharedMesh;
                meshCollider.convex = true;
                meshCollider.material = defaultMaterial;
            }
            for (int i = smrs.Length - 1; i >= 0; i--)
            {
                Destroy(smrs[i]);
            }
        }
    }
}
