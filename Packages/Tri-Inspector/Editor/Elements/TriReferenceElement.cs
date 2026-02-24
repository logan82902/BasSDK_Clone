using System;
using TriInspector.Utilities;
using UnityEditor;
using UnityEngine;

namespace TriInspector.Elements
{
    internal class TriReferenceElement : TriPropertyCollectionBaseElement
    {
        private readonly Props _props;
        private readonly TriProperty _property;
        private readonly bool _showReferencePicker;
        private readonly bool _referencePickerReadonly;
        private readonly bool _referencePickerScriptButton;
        private readonly bool _skipReferencePickerExtraLine;

        private Type _referenceType;

        [Serializable]
        public struct Props
        {
            public bool inline;
            public bool drawPrefixLabel;
            public float labelWidth;
        }

        public TriReferenceElement(TriProperty property, Props props = default)
        {
            _property = property;
            _props = props;
            _showReferencePicker = !property.TryGetAttribute(out HideReferencePickerAttribute _);
            _referencePickerReadonly = property.TryGetAttribute(out ReadOnlyReferencePickerAttribute _);
            _referencePickerScriptButton = property.TryGetAttribute(out ReferencePickerScriptButtonAttribute _);
            _skipReferencePickerExtraLine = !_showReferencePicker && _props.inline;
        }

        public override bool Update()
        {
            var dirty = false;

            if (ChildrenCount == 0)
            {
                GenerateChildren();
            }

            dirty |= base.Update();

            return dirty;
        }

        public override float GetHeight(float width)
        {
            var height = _skipReferencePickerExtraLine ? 0f : EditorGUIUtility.singleLineHeight;

            if (_props.inline || _property.IsExpanded)
            {
                height += base.GetHeight(width);
            }

            return height;
        }

        public override void OnGUI(Rect position)
        {
            if (_props.drawPrefixLabel)
            {
                //var controlId = GUIUtility.GetControlID(FocusType.Passive);
                //position = EditorGUI.PrefixLabel(position, controlId, _property.DisplayNameContent);
            }

            float labelWidth = EditorGUIUtility.labelWidth;
            GUIContent displayNameContent = GUIContent.none;
            if (TriPropertyOverrideContext.Current?.TryGetDisplayName(_property, out displayNameContent) == true && string.IsNullOrEmpty(displayNameContent.text))
            {
                labelWidth = 0f;
            }
            var headerRect = new Rect(position)
            {
                height = _skipReferencePickerExtraLine ? 0f : EditorGUIUtility.singleLineHeight,
            };
            var headerLabelRect = new Rect(position)
            {
                height = headerRect.height,
                width = labelWidth + 15f,
            };
            var headerFieldRect = new Rect(position)
            {
                height = headerRect.height,
                xMin = headerRect.xMin + labelWidth + 15f,
            };
            var contentRect = new Rect(position)
            {
                yMin = position.yMin + headerRect.height,
            };

            if (_props.inline)
            {
                if (_showReferencePicker)
                {
                    TriManagedReferenceGui.DrawTypeSelector(headerRect, _property);
                }

                using (TriGuiHelper.PushLabelWidth(_props.labelWidth))
                {
                    base.OnGUI(contentRect);
                }
            }
            else
            {
                if (_property.ChildrenProperties.Count != 0) TriEditorGUI.Foldout(headerLabelRect, _property);

                if (_showReferencePicker)
                {
                    bool showScriptButton = _referencePickerScriptButton && _property.Value != null;
                    GUIContent openScriptButton = new GUIContent("Open Script");
                    float buttonWidth = GUI.skin.button.CalcSize(openScriptButton).x;
                    if (showScriptButton)
                    {
                        headerFieldRect.width -= 2f + buttonWidth;
                    }
                    bool wasEnabled = GUI.enabled;
                    if (_referencePickerReadonly) GUI.enabled = false;
                    TriManagedReferenceGui.DrawTypeSelector(headerFieldRect, _property);
                    GUI.enabled = wasEnabled;
                    if (showScriptButton)
                    {
                        Rect buttonRect = new Rect(headerFieldRect) { x = headerFieldRect.x + headerFieldRect.width + 2f, width = buttonWidth };
                        if (GUI.Button(buttonRect, openScriptButton))
                        {
                            Type propertyType = _property.Value.GetType();
                            string fileName = propertyType.Name;
                            foreach (string guid in AssetDatabase.FindAssets(fileName))
                            {
                                string path = AssetDatabase.GUIDToAssetPath(guid);
                                if (string.IsNullOrEmpty(path)) continue;
                                MonoScript scriptAsset = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                                if (scriptAsset?.GetClass() != propertyType) continue;
                                AssetDatabase.OpenAsset(scriptAsset);
                            }
                        }
                    }
                }

                if (_property.IsExpanded)
                {
                    using (var indentedRectScope = TriGuiHelper.PushIndentedRect(contentRect, 1))
                    using (TriGuiHelper.PushLabelWidth(_props.labelWidth))
                    {
                        base.OnGUI(indentedRectScope.IndentedRect);
                    }
                }
            }
        }

        private bool GenerateChildren()
        {
            if (_property.ValueType == _referenceType)
            {
                return false;
            }

            _referenceType = _property.ValueType;

            RemoveAllChildren();

            ClearGroups();
            DeclareGroups(_property.ValueType);

            foreach (var childProperty in _property.ChildrenProperties)
            {
                AddProperty(childProperty);
            }

            return true;
        }

        private bool ClearChildren()
        {
            if (ChildrenCount == 0)
            {
                return false;
            }

            _referenceType = null;
            RemoveAllChildren();

            return true;
        }
    }
}