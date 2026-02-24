#define SHADOWOODTRI
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TriInspector;
using TriInspector.Drawers;
using UnityEditor;
using UnityEngine;

[assembly: RegisterTriAttributeDrawer(typeof(EnumToggleButtonsDrawer), TriDrawerOrder.Drawer,
    ApplyOnArrayElement = true)]

namespace TriInspector.Drawers
{
    public class EnumToggleButtonsDrawer : TriAttributeDrawer<EnumToggleButtonsAttribute>
    {
        public override TriExtensionInitializationResult Initialize(TriPropertyDefinition propertyDefinition)
        {
            if (!propertyDefinition.FieldType.IsEnum)
            {
                return "EnumToggleButtons attribute can be used only on enums";
            }

            return TriExtensionInitializationResult.Ok;
        }

        public override TriElement CreateElement(TriProperty property, TriElement next)
        {
#if SHADOWOODTRI
            return new EnumToggleButtonsElement(property, Attribute);
#else
            return new EnumToggleButtonsElement(property);
#endif
        }

        private sealed class EnumToggleButtonsElement : TriElement
        {
            private readonly TriProperty _property;
#if SHADOWOODTRI
            private readonly EnumToggleButtonsAttribute _attribute;
#endif
            private readonly List<KeyValuePair<string, Enum>> _enumValues;
            private readonly bool _isFlags;

#if SHADOWOODTRI
            public EnumToggleButtonsElement(TriProperty property, EnumToggleButtonsAttribute attribute)
            {
                _property = property;
                _attribute = attribute;

                var enumType = property.FieldType;
                var names = Enum.GetNames(enumType);
                var values = Enum.GetValues(enumType).OfType<Enum>().ToList();

                _enumValues = new List<KeyValuePair<string, Enum>>();

                for (var i = 0; i < names.Length; i++)
                {
                    var name = names[i];
                    var value = values[i];
                    var field = enumType.GetField(name);
                    var inspectorNameAttribute = field?.GetCustomAttribute<InspectorNameAttribute>();

                    if (inspectorNameAttribute != null)
                    {
                        name = inspectorNameAttribute.displayName;
                    }
                    else
                    {
                        name = ObjectNames.NicifyVariableName(name);
                    }

                    _enumValues.Add(new KeyValuePair<string, Enum>(name, value));
                }

                _isFlags = property.FieldType.GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0;

                _enumValues.Sort(new DeclarationOrderComparer(property.FieldType, names));
            }
#else
            public EnumToggleButtonsElement(TriProperty property)
            {
                _property = property;
                _enumValues = Enum.GetNames(property.FieldType)
                    .Zip(Enum.GetValues(property.FieldType).OfType<Enum>(),
                        (name, value) => new KeyValuePair<string, Enum>(name, value))
                    .ToList();
                _isFlags = property.FieldType.GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0;

                _enumValues.Sort(new DeclarationOrderComparer(property.FieldType));
            }
#endif

            public override float GetHeight(float width)
            {
#if SHADOWOODTRI
                var rowHeight = EditorGUIUtility.singleLineHeight;
                var spacing = EditorGUIUtility.standardVerticalSpacing;
                var rows = GetRowsCount();

                return rowHeight * rows + spacing * (rows - 1);
#else
                return EditorGUIUtility.singleLineHeight;
#endif
            }

            public override void OnGUI(Rect position)
            {
                var value = _property.TryGetSerializedProperty(out var serializedProperty)
                    ? (Enum) Enum.ToObject(_property.FieldType, serializedProperty.longValue)
                    : (Enum) _property.Value;

                var controlId = GUIUtility.GetControlID(FocusType.Passive);
                position = EditorGUI.PrefixLabel(position, controlId, _property.DisplayNameContent);

#if SHADOWOODTRI
                var columns = _attribute.Columns > 0 ? _attribute.Columns : _enumValues.Count;
                var rowHeight = EditorGUIUtility.singleLineHeight;
                var spacing = EditorGUIUtility.standardVerticalSpacing;

                for (var i = 0; i < _enumValues.Count; i++)
                {
                    var row = i / columns;
                    var col = i % columns;

                    var totalItemsInRow = Math.Min(columns, _enumValues.Count - row * columns);

                    var itemRect = position;
                    itemRect.height = rowHeight;
                    itemRect.y += row * (rowHeight + spacing);
                    itemRect.width /= totalItemsInRow;
                    itemRect.x += itemRect.width * col;

                    var itemStyle = GetButtonStyle(totalItemsInRow, col);
                    var itemName = _enumValues[i].Key;
                    var itemValue = _enumValues[i].Value;

                    var oldSelected = value != null && (_isFlags ? value.HasFlag(itemValue) : value.Equals(itemValue));
                    var newSelected = GUI.Toggle(itemRect, oldSelected, itemName, itemStyle);

                    if (oldSelected != newSelected)
                    {
                        if (_isFlags)
                        {
                            var newValue = newSelected
                                ? (Convert.ToInt64(value) | Convert.ToInt64(itemValue))
                                : (Convert.ToInt64(value) & ~Convert.ToInt64(itemValue));

                            _property.SetValue((Enum) Enum.ToObject(_property.FieldType, newValue));
                        }
                        else
                        {
                            _property.SetValue(itemValue);
                        }
                    }
                }
#else
                for (var i = 0; i < _enumValues.Count; i++)
                {
                    var itemRect = SplitRectWidth(position, _enumValues.Count, i);
                    var itemStyle = GetButtonStyle(_enumValues.Count, i);
                    var itemName = _enumValues[i].Key;
                    var itemValue = _enumValues[i].Value;

                    var oldSelected = value != null && (_isFlags ? value.HasFlag(itemValue) : value.Equals(itemValue));
                    var newSelected = GUI.Toggle(itemRect, oldSelected, itemName, itemStyle);

                    if (oldSelected != newSelected)
                    {
                        if (_isFlags)
                        {
                            var newValue = newSelected
                                ? (Convert.ToInt64(value) | Convert.ToInt64(itemValue))
                                : (Convert.ToInt64(value) & ~Convert.ToInt64(itemValue));

                            _property.SetValue((Enum) Enum.ToObject(_property.FieldType, newValue));
                        }
                        else
                        {
                            _property.SetValue(itemValue);
                        }
                    }
                }
#endif
            }

#if SHADOWOODTRI
            private int GetRowsCount()
            {
                if (_enumValues.Count == 0)
                {
                    return 1;
                }

                var columns = _attribute.Columns > 0 ? _attribute.Columns : _enumValues.Count;
                return Mathf.CeilToInt((float) _enumValues.Count / columns);
            }
#endif

            private static GUIStyle GetButtonStyle(int total, int current)
            {
                if (total <= 1)
                {
                    return EditorStyles.miniButton;
                }

                if (current == 0)
                {
                    return EditorStyles.miniButtonLeft;
                }

                if (current == total - 1)
                {
                    return EditorStyles.miniButtonRight;
                }

                return EditorStyles.miniButtonMid;
            }

#if !SHADOWOODTRI
            private static Rect SplitRectWidth(Rect rect, int total, int current)
            {
                if (total == 0)
                {
                    return rect;
                }

                rect.width /= total;
                rect.x += rect.width * current;
                return rect;
            }
#endif


            private class DeclarationOrderComparer : IComparer<KeyValuePair<string, Enum>>
            {
                private readonly FieldInfo[] _fields;
#if SHADOWOODTRI
                private readonly string[] _rawNames;

                public DeclarationOrderComparer(Type enumType, string[] rawNames)
                {
                    _fields = enumType.GetFields(BindingFlags.Static | BindingFlags.Public);
                    _rawNames = rawNames;
                }

                public int Compare(KeyValuePair<string, Enum> x, KeyValuePair<string, Enum> y)
                {
                    var rawNameX = _rawNames[Array.FindIndex(_fields, it =>
                    {
                        var attr = it.GetCustomAttribute<InspectorNameAttribute>();
                        var displayName = attr != null ? attr.displayName : ObjectNames.NicifyVariableName(it.Name);
                        return displayName == x.Key;
                    })];
                    var rawNameY = _rawNames[Array.FindIndex(_fields, it =>
                    {
                        var attr = it.GetCustomAttribute<InspectorNameAttribute>();
                        var displayName = attr != null ? attr.displayName : ObjectNames.NicifyVariableName(it.Name);
                        return displayName == y.Key;
                    })];

                    var orderX = Array.FindIndex(_fields, it => it.Name == rawNameX);
                    var orderY = Array.FindIndex(_fields, it => it.Name == rawNameY);
                    return orderX.CompareTo(orderY);
                }
#else
                public DeclarationOrderComparer(Type enumType)
                {
                    _fields = enumType.GetFields(BindingFlags.Static | BindingFlags.Public);
                }

                public int Compare(KeyValuePair<string, Enum> x, KeyValuePair<string, Enum> y)
                {
                    var orderX = Array.FindIndex(_fields, it => it.Name == x.Key);
                    var orderY = Array.FindIndex(_fields, it => it.Name == y.Key);
                    return orderX.CompareTo(orderY);
                }
#endif
            }
        }
    }
}