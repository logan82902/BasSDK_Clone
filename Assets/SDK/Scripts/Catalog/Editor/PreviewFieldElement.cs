using UnityEditor;
using UnityEngine;
using TriInspector;

namespace ThunderRoad
{
    public class PreviewFieldElement : TriElement
    {
        private readonly TriProperty _property;
        private readonly float _height;
        private readonly bool _allowSceneObjects;
        private readonly bool _labelTop;

        public PreviewFieldElement(TriProperty property, float height, bool allowSceneObjects, bool labelTop)
        {
            _property = property;
            _height = height;
            _allowSceneObjects = allowSceneObjects;
            _labelTop = labelTop;
        }

        public override float GetHeight(float width) => _labelTop ? _height + EditorGUIUtility.singleLineHeight + 2 : _height;

        public override void OnGUI(Rect position)
        {
            var controlId = GUIUtility.GetControlID(FocusType.Passive);
            position.height = _height;
            if (_labelTop)
            {
                EditorGUI.LabelField(position, _property.DisplayNameContent);
                position.y += EditorGUIUtility.singleLineHeight + 2;
            }
            else
            {
                position = EditorGUI.PrefixLabel(position, controlId, _property.DisplayNameContent);
            }
            position.height = position.width = _height;
            UnityEngine.Object fieldValue = EditorGUI.ObjectField(position, (UnityEngine.Object)_property.Value, _property.ValueType, _allowSceneObjects);
            if (fieldValue == (UnityEngine.Object)_property.Value) return;
            _property.SetValue(fieldValue);
        }
    }
}
