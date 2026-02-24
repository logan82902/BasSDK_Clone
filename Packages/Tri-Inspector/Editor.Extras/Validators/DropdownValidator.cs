using System.Collections.Generic;
using TriInspector;
using TriInspector.Resolvers;
using TriInspector.Validators;

[assembly: RegisterTriAttributeValidator(typeof(DropdownValidator<>))]

namespace TriInspector.Validators
{
    public class DropdownValidator<T> : TriAttributeValidator<DropdownAttribute>
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

        public override TriValidationResult Validate(TriProperty property)
        {
            var items = _valuesResolver.GetDropdownItems(property);
            if (!_valuesResolver.needsValidation) return TriValidationResult.Valid;
            foreach (var item in items)
            {
                if (property.Comparer.Equals(item.Value, property.Value))
                {
                    return TriValidationResult.Valid;
                }
            }

            var msg = $"Dropdown value '{property.Value}' not valid";

            switch (Attribute.ValidationMessageType)
            {
                case TriMessageType.Info:
                    return TriValidationResult.Info(msg);

                case TriMessageType.Warning:
                    return TriValidationResult.Warning(msg);

                case TriMessageType.Error:
                    return TriValidationResult.Error(msg);
            }

            return TriValidationResult.Valid;
        }
    }
}