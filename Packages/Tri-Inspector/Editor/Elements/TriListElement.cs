using System;
using System.Collections;
using System.Linq;
using TriInspectorUnityInternalBridge;
using TriInspector.Utilities;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace TriInspector.Elements
{
    public class TriListElement : TriElement
    {
        private const float ListExtraWidth = 7f;
        private const float DraggableAreaExtraWidth = 14f;

        private readonly TriProperty _property;
        private readonly Func<TriProperty, TriElement> _customChildFunction;
        private readonly ReorderableList _reorderableListGui;
        private readonly bool _alwaysExpanded;
        private readonly bool _showElementLabels;
        private readonly bool _dontForceInline;
        private readonly bool _useDropdown;
        private readonly bool _searchableDropdown;
        private readonly string _dropdownElementSource;
        private readonly int _itemsPerPage;

        private float _lastContentWidth;
        private bool _displayAll = false;
        private int _currentStartIndex = 0;

        protected readonly bool ignoreButtonSpacing;

        protected ReorderableList ListGui => _reorderableListGui;

        public TriListElement(TriProperty property, Func<TriProperty, TriElement> customChildFunction = null)
        {
            property.TryGetAttribute(out ListDrawerSettingsAttribute settings);

            _property = property;
            _customChildFunction = customChildFunction;
            _alwaysExpanded = settings?.AlwaysExpanded ?? false;
            _showElementLabels = settings?.ShowElementLabels ?? false;
            _dontForceInline = settings?.DontForceInline ?? false;
            ignoreButtonSpacing = settings?.IgnoreButtonSpacing ?? false;
            _itemsPerPage = settings?.ItemsPerPage ?? 0;
            _useDropdown = property.TryGetAttribute(out DropdownListAttribute dropdown);
            _searchableDropdown = dropdown?.Searchable ?? false;
            _dropdownElementSource = _useDropdown ? dropdown.Source : "";
            _reorderableListGui = new ReorderableList(null, _property.ArrayElementType)
            {
                draggable = settings?.Draggable ?? true,
                displayAdd = settings == null || !settings.HideAddButton,
                displayRemove = settings == null || !settings.HideRemoveButton,
                drawHeaderCallback = DrawHeaderCallback,
                elementHeightCallback = ElementHeightCallback,
                drawElementCallback = DrawElementCallback,
                onRemoveCallback = RemoveElementCallback,
                onReorderCallbackWithDetails = ReorderCallback,
            };

            if (property.ArrayElementType.IsAbstract)
            {
                _reorderableListGui.onAddDropdownCallback = AddElementDropdownCallBack;
            }
            else
            {
                _reorderableListGui.onAddCallback = AddElementCallback;
            }

            if (!_reorderableListGui.displayAdd && !_reorderableListGui.displayRemove)
            {
                _reorderableListGui.footerHeight = 0f;
            }
        }

        public override bool Update()
        {
            var dirty = false;

            if (_itemsPerPage <= 0)
            {
                if (_property.TryGetSerializedProperty(out var serializedProperty) && serializedProperty.isArray)
                {
                    _reorderableListGui.serializedProperty = serializedProperty;
                }
                else if (_property.Value != null)
                {
                    _reorderableListGui.list = (IList) _property.Value;
                }
                else if (_reorderableListGui.list == null)
                {
                    _reorderableListGui.list = (IList) (_property.FieldType.IsArray
                        ? Array.CreateInstance(_property.ArrayElementType, 0)
                        : Activator.CreateInstance(_property.FieldType));
                }
            }
            else
            {
                int elementCount = _displayAll ? _property.ArrayElementProperties.Count : _itemsPerPage;
                if (_currentStartIndex + _itemsPerPage >= _property.ArrayElementProperties.Count) elementCount = _property.ArrayElementProperties.Count - _currentStartIndex;
                if (_reorderableListGui.count != elementCount)
                {
                    _reorderableListGui.list = (IList) (_property.FieldType.IsArray
                        ? Array.CreateInstance(_property.ArrayElementType, 0)
                        : Activator.CreateInstance(_property.FieldType));
                    for (int i = 0; i < elementCount; i++)
                    {
                        _reorderableListGui.list.Add(null);
                    }
                }
            }

            if (_alwaysExpanded && !_property.IsExpanded)
            {
                _property.IsExpanded = true;
            }

            if (_property.IsExpanded)
            {
                dirty |= GenerateChildren();
            }
            else
            {
                dirty |= ClearChildren();
            }

            dirty |= base.Update();

            if (dirty)
            {
                ReorderableListProxy.ClearCacheRecursive(_reorderableListGui);
            }

            return dirty;
        }

        public override float GetHeight(float width)
        {
            if (!_property.IsExpanded)
            {
                return _reorderableListGui.headerHeight + 4f;
            }

            _lastContentWidth = width;

            float height = _reorderableListGui.GetHeight();
            if (ignoreButtonSpacing)
            {
                height -= EditorGUIUtility.singleLineHeight;
            }
            return height;
        }

        public override void OnGUI(Rect position)
        {
            if (!_property.IsExpanded)
            {
                ReorderableListProxy.DoListHeader(_reorderableListGui, new Rect(position)
                {
                    yMax = position.yMax - 4,
                });
                return;
            }

            var labelWidthExtra = ListExtraWidth + DraggableAreaExtraWidth;

            using (TriGuiHelper.PushLabelWidth(EditorGUIUtility.labelWidth - labelWidthExtra))
            {
                _reorderableListGui.DoList(position);
            }
        }

        private void AddElementDropdownCallBack(Rect rect, ReorderableList reorderableList)
        {
            void TypeSelected(object type)
            {
                object result = CreateObjectOfType((Type)type);
                AddElementCallback(reorderableList, result);
            }
            GenericMenu pickTypeDropdown = new GenericMenu();
            Type elementType = _property.ArrayElementType;
            foreach (Assembly assembly in TriReflectionUtilities.Assemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsAbstract) continue;
                    if (!elementType.IsAssignableFrom(type)) continue;
                    string typePath = type.Name;
                    Type parent = type.BaseType;
                    while (true)
                    {
                        if (parent == null || parent == typeof(object) || parent == elementType) break;
                        if (parent.GetCustomAttribute<SkipInTypeDropdownAttribute>() == null) typePath = $"{parent.Name}/{typePath}";
                        parent = parent.BaseType;
                    }
                    typePath = Regex.Replace(typePath, @"`\d+", "");
                    if (assembly != elementType.Assembly) typePath = $"{assembly.GetName().Name}/{typePath}";
                    pickTypeDropdown.AddItem(new GUIContent(typePath), false, TypeSelected, type);
                }
            }
            pickTypeDropdown.ShowAsContext();
        }

        private void AddElementCallback(ReorderableList reorderableList)
        {
            AddElementCallback(reorderableList, null);
        }

        private void AddElementCallback(ReorderableList reorderableList, object addedReferenceValue)
        {
            if (_property.TryGetSerializedProperty(out _) && addedReferenceValue is Object addedUnityObject)
            {
                ReorderableListProxy.DoAddButton(reorderableList, addedUnityObject);
                _property.NotifyValueChanged();
                return;
            }

            var template = CloneValue(_property);

            _property.SetValues(targetIndex =>
            {
                var value = (IList) _property.GetValue(targetIndex);

                if (_property.FieldType.IsArray)
                {
                    var array = Array.CreateInstance(_property.ArrayElementType, template.Length + 1);
                    Array.Copy(template, array, template.Length);

                    if (addedReferenceValue != null)
                    {
                        array.SetValue(addedReferenceValue, array.Length - 1);
                    }

                    value = array;
                }
                else
                {
                    if (value == null)
                    {
                        value = (IList) Activator.CreateInstance(_property.FieldType);
                    }

                    var newElement = addedReferenceValue != null
                        ? addedReferenceValue
                        : CreateDefaultElementValue(_property);

                    value.Add(newElement);
                }

                return value;
            });
        }

        private void RemoveElementCallback(ReorderableList reorderableList)
        {
            var mainValue = (IList) _property.Value;
            IList newList = (IList) (_property.FieldType.IsArray
                        ? Array.CreateInstance(_property.ArrayElementType, _property.ArrayElementProperties.Count - 1)
                        : Activator.CreateInstance(_property.FieldType));
            int removeIndex = reorderableList.index + (_displayAll ? 0 : _currentStartIndex);
            if (newList.IsFixedSize)
            {
                int index = 0;
                for (int i = 0; i < mainValue.Count; i++)
                {
                    if (i == removeIndex) continue;
                    newList[index] = mainValue[i];
                    index++;
                }
            }
            else
            {
                for (int i = 0; i < mainValue.Count; i++)
                {
                    if (i == removeIndex) continue;
                    newList.Add(mainValue[i]);
                }
            }
            _property.SetValue(newList);
            if (_currentStartIndex == mainValue.Count) _currentStartIndex -= _itemsPerPage;
            _property.NotifyValueChanged();
            // clear the children after remove so the list gets regenerated and doesn't cause errors
            ClearChildren();
        }

        private void ReorderCallback(ReorderableList list, int oldIndex, int newIndex)
        {
            bool paginationActive = _itemsPerPage > 0 && !_displayAll;
            bool listUpdated = (list.serializedProperty != null && list.serializedProperty.serializedObject != null) || list.list != null;
            // we need to manually update the list if pagination is active (not handled by default GUI) or if the list wasn't updated for some reason
            if (paginationActive || !listUpdated)
            {
                // if pagination is active, we can't trust that the default reorderable list GUI moved the elements correctly, so if the list was updated, undo that change
                if (listUpdated)
                {
                    if (list.serializedProperty != null && list.serializedProperty.serializedObject != null)
                    {
                        list.serializedProperty.MoveArrayElement(newIndex, oldIndex);
                    }
                    else if (list.list != null)
                    {
                        object value = list.list[newIndex];
                        for (int j = 0; j < list.list.Count - 1; j++)
                        {
                            if (j >= newIndex)
                            {
                                list.list[j] = list.list[j + 1];
                            }
                        }

                        for (int num3 = list.list.Count - 1; num3 > 0; num3--)
                        {
                            if (num3 > oldIndex)
                            {
                                list.list[num3] = list.list[num3 - 1];
                            }
                        }

                        list.list[oldIndex] = value;
                    }
                }
                // if we're using pagination, increase both indices by the current start index so we have the "real" indices
                oldIndex += _displayAll ? 0 : _currentStartIndex;
                newIndex += _displayAll ? 0 : _currentStartIndex;
                // this condition will probably never be hit, since oldIndex and newIndex should always be different prior to this change
                if (oldIndex == newIndex) return;

                var propList = (IList) _property.Value;
                var moveValue = propList[oldIndex];
                if (!propList.IsFixedSize)
                {
                    propList.RemoveAt(oldIndex);
                    propList.Insert(newIndex, moveValue);
                }
                else
                {
                    if (newIndex > oldIndex)
                    {
                        for (int i = oldIndex; i < newIndex; i++)
                        {
                            propList[i] = propList[i + 1];
                        }
                    }
                    else
                    {
                        for (int i = oldIndex; i > newIndex; i--)
                        {
                            propList[i] = propList[i - 1];
                        }
                    }
                    propList[newIndex] = moveValue;
                }
            }
            _property.NotifyValueChanged();
            // clear the children after we reorder, so that on the next update the children get refreshed. prevents error spam if an array has multiple element types in it
            ClearChildren();
        }

        private bool GenerateChildren()
        {
            var count = _property.ArrayElementProperties.Count;

            if (ChildrenCount == count)
            {
                return false;
            }

            while (ChildrenCount < count)
            {
                var property = _property.ArrayElementProperties[ChildrenCount];
                AddChild(CreateItemElement(property));
            }

            while (ChildrenCount > count)
            {
                RemoveChildAt(ChildrenCount - 1);
            }

            return true;
        }

        private bool ClearChildren()
        {
            if (ChildrenCount == 0)
            {
                return false;
            }

            RemoveAllChildren();

            return true;
        }

        protected virtual TriElement CreateItemElement(TriProperty property)
        {
            if (_customChildFunction != null) return _customChildFunction.Invoke(property);
            var element = new TriPropertyElement(property, new TriPropertyElement.Props
            {
                forceInline = !_showElementLabels && !_dontForceInline,
            });
            if (_useDropdown)
            {
                element.RemoveAllChildren();
                IEnumerable<ITriDropdownItem> ValuesGetter(TriProperty prop)
                {
                    object FindAndRunMethodRecursive(TriProperty searchProp, string methodName)
                    {
                        var target = searchProp.Value;
                        if (target == null) return FindAndRunMethodRecursive(searchProp.Parent, methodName);
                        var methodInfo = target.GetType().GetMethod(methodName);
                        if (methodInfo != null) return methodInfo.Invoke(target, null);
                        return FindAndRunMethodRecursive(searchProp.Parent, methodName);
                    }
                    var resultList = FindAndRunMethodRecursive(prop, _dropdownElementSource);
                    foreach (var item in resultList as IEnumerable)
                    {
                        yield return item as ITriDropdownItem;
                    }
                }
                element.AddChild(new TriDropdownElement(property, ValuesGetter, _searchableDropdown));
            }
            return element;
        }

        private void DrawHeaderCallback(Rect rect)
        {
            var labelRect = new Rect(rect);
            bool hasSerialProp = _property.TryGetSerializedProperty(out SerializedProperty serialProp);
            if (hasSerialProp)
            {
                EditorGUI.BeginProperty(labelRect, GUIContent.none, serialProp);
            }
            var arraySizeRect = new Rect(rect)
            {
                xMin = rect.xMax - 300,
            };
            if (_itemsPerPage > 0)
            {
                float buttonsWidth = (EditorGUIUtility.singleLineHeight * 2) + 86f;
                arraySizeRect.xMin -= buttonsWidth;
                arraySizeRect.xMax -= buttonsWidth;
                labelRect.xMax -= buttonsWidth;
            }

            if (_alwaysExpanded)
            {
                EditorGUI.LabelField(labelRect, _property.DisplayNameContent);
            }
            else
            {
                TriEditorGUI.Foldout(labelRect, _property);
            }

            int arrayCount = _property.ArrayElementProperties.Count;
            var label = arrayCount == 0 ? "Empty" : $"{arrayCount} items";
            GUI.Label(arraySizeRect, label, Styles.ItemsCount);
            if (_itemsPerPage > 0)
            {
                Rect buttonRect = new Rect(arraySizeRect)
                {
                    xMin = arraySizeRect.xMax + 2f,
                    width = EditorGUIUtility.singleLineHeight,
                    height = EditorGUIUtility.singleLineHeight,
                };
                GUIStyle buttonStyle = new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter };
                if (GUI.Button(buttonRect, "←", buttonStyle))
                {
                    GUI.FocusControl(null);
                    _currentStartIndex -= _itemsPerPage;
                    if (_currentStartIndex < 0) _currentStartIndex = 0;
                }
                buttonRect.x += EditorGUIUtility.singleLineHeight + 2f;
                GUIContent[] options = new GUIContent[Mathf.CeilToInt(_property.ArrayElementProperties.Count / (float) _itemsPerPage) + 1];
                for (int i = 0; i < options.Length; i++)
                {
                    int pageIndex = i - 1;
                    if (pageIndex >= 0)
                    {
                        int min = pageIndex * _itemsPerPage;
                        int max = Mathf.Min(min + _itemsPerPage, _property.ArrayElementProperties.Count) - 1;
                        options[i] = new GUIContent($"{min} - {max}");
                    }
                    else
                    {
                        options[i] = new GUIContent("All Items");
                    }
                }
                int currentPage = _displayAll ? 0 : 1 + (_currentStartIndex / _itemsPerPage);
                int pageDropdownSelection = EditorGUI.Popup(new Rect(buttonRect) { width = 80f }, currentPage, options, new GUIStyle(EditorStyles.popup) { alignment = TextAnchor.MiddleCenter });
                if (pageDropdownSelection != currentPage)
                {
                    GUI.FocusControl(null);
                    _displayAll = pageDropdownSelection == 0;
                    if (_displayAll)
                    {
                        _currentStartIndex = 0;
                    }
                    else
                    {
                        _currentStartIndex = (pageDropdownSelection - 1) * _itemsPerPage;
                    }
                }
                buttonRect.x += 82f;
                if (GUI.Button(buttonRect, "→", buttonStyle))
                {
                    GUI.FocusControl(null);
                    _currentStartIndex += _itemsPerPage;
                    if (_currentStartIndex >= arrayCount) _currentStartIndex -= _itemsPerPage;
                }
            }

            if (Event.current.type == EventType.DragUpdated && rect.Contains(Event.current.mousePosition))
            {
                DragAndDrop.visualMode = DragAndDrop.objectReferences.All(obj => TryGetDragAndDropObject(obj, out _))
                    ? DragAndDropVisualMode.Copy
                    : DragAndDropVisualMode.Rejected;

                Event.current.Use();
            }
            else if (Event.current.type == EventType.DragPerform && rect.Contains(Event.current.mousePosition))
            {
                DragAndDrop.AcceptDrag();

                foreach (var obj in DragAndDrop.objectReferences)
                {
                    if (TryGetDragAndDropObject(obj, out var addedReferenceValue))
                    {
                        AddElementCallback(_reorderableListGui, addedReferenceValue);
                    }
                }

                Event.current.Use();
            }
            if (hasSerialProp) EditorGUI.EndProperty();
        }

        private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            index += _displayAll ? 0 : _currentStartIndex;
            if (index >= ChildrenCount)
            {
                return;
            }

            if (!_reorderableListGui.draggable)
            {
                rect.xMin += DraggableAreaExtraWidth;
            }

            using (TriPropertyOverrideContext.BeginOverride(ListPropertyOverrideContext.Instance))
            {
                ListPropertyOverrideContext.SetIndex(index);
                GetChild(index).OnGUI(rect);
            }
        }

        private float ElementHeightCallback(int index)
        {
            index += _displayAll ? 0 : _currentStartIndex;
            if (index >= ChildrenCount)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            return GetChild(index).GetHeight(_lastContentWidth);
        }

        private static object CreateDefaultElementValue(TriProperty property) => CreateObjectOfType(property.ArrayElementType);

        private static object CreateObjectOfType(Type type)
        {
            var canActivate = type.IsValueType || type.GetConstructor(Type.EmptyTypes) != null;

            return canActivate ? Activator.CreateInstance(type) : null;
        }

        private static Array CloneValue(TriProperty property)
        {
            var list = (IList) property.Value;
            var template = Array.CreateInstance(property.ArrayElementType, list?.Count ?? 0);
            list?.CopyTo(template, 0);
            return template;
        }

        private bool TryGetDragAndDropObject(Object obj, out Object result)
        {
            if (obj == null)
            {
                result = null;
                return false;
            }

            var elementType = _property.ArrayElementType;
            var objType = obj.GetType();

            if (elementType == objType || elementType.IsAssignableFrom(objType))
            {
                result = obj;
                return true;
            }

            if (obj is GameObject go && typeof(Component).IsAssignableFrom(elementType) &&
                go.TryGetComponent(elementType, out var component))
            {
                result = component;
                return true;
            }

            result = null;
            return false;
        }

        private class ListPropertyOverrideContext : TriPropertyOverrideContext
        {
            public static readonly ListPropertyOverrideContext Instance = new ListPropertyOverrideContext();
            
            private readonly GUIContent _noneLabel = GUIContent.none;
            private static int Index = 0;

            public static void SetIndex(int index) => Index = index;

            public override bool TryGetDisplayName(TriProperty property, out GUIContent displayName)
            {
                var showLabels = property.TryGetAttribute(out ListDrawerSettingsAttribute settings) &&
                                 settings.ShowElementLabels;

                if (!showLabels)
                {
                    displayName = _noneLabel;
                    return true;
                }


                if (property.FieldType.GetCustomAttribute<ListElementLabelAttribute>() is ListElementLabelAttribute labelAttribute)
                {
                    string customText = labelAttribute.Text;
                    if (customText.Contains('%'))
                    {
                        customText = customText.Replace("%", Index.ToString());
                    }
                    displayName = new GUIContent(customText);
                    return true;
                }

                displayName = default;
                return false;
            }
        }
 
        private static class Styles
        {
            public static readonly GUIStyle ItemsCount;

            static Styles()
            {
                ItemsCount = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleRight,
                    normal =
                    {
                        textColor = EditorGUIUtility.isProSkin
                            ? new Color(0.6f, 0.6f, 0.6f)
                            : new Color(0.3f, 0.3f, 0.3f),
                    },
                };
            }
        }
    }
}