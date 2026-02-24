using TriInspector;
using TriInspector.Drawers;
using UnityEngine;
using TriInspector.Elements;

[assembly: RegisterTriAttributeDrawer(typeof(ColorPickerFieldDrawer), TriDrawerOrder.Decorator)]

namespace TriInspector.Drawers
{
    public class ColorPickerFieldDrawer : TriAttributeDrawer<ColorPickerFieldAttribute>
    {
        public override TriElement CreateElement(TriProperty property, TriElement next)
        {
            if (property.IsArray)
            {
                return new TriListElement(property, listChildProp => CreateColorPickerElement(listChildProp));
            }
            return CreateColorPickerElement(property);
        }

        private TriColorPickerFieldElement CreateColorPickerElement(TriProperty property)
        {
            return new TriColorPickerFieldElement(property, Attribute.EyeDropper, Attribute.Alpha, Attribute.HDR);
        }
    }
}
