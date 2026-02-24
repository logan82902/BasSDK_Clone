using System;
using System.Collections.Generic;
using System.Linq;
using TriInspectorUnityInternalBridge;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace TriInspector.Elements
{
    public class TriDropdownElement : TriElement
    {
        private readonly TriProperty _property;
        private readonly Func<TriProperty, IEnumerable<ITriDropdownItem>> _valuesGetter;
        private readonly bool _searchable;

        private object _currentValue;
        private string _currentText;

        private bool _hasNextValue;
        private object _nextValue;

        public TriDropdownElement(TriProperty property, Func<TriProperty, IEnumerable<ITriDropdownItem>> valuesGetter, bool searchable)
        {
            _property = property;
            _valuesGetter = valuesGetter;
            _searchable = searchable;
        }

        public override float GetHeight(float width)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position)
        {
            if (_hasNextValue)
            {
                var nextValue = _nextValue;
                _hasNextValue = false;
                _nextValue = null;

                _property.SetValue(nextValue);
                GUI.changed = true;
            }

            var values = _valuesGetter.Invoke(_property);
            if (_property.Value == null)
            {
                _property.SetValue(values.FirstOrDefault().Value);
            }

            if (!_property.Comparer.Equals(_currentValue, _property.Value))
            {
                _currentValue = _property.Value;
                _currentText = values
                    .FirstOrDefault(it => _property.Comparer.Equals(it.Value, _property.Value))
                    ?.Text ?? (_property.Value?.ToString() ?? string.Empty);
            }

            var controlId = GUIUtility.GetControlID(FocusType.Passive);
            position = EditorGUI.PrefixLabel(position, controlId, _property.DisplayNameContent);

            if (GUI.Button(position, _currentText, EditorStyles.popup))
            {
                ShowDropdown(position);
            }
        }

        private void ShowDropdown(Rect position)
        {
            var items = _valuesGetter.Invoke(_property);

            if (!_searchable)
            {
                var menu = new GenericMenu();

                foreach (var item in items)
                {
                    var isOn = _property.Comparer.Equals(item.Value, _property.Value);
                    menu.AddItem(new GUIContent(item.Text), isOn, ChangeValue, item.Value);
                }

                menu.DropDown(position);

                void ChangeValue(object v)
                {
                    _nextValue = v;
                    _hasNextValue = true;
                    _property.PropertyTree.RequestRepaint();
                }
            }
            else
            {
                var dropdown = new TriAdvancedDropdown(_property, new AdvancedDropdownState(), items);
                dropdown.Show(position);
                AdvancedDropdownProxy.SetShowHeader(dropdown, false);

                Event.current.Use();
            }
        }

        private class TriAdvancedDropdown : AdvancedDropdown
        {
            private readonly TriProperty _property;
            private readonly IEnumerable<ITriDropdownItem> _items;

            public TriAdvancedDropdown(TriProperty property, AdvancedDropdownState state, IEnumerable<ITriDropdownItem> items) : base(state)
            {
                _property = property;
                minimumSize = new Vector2(0, 120);
                _items = items;
            }

            protected override AdvancedDropdownItem BuildRoot()
            {
                var root = new AdvancedDropdownItem("Dropdown");

                foreach (var item in _items)
                {
                    root.AddChild(new TriAdvancedDropdownItem(item));
                }

                return root;
            }

            protected override void ItemSelected(AdvancedDropdownItem item)
            {
                if (!(item is TriAdvancedDropdownItem referenceTypeItem))
                {
                    return;
                }

                _property.SetValue(referenceTypeItem?.triDropdownItem?.Value);
            }

            private class TriAdvancedDropdownItem : AdvancedDropdownItem
            {
                public readonly ITriDropdownItem triDropdownItem;

                public TriAdvancedDropdownItem(ITriDropdownItem dropdownItem) : base(dropdownItem.Text)
                {
                    triDropdownItem = dropdownItem;
                    icon = null;
                }
            }
        }
    }
}