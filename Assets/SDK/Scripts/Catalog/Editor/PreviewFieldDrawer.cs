using TriInspector.Elements;
using TriInspector;
using UnityEngine;
using UnityEditor;
using ThunderRoad;

[assembly: RegisterTriAttributeDrawer(typeof(PreviewFieldDrawer), TriDrawerOrder.Decorator)]

namespace ThunderRoad
{
    public class PreviewFieldDrawer : TriAttributeDrawer<PreviewFieldAttribute>
    {
        private float _height;

        public override TriExtensionInitializationResult Initialize(TriPropertyDefinition propertyDefinition)
        {
            _height = Attribute.Height;
            if (Mathf.Approximately(_height, 0f))
            {
                _height = EditorGUIUtility.singleLineHeight * Attribute.LinesHeight;
            }
            return TriExtensionInitializationResult.Ok;
        }

        public override TriElement CreateElement(TriProperty property, TriElement next)
        {
            if (!property.ValueType.IsSubclassOf(typeof(UnityEngine.Object)))
            {
                return base.CreateElement(property, next);
            }
            return new PreviewFieldElement(property, _height, Attribute.SceneObjects, Attribute.LabelTop);
        }
    }
}
