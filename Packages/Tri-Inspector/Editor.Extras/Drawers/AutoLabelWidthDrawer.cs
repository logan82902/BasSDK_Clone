using TriInspector;
using TriInspector.Drawers;
using UnityEditor;
using UnityEngine;

[assembly: RegisterTriAttributeDrawer(typeof(AutoLabelWidthDrawer), TriDrawerOrder.Decorator - 100)]

namespace TriInspector.Drawers
{
    public class AutoLabelWidthDrawer : TriAttributeDrawer<AutoLabelWidthAttribute>
    {
        public override void OnGUI(Rect position, TriProperty property, TriElement next)
        {
            if (property.TryGetAttribute<LabelWidthAttribute>(out _))
            {
                // if there's a label width attribute added to this, assume it should override the automatic label width
                next.OnGUI(position);
                return;
            }
            var oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 0;
            float defaultLabelWidth = EditorGUIUtility.labelWidth;
            float minWidth = Attribute.MinWidth;
            if (minWidth <= 0f) minWidth = defaultLabelWidth;
            float newWidth = EditorStyles.label.CalcSize(property.DisplayNameContent).x + Attribute.AddedWidth;
            if (newWidth < minWidth) newWidth = minWidth;
            EditorGUIUtility.labelWidth = newWidth;
            next.OnGUI(position);
#if ProjectCore
            EditorGUI.LabelField(new Rect(position.x, position.y, 2f, 2f), new GUIContent("", $"{newWidth}"));
#endif
            EditorGUIUtility.labelWidth = oldLabelWidth;
        }
    }
}