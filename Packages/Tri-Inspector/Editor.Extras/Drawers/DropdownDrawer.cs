using TriInspector;
using TriInspector.Drawers;
using TriInspector.Elements;
using TriInspector.Resolvers;

[assembly: RegisterTriAttributeDrawer(typeof(DropdownDrawer<>), TriDrawerOrder.Decorator)]

namespace TriInspector.Drawers
{
    public class DropdownDrawer<T> : TriAttributeDrawer<DropdownAttribute>
    {
        private DropdownValuesResolver<T> _valuesResolver;

        public override TriExtensionInitializationResult Initialize(TriPropertyDefinition propertyDefinition)
        {
            _valuesResolver = DropdownValuesResolver<T>.Resolve(propertyDefinition, Attribute.Values);

            if (_valuesResolver.TryGetErrorString(out var error))
            {
                return error;
            }

            return TriExtensionInitializationResult.Ok;
        }

        public override TriElement CreateElement(TriProperty property, TriElement next)
        {
            if (Attribute.AppendToField)
            {
                return new TriAppendedDropdownElement(property, _valuesResolver.GetDropdownItems, Attribute.Searchable, next);
            }
            return new TriDropdownElement(property, _valuesResolver.GetDropdownItems, Attribute.Searchable);
        }
    }
}