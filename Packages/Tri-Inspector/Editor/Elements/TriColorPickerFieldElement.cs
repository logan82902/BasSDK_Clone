using UnityEditor;
using UnityEngine;

namespace TriInspector.Elements
{
    public class TriColorPickerFieldElement : TriElement
    {
        private readonly TriProperty _property;
        private readonly bool _eyedropper;
        private readonly bool _alpha;
        private readonly bool _hdr;

        public TriColorPickerFieldElement(TriProperty property, bool eyedropper, bool alpha, bool hdr)
        {
            _property = property;
            _eyedropper = eyedropper;
            _alpha = alpha;
            _hdr = hdr;
        }

        public override float GetHeight(float width)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position)
        {
            bool hasSerialProp = _property.TryGetSerializedProperty(out SerializedProperty serializedProperty);
            if (hasSerialProp) EditorGUI.BeginProperty(position, GUIContent.none, serializedProperty);
            var controlId = GUIUtility.GetControlID(FocusType.Passive);
            position = EditorGUI.PrefixLabel(position, controlId, _property.DisplayNameContent);
            Color result = EditorGUI.ColorField(position, GUIContent.none, (Color)_property.Value, _eyedropper, _alpha, _hdr);
            if (hasSerialProp) EditorGUI.EndProperty();
            _property.SetValue(result);
        }
    }
}
