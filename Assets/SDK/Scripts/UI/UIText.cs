using System.Collections.Generic;
using TMPro;
using UnityEngine;

using TriInspector;

namespace ThunderRoad
{
    public class UIText : MonoBehaviour
    {
        [Dropdown(nameof(GetAllTextGroupID), AppendToField = true)]
        public string textGroupId;

        [Tooltip("Enable this flag to have dynamic localized texts set from script. *The group id must be set on the inspector.")]
        public bool setFromScript;

        [Multiline]
        public string text;
        public bool forceUpperCase;

        /// <summary>
        /// Invoked when this changes the target text component.
        /// </summary>
        public event System.Action TextChanged;
        public bool ForceEnglishForNonLatin = false; //should this UI text force english for non-latin languages?
        public bool ForceLatinFontForNonLatin = false;

        public TriDropdownList<string> GetAllTextGroupID() => Catalog.GetTextData().GetDropdownAllTextGroups();
    }
}
