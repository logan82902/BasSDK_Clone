using System;
using System.Collections.Generic;
using System.Linq;
using TriInspectorUnityInternalBridge;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TriInspector.Utilities
{
    internal static class TriManagedReferenceGui
    {
        public static void DrawTypeSelector(Rect rect, TriProperty property)
        {
            var typeName = property.ValueType != null
                ? TriTypeUtilities.GetTypeNiceName(property.ValueType)
                : "Null";
            var icon = AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/SDK/UI/TriInspectorIcons/object_cog.png");
            var typeNameContent = new GUIContent($" {typeName} ({property.FieldType.Name})", icon);

            GUIStyle objectRefStyle = new GUIStyle(EditorStyles.objectField)
            {
                imagePosition = ImagePosition.ImageLeft,
            };
            EditorGUIUtility.SetIconSize(new Vector2(objectRefStyle.fontSize, objectRefStyle.fontSize));
            if (GUI.Button(rect, typeNameContent, objectRefStyle))
            {
                var dropdown = new ReferenceTypeDropDown(property, new AdvancedDropdownState());
                dropdown.Show(rect);
                AdvancedDropdownProxy.SetShowHeader(dropdown, false);

                Event.current.Use();
            }
        }

        private class ReferenceTypeDropDown : AdvancedDropdown
        {
            private readonly TriProperty _property;

            public ReferenceTypeDropDown(TriProperty property, AdvancedDropdownState state) : base(state)
            {
                _property = property;
                minimumSize = new Vector2(0, 120);
            }

            protected override AdvancedDropdownItem BuildRoot()
            {
                var types = TriReflectionUtilities
                    .AllNonAbstractTypes
                    .Where(type => !typeof(Object).IsAssignableFrom(type))
                    .Where(type => _property.FieldType.IsAssignableFrom(type))
                    .Where(type => type.IsValueType || type.GetConstructor(Type.EmptyTypes) != null)
                    .Where(type => !type.ContainsGenericParameters)
                    .ToList();

                var root = new ReferenceTypeGroupItem("Type");
                root.AddChild(new ReferenceTypeItem(null));

                Dictionary<string, int> namespacePartCounts = new Dictionary<string, int>();

                foreach (var type in types)
                {
                    if (string.IsNullOrEmpty(type.Namespace)) continue;
                    foreach (string part in type.Namespace.Split('.'))
                    {
                        namespacePartCounts[part] = namespacePartCounts.TryGetValue(part, out int count) ? count + 1 : 1;
                    }
                }

                foreach (var type in types)
                {
                    IEnumerable<string> namespaceEnumerator = Array.Empty<string>();
                    if (!string.IsNullOrEmpty(type.Namespace))
                    {
                        IEnumerable<string> GetNamespaceEnumator()
                        {
                            foreach (string part in type.Namespace.Split('.'))
                            {
                                if (namespacePartCounts.TryGetValue(part, out int count) && count == types.Count) continue;
                                yield return part;
                            }
                        }
                        namespaceEnumerator = GetNamespaceEnumator();
                    }

                    root.AddTypeChild(type, namespaceEnumerator.GetEnumerator());
                }

                root.Build();

                return root;
            }

            protected override void ItemSelected(AdvancedDropdownItem item)
            {
                if (!(item is ReferenceTypeItem referenceTypeItem))
                {
                    return;
                }

                if (referenceTypeItem.Type == null)
                {
                    _property.SetValue(null);
                }
                else
                {
                    var instance = Activator.CreateInstance(referenceTypeItem.Type);
                    _property.SetValue(instance);
                }
            }

            private class ReferenceTypeGroupItem : AdvancedDropdownItem
            {
                private static readonly Texture2D ScriptIcon = EditorGUIUtility.FindTexture("cs Script Icon");

                private readonly List<ReferenceTypeItem> _childItems = new List<ReferenceTypeItem>();

                private readonly Dictionary<string, ReferenceTypeGroupItem> _childGroups =
                    new Dictionary<string, ReferenceTypeGroupItem>();

                public ReferenceTypeGroupItem(string name) : base(name)
                {
                }

                public void AddTypeChild(Type type, IEnumerator<string> namespaceRemaining)
                {
                    if (!namespaceRemaining.MoveNext())
                    {
                        _childItems.Add(new ReferenceTypeItem(type, ScriptIcon));
                        return;
                    }

                    var ns = namespaceRemaining.Current ?? "";

                    if (!_childGroups.TryGetValue(ns, out var child))
                    {
                        _childGroups[ns] = child = new ReferenceTypeGroupItem(ns);
                    }

                    child.AddTypeChild(type, namespaceRemaining);
                }

                public void Build()
                {
                    foreach (var child in _childGroups.Values.OrderBy(it => it.name))
                    {
                        AddChild(child);

                        child.Build();
                    }

                    foreach (var child in _childItems)
                    {
                        AddChild(child);
                    }
                }
            }

            private class ReferenceTypeItem : AdvancedDropdownItem
            {
                public ReferenceTypeItem(Type type, Texture2D preview = null)
                    : base(type != null ? TriTypeUtilities.GetTypeNiceName(type) : "Null")
                {
                    Type = type;
                    icon = preview;
                }

                public Type Type { get; }
            }
        }
    }
}