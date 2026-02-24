using System;
using TriInspector;
using TriInspector.Drawers;
using UnityEditor;
using UnityEngine;

[assembly: RegisterTriAttributeDrawer(typeof(NullCheckboxDrawer), TriDrawerOrder.Inspector)]

namespace TriInspector.Drawers
{
    public class NullCheckboxDrawer : TriAttributeDrawer<NullCheckboxAttribute>
    {
        public override float GetHeight(float width, TriProperty property, TriElement next)
        {
            return property.Value != null
                ? EditorGUIUtility.singleLineHeight + 2f + next.GetHeight(width)
                : EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, TriProperty property, TriElement next)
        {
            if (property.FieldType.IsPrimitive)
            {
                next.OnGUI(position);
                return;
            }
            bool fieldHasValue = property.Value != null;
            Rect checkboxRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            float boxOffset = EditorGUIUtility.singleLineHeight + 2f;
            float fieldWidth = position.width - boxOffset;
            Rect fieldRect = new Rect(position.x + boxOffset, position.y + boxOffset, fieldWidth, next.GetHeight(fieldWidth));
            bool fieldShouldHaveValue = EditorGUI.ToggleLeft(checkboxRect, property.GetDisplayNameContent(), fieldHasValue);
            if (fieldShouldHaveValue && !fieldHasValue)
            {
                property.SetValue(Activator.CreateInstance(property.FieldType));
                fieldHasValue = true;
            }
            if (!fieldShouldHaveValue && fieldHasValue)
            {
                property.SetValue(null);
                fieldHasValue = false;
            }
            if (fieldHasValue)
            {
                next.OnGUI(fieldRect);
            }
        }
    }
}
