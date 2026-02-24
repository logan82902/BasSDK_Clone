#define SHADOWOODTRI
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using TriInspector;
using TriInspector.Drawers;
using TriInspector.Resolvers;
using UnityEditor;
using UnityEngine;

[assembly: RegisterTriAttributeDrawer(typeof(InlineButtonDrawer), TriDrawerOrder.Inspector)]

namespace TriInspector.Drawers
{
    public class InlineButtonDrawer : TriAttributeDrawer<InlineButtonAttribute>
    {
#if SHADOWOODTRI
        private ValueResolver<string> _textResolver;
#endif
        private DropdownValuesResolver<Action> _optionsResolver;

        public override TriExtensionInitializationResult Initialize(TriPropertyDefinition propertyDefinition)
        {
#if SHADOWOODTRI
            _textResolver = ValueResolver.ResolveString(propertyDefinition, Attribute.Text);

            if (_textResolver != null && _textResolver.TryGetErrorString(out var error))
            {
                return error;
            }
#endif

            return TriExtensionInitializationResult.Ok;
        }

        public override float GetHeight(float width, TriProperty property, TriElement next)
        {
            var height = next.GetHeight(width);

#if SHADOWOODTRI
            if (Attribute.NewLine)
            {
                var h = Attribute.ButtonHeight;
                if (h <= 0) h = EditorGUIUtility.singleLineHeight;
                height += h + EditorGUIUtility.standardVerticalSpacing;
            }
#endif

            return height;
        }

        public override void OnGUI(Rect position, TriProperty property, TriElement next)
        {
            if (Attribute.DropdownButton) InlineDropdownButton(position, property, next);
            else DefaultInlineButton(position, property, next);
        }

        private void DefaultInlineButton(Rect position, TriProperty property, TriElement next)
        {
            Rect fieldPropRect = new Rect(position);
            Rect buttonRect;
            object target = null;

#if SHADOWOODTRI
            var resolvedText = _textResolver?.GetValue(property);
            if (string.IsNullOrEmpty(resolvedText))
            {
                resolvedText = Regex.Replace(Attribute.Method, "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", " $1");
            }
#else
            var resolvedText = Attribute.Text;
            if (string.IsNullOrEmpty(resolvedText))
            {
                resolvedText = Regex.Replace(Attribute.Method, "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", " $1");
            }
#endif

            GUIContent buttonText = new GUIContent(resolvedText);

            MethodInfo FindMethodRecursive(TriProperty searchProp, string methodName, out object target)
            {
                target = searchProp.Value;
                if (target != null)
                {
                    const BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

                    foreach (var methodInfo in target.GetType().GetMethods(flags))
                    {
                        if (methodInfo.Name == methodName && methodInfo.GetParameters().Length == 0)
                        {
                            if (methodInfo.IsStatic)
                            {
                                target = null;
                            }

                            return methodInfo;
                        }
                    }
                }
                if (searchProp.Parent != null)
                {
                    return FindMethodRecursive(searchProp.Parent, methodName, out target);
                }
                else
                {
                    return null;
                }
            }
            MethodInfo method = null;
            try
            {
                method = FindMethodRecursive(property, Attribute.Method, out target);
            }
            catch (Exception e)
            {
                buttonText.text += " (MISSING OR WRONG SIG.)";
            }
            Vector2 buttonTextSize = GUI.skin.button.CalcSize(buttonText);

            var buttonWidth = Attribute.ButtonWidth;
            var buttonHeight = Attribute.ButtonHeight;

#if SHADOWOODTRI
            if (Attribute.NewLine)
            {
                if (Mathf.Approximately(buttonWidth, 0f))
                {
                    buttonWidth = position.width;
                }

                if (Mathf.Approximately(buttonHeight, 0f))
                {
                    buttonHeight = EditorGUIUtility.singleLineHeight;
                }

                buttonRect = new Rect(position)
                {
                    width = buttonWidth,
                    height = buttonHeight,
                };

                fieldPropRect.y += buttonHeight + EditorGUIUtility.standardVerticalSpacing;
                fieldPropRect.height -= buttonHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            else
#endif
            {
                if (Mathf.Approximately(buttonWidth, 0f))
                {
                    buttonWidth = buttonTextSize.x;
                }

                if (Mathf.Approximately(buttonHeight, 0f))
                {
                    buttonHeight = buttonTextSize.y;
                }

                fieldPropRect.width -= buttonWidth + 5f;

                buttonRect = new Rect(position) { x = fieldPropRect.x + fieldPropRect.width + 5f, width = buttonWidth, height = buttonHeight };

                if (Attribute.LeftSide)
                {
                    buttonRect.x = fieldPropRect.x;
                    fieldPropRect.x = fieldPropRect.x + buttonRect.width + 5f;
                }
            }

            next.OnGUI(fieldPropRect);

            bool orgEnabled = GUI.enabled;
            switch (Attribute.GUIEnabled)
            {
                case InlineButtonAttribute.GUIEnableState.Always:
                    GUI.enabled = true;
                    break;
                case InlineButtonAttribute.GUIEnableState.WithProperty:
                    break;
                case InlineButtonAttribute.GUIEnableState.OppositeProperty:
                    GUI.enabled = !orgEnabled;
                    break;
            }
            if (method == null) GUI.enabled = false;
            if (GUI.Button(buttonRect, buttonText) && GUI.enabled)
            {
                method.Invoke(target, null);
            }
            GUI.enabled = orgEnabled;
        }

        private void InlineDropdownButton(Rect position, TriProperty property, TriElement next)
        {
            _optionsResolver = DropdownValuesResolver<Action>.Resolve(property.Definition, Attribute.Method);
            if (_optionsResolver == null)
            {
                next.OnGUI(position);
                return;
            }
            Rect fieldPropRect = new Rect(position);
            Rect buttonRect;
#if SHADOWOODTRI
            var resolvedText = _textResolver?.GetValue(property) ?? Attribute.Text;
#else
            var resolvedText = Attribute.Text;
#endif
            if (string.IsNullOrEmpty(resolvedText))
            {
                resolvedText = Regex.Replace(Attribute.Method, "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", " $1");
            }
            GUIContent buttonText = new GUIContent(resolvedText);
            Vector2 buttonTextSize = GUI.skin.button.CalcSize(buttonText);

            var buttonWidth = Attribute.ButtonWidth;
            var buttonHeight = Attribute.ButtonHeight;

#if SHADOWOODTRI
            if (Attribute.NewLine)
            {
                if (Mathf.Approximately(buttonWidth, 0f))
                {
                    buttonWidth = position.width;
                }

                if (Mathf.Approximately(buttonHeight, 0f))
                {
                    buttonHeight = EditorGUIUtility.singleLineHeight;
                }

                buttonRect = new Rect(position)
                {
                    width = buttonWidth,
                    height = buttonHeight,
                };

                fieldPropRect.y += buttonHeight + EditorGUIUtility.standardVerticalSpacing;
                fieldPropRect.height -= buttonHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            else
#endif
            {
                if (Mathf.Approximately(buttonWidth, 0f))
                {
                    buttonWidth = buttonTextSize.x + 15f;
                }

                if (Mathf.Approximately(buttonHeight, 0f))
                {
                    buttonHeight = buttonTextSize.y;
                }

                fieldPropRect.width -= buttonWidth + 5f;

                buttonRect = new Rect(position) { x = fieldPropRect.x + fieldPropRect.width + 5f, width = buttonWidth, height = buttonHeight };

                if (Attribute.LeftSide)
                {
                    buttonRect.x = fieldPropRect.x;
                    fieldPropRect.x = fieldPropRect.x + buttonRect.width + 5f;
                }
            }

            next.OnGUI(fieldPropRect);

            bool orgEnabled = GUI.enabled;
            switch (Attribute.GUIEnabled)
            {
                case InlineButtonAttribute.GUIEnableState.Always:
                    GUI.enabled = true;
                    break;
                case InlineButtonAttribute.GUIEnableState.WithProperty:
                    break;
                case InlineButtonAttribute.GUIEnableState.OppositeProperty:
                    GUI.enabled = !orgEnabled;
                    break;
            }
            GenericMenu menu = new GenericMenu();
            foreach (ITriDropdownItem dropdownItem in _optionsResolver.GetDropdownItems(property))
            {
                menu.AddItem(new GUIContent(dropdownItem.Text), false, () => ((Action)dropdownItem.Value).Invoke());
            }
            if (GUI.Button(buttonRect, buttonText, EditorStyles.popup) && GUI.enabled)
            {
                menu.ShowAsContext();
            }
            GUI.enabled = orgEnabled;
        }
    }
}
