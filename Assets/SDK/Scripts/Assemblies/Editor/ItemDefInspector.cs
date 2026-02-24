using UnityEngine;
using UnityEditor;

namespace ThunderRoad
{
    [CustomEditor(typeof(Item))]
    public class ItemDefInspector : ThunderInspector
    {
        protected override void OnDisablePostDispose()
        {
            if (target)
            {
                EditorUtility.SetDirty(target);
                ((Item)target).showCoMHandle = false;
            }
        }

        protected override void OnCreateGUIPostCoreElement()
        {
            Item item = (Item)target;
            if (item.itemId == "")
            {
                EditorGUILayout.HelpBox("Do not forget to assign a valid Item ID.", MessageType.Error);
            }

            foreach (MeshCollider col in item.gameObject.GetComponentsInChildren<MeshCollider>())
            {
                if (!col.convex)
                {
                    EditorGUILayout.HelpBox("The mesh collider " + col.name + " has to be set to convex.", MessageType.Error);
                }
            }

            if (!item.GetComponentInChildren<Collider>())
            {
                EditorGUILayout.HelpBox("There are no colliders assigned to this object. It will fall through the ground.", MessageType.Error);
            }
            foreach (Collider collider in item.GetComponentsInChildren<Collider>())
            {
                if (!collider.sharedMaterial && collider != item.customInertiaTensorCollider)
                {
                    EditorGUILayout.HelpBox("Collider " + collider.name + " is missing a physics material and will not make sounds nor decals on impact.", MessageType.Warning);
                }
                if (!collider.GetComponentInParent<ColliderGroup>() && collider != item.customInertiaTensorCollider)
                {
                    EditorGUILayout.HelpBox("Collider " + collider.name + " do not have any Collider Group on parent.", MessageType.Warning);
                }
            }

            if (item.GetDefaultHolderPoint() == null)
            {
                EditorGUILayout.HelpBox("There is no holder point assigned to this object. It will not be able to be holstered.", MessageType.Warning);
            }

            if (item.parryPoint == null)
            {
                EditorGUILayout.HelpBox("There is no parry point assigned to this object. Enemies will not know how to weild it.", MessageType.Warning);
            }

            if (item.transform.lossyScale != Vector3.one)
            {
                EditorGUILayout.HelpBox("The scale of the Item definition object should set to 1. Sercale the Mesh object instead.", MessageType.Warning);
            }

            if (!item.mainHandleLeft || !item.mainHandleRight)
            {
                EditorGUILayout.HelpBox("There are no handles on this object. It will not be able to be picked up from the spawn book.", MessageType.Warning);
            }
        }

        private void OnSceneGUI()
        {
            Item item = (Item)target;
            if (item.showCoMHandle)
            {
                item.customCenterOfMass = Handles.DoPositionHandle(item.customCenterOfMass, Quaternion.LookRotation(item.transform.forward, item.transform.up));
                item.GetComponent<Rigidbody>().centerOfMass = item.customCenterOfMass;
            }
        }
    }
}
