using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ThunderRoad
{
    /// <summary>Custom inspector for <see cref="Item"/> that shows critical configuration errors above the TriInspector core and collapsible warnings below it.</summary>
    [CustomEditor(typeof(Item))]
    public class ItemDefInspector : ThunderInspector
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Domain reload", "UDR0001:Domain Reload Analyzer", Justification = "<Pending>")]
        private static bool warningFoldoutOpen = true;

        private List<Item.ItemProblem> currentIssues;
        private int majorIssueCount = 0;

        /// <inheritdoc/>
        protected override void OnDisablePostDispose()
        {
            if (target)
            {
                EditorUtility.SetDirty(target);
                ((Item)target).showCoMHandle = false;
            }
        }

        /// <inheritdoc/>
        protected override void OnCreateGUIPreCoreElement()
        {
            Item item = (Item)target;
            currentIssues = item.CheckItemProblems();
            majorIssueCount = 0;
            foreach (Item.ItemProblem infoString in currentIssues)
            {
                if (!infoString.message.Contains("!ERROR!")) continue;
                DrawHelpBox(infoString.message);
                majorIssueCount++;
            }
        }

        /// <inheritdoc/>
        protected override void OnCreateGUIPostCoreElement()
        {
            int warningCount = currentIssues.Count - majorIssueCount;
            warningFoldoutOpen = EditorGUILayout.Foldout(warningFoldoutOpen, $"Configuration Warnings ({warningCount})", warningCount > 0 ? EditorStyles.foldoutHeader : EditorStyles.foldout);
            if (warningFoldoutOpen)
            {
                foreach (Item.ItemProblem issue in currentIssues)
                {
                    if (!issue.message.Contains("!WARNING!")) continue;
                    DrawHelpBox(issue);
                }
            }
        }

        private void DrawHelpBox(Item.ItemProblem issue)
        {
            if (issue.message.Contains("!ERROR!"))
            {
                EditorGUILayout.HelpBox(issue.message.Replace("!ERROR!", ""), MessageType.Error);
                return;
            }
            if (issue.message.Contains("!WARNING!"))
            {
                EditorGUILayout.HelpBox(issue.message.Replace("!WARNING!", ""), MessageType.Warning);
                return;
            }
            EditorGUILayout.HelpBox(issue.message, MessageType.Info);
        }

        private void OnSceneGUI()
        {
            Item item = (Item)target;
            if (item.showCoMHandle)
            {
                Vector3 worldSpaceCoM = Handles.DoPositionHandle(item.transform.TransformPoint(item.customCenterOfMass), Quaternion.LookRotation(item.transform.forward, item.transform.up));
                item.customCenterOfMass = item.transform.InverseTransformPoint(worldSpaceCoM);
                item.GetComponent<Rigidbody>().centerOfMass = item.customCenterOfMass;
            }
        }
    }
}
