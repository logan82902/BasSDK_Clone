using TriInspector;
using TriInspector.Drawers;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[assembly: RegisterTriAttributeDrawer(typeof(NoRecusionDrawer), TriDrawerOrder.Decorator)]

namespace TriInspector.Drawers
{
    public class NoRecusionDrawer : TriAttributeDrawer<NoRecursionAttribute>
    {
        public static bool insideNoRecursion = false;

        public override void OnGUI(Rect position, TriProperty property, TriElement next)
        {
            if (!insideNoRecursion)
            {
                insideNoRecursion = true;
                next.OnGUI(position);
                insideNoRecursion = false;
            }
            else
            {
                EditorGUI.LabelField(position, new GUIContent($"  ** <i>Field</i> \"{property.DisplayName}\" <i>hidden to avoid recursive UI drawing</i> **"), new GUIStyle(EditorStyles.label) { richText = true });
            }
        }

        public override float GetHeight(float width, TriProperty property, TriElement next)
        {
            float height = EditorGUIUtility.singleLineHeight;
            if (!insideNoRecursion)
            {
                insideNoRecursion = true;
                height = base.GetHeight(width, property, next);
                insideNoRecursion = false;
            }
            return height;
        }
    }
}
