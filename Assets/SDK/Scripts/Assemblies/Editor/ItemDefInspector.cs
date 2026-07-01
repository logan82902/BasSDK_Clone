using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ThunderRoad
{
    [CustomEditor(typeof(Item))]
    public class ItemDefInspector : ThunderInspector
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Domain reload", "UDR0001:Domain Reload Analyzer", Justification = "<Pending>")]
        private static bool warningFoldoutOpen = true;

        private List<string> currentIssues;
        private int majorIssueCount = 0;

        protected override void OnDisablePostDispose()
        {
            if (target)
            {
                EditorUtility.SetDirty(target);
                ((Item)target).showCoMHandle = false;
            }
        }

        protected override void OnCreateGUIPreCoreElement()
        {
            Item item = (Item)target;
            currentIssues = item.CheckItemProblems();
            majorIssueCount = 0;
            foreach (string infoString in currentIssues)
            {
                if (!infoString.Contains("!ERROR!")) continue;
                DrawHelpBox(infoString);
                majorIssueCount++;
            }
        }

        protected override void OnCreateGUIPostCoreElement()
        {
            int warningCount = currentIssues.Count - majorIssueCount;
            warningFoldoutOpen = EditorGUILayout.Foldout(warningFoldoutOpen, $"Configuration Warnings ({warningCount})", warningCount > 0 ? EditorStyles.foldoutHeader : EditorStyles.foldout);
            if (warningFoldoutOpen)
            {
                foreach (string infoString in currentIssues)
                {
                    if (!infoString.Contains("!WARNING!")) continue;
                    DrawHelpBox(infoString);
                }
            }
        }

        private void DrawHelpBox(string message)
        {
            if (message.Contains("!ERROR!"))
            {
                EditorGUILayout.HelpBox(message.Replace("!ERROR!", ""), MessageType.Error);
                return;
            }
            if (message.Contains("!WARNING!"))
            {
                EditorGUILayout.HelpBox(message.Replace("!WARNING!", ""), MessageType.Warning);
                return;
            }
            EditorGUILayout.HelpBox(message, MessageType.Info);
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
