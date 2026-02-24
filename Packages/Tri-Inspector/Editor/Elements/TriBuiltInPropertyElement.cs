using TriInspectorUnityInternalBridge;
using UnityEditor;
using UnityEngine;

namespace TriInspector.Elements
{
    internal class TriBuiltInPropertyElement : TriElement
    {
        private readonly TriProperty _property;
        private readonly PropertyHandlerProxy _propertyHandler;
        private readonly SerializedProperty _serializedProperty;

        public TriBuiltInPropertyElement(
            TriProperty property,
            SerializedProperty serializedProperty,
            PropertyHandlerProxy propertyHandler)
        {
            _property = property;
            _serializedProperty = serializedProperty;
            _propertyHandler = propertyHandler;
        }

        public override float GetHeight(float width)
        {
            GUIContent displayNameContent = _property.DisplayNameContent;
            if (TriPropertyOverrideContext.Current != null && TriPropertyOverrideContext.Current.TryGetDisplayName(_property, out var overrideName)) displayNameContent = overrideName;
            return _propertyHandler.GetHeight(_serializedProperty, displayNameContent, true);
        }

        public override void OnGUI(Rect position)
        {
            EditorGUI.BeginChangeCheck();

            if (_property.IsArrayElement &&
                _serializedProperty.propertyType == SerializedPropertyType.Generic &&
                _serializedProperty.hasVisibleChildren)
            {
                position.xMin += 12;
            }
            GUIContent displayNameContent = _property.DisplayNameContent;
            if (TriPropertyOverrideContext.Current != null && TriPropertyOverrideContext.Current.TryGetDisplayName(_property, out var overrideName)) displayNameContent = overrideName;
            _propertyHandler.OnGUI(position, _serializedProperty, displayNameContent, true);

            if (EditorGUI.EndChangeCheck())
            {
                _property.NotifyValueChanged();
            }
        }
    }
}