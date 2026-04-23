using UnityEngine;
using UnityEditor;

namespace ThunderRoad
{
    [CustomEditor(typeof(ItemMagnet))]
    public class ItemMagnetInspector : ThunderInspector
    {
        protected override void OnCreateGUIPreCoreElement()
        {
            ItemMagnet itemMagnet = (ItemMagnet)target;
            if (itemMagnet.GetComponentInParent<Rigidbody>() == null)
            {
                EditorGUILayout.HelpBox("ItemMagnet will encounter errors and not function without a Rigidbody component placed on the same GameObject or a parent GameObject!", MessageType.Error);
            }
        }
    }
}
