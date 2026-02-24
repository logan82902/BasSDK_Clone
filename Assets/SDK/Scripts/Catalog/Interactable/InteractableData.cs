using System;
using UnityEngine;


using TriInspector;

namespace ThunderRoad
{
    [Serializable]
#if UNITY_EDITOR
    [EditorLoadPriority(-5)]
#endif
    [DeclareBoxGroup("Interactable", Title = "Interactable")]
    public class InteractableData : CatalogData
    {
        [Group("Interactable")]
        public bool disableTouch;
        [Group("Interactable")]
        public bool highlighterViewHandFreeOnly = false;
        [Group("Interactable")]
        public bool ignoreWhenTouchConflict;
        [Group("Interactable")]
        public ReferencePoint referencePoint = ReferencePoint.Grip;

        [Group("Interactable")]
        public string localizationId;
        [Group("Interactable")]
        public string highlightDefaultTitle;
        [Group("Interactable")]
        [Multiline]
        public string highlightDefaultDesignation;

        public enum Action
        {
            UseStart,
            UseStop,
            AlternateUseStart,
            AlternateUseStop,
            Grab,
            Ungrab,
            DirLeft,
            DirRight,
            DirUp,
            DirDown,
            DirStop,
            Custom,
        }

        public enum ReferencePoint
        {
            HandRoot,
            Grip,
            IndexTip,
        }

        public override int GetCurrentVersion() => 3;

#if UNITY_EDITOR
       public override string GetCatalogPath()
        {
            string categoryName = Catalog.GetCategory(GetType()).ToString();
            if (categoryName.ToLowerInvariant()[^1] == 's') categoryName += "e";
            string interactableTypeString = "Generic";
            Type interactableType = GetType();
            if (this is HandleRagdollData)
            {
                interactableTypeString = "Ragdoll Handle";
            }
            else if (this is HandleData)
            {
                interactableTypeString = "Object Handle";
            }
            else if (this is HolderData)
            {
                interactableTypeString = "Holder";
            }
            string result = $"{categoryName}s/{interactableTypeString}";
            if (!groupPath.IsNullOrEmptyOrWhitespace()) result += $"/{groupPath}";
            if (result[result.Length - 1] == '/') result = result.Substring(0, result.Length - 1);
            return result;
        }
#endif
    }
}
