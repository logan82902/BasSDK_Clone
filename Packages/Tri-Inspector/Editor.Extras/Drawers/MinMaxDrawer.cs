using TriInspector;
using TriInspector.Drawers;
using UnityEngine;
using TriInspector.Elements;

[assembly: RegisterTriAttributeDrawer(typeof(MinMaxDrawer), TriDrawerOrder.Decorator)]

namespace TriInspector.Drawers
{
    public class MinMaxDrawer : TriAttributeDrawer<MinMaxSliderAttribute>
    {
        public override TriElement CreateElement(TriProperty property, TriElement next)
        {
            object source = null;
            if (property.Parent.Value != null) source = property.Parent.Value;
            if (property.IsArray)
            {
                return new TriListElement(property, listChildProp => CreateMinMaxSliderElement(listChildProp, source));
            }
            return CreateMinMaxSliderElement(property, source);
        }

        private TriMinMaxSliderElement CreateMinMaxSliderElement(TriProperty property, object source)
        {
            return new TriMinMaxSliderElement(property, Attribute.ShowFields, () => Attribute.GetMin(source), () => Attribute.GetMax(source), property.ValueType == typeof(Vector2Int));
        }
    }
}
