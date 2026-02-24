using UnityEngine;
using UnityEditor;

namespace ThunderRoad
{
    [CustomEditor(typeof(Interactable), true)]
    public class InteractableDefInspector : PointToPoint
    {
        private Interactable interactable;

        protected override void OnCreateGUIPreCoreElement()
        {
            script = interactable = (Interactable)target;
            Item item = interactable.transform.GetComponentInParent<Item>();

            PointToPointButton(interactable.axisLength);
        }

        protected override void OnCreateGUIPostCoreElement()
        {
            if (interactable.transform.localScale != Vector3.one)
            {
                EditorGUILayout.HelpBox("Interactable GameObject scale must be set to 1.", MessageType.Error);
            }

            if (interactable.axisLength < 0)
            {
                EditorGUILayout.HelpBox("Axis length must be a positive number or zero.", MessageType.Error);
            }

            if (interactable.touchRadius <= 0)
            {
                EditorGUILayout.HelpBox("Touch radius must be a positive number.", MessageType.Error);
            }

            if (interactable is Handle handle)
            {
                if (handle.reach <= 0)
                {
                    EditorGUILayout.HelpBox("Handle reach must be a positive number.", MessageType.Error);
                }

                if (handle.slideToHandleOffset <= 0)
                {
                    EditorGUILayout.HelpBox("Slide to handle offset must be a positive number.", MessageType.Error);
                }
            }
        }

        protected override void OnEnablePostCore()
        {
            script = interactable = (Interactable)target;
            ResetPoints(interactable.axisLength);
        }

        private void OnSceneGUI()
        {
            script = interactable = (Interactable)target;
            UpdatePoints(ref interactable.axisLength);
        }
    }
}
