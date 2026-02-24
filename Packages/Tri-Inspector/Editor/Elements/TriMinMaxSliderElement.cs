using System;
using System.Collections.Generic;
using System.Linq;
using TriInspectorUnityInternalBridge;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace TriInspector.Elements
{
    public class TriMinMaxSliderElement : TriElement
    {
        private readonly TriProperty _property;
        private readonly bool _showFields;
        private readonly Func<float> _minValueFunc;
        private readonly Func<float> _maxValueFunc;
        private readonly bool _isIntVector;

        public TriMinMaxSliderElement(TriProperty property, bool showFields, Func<float> minValueFunc, Func<float> maxValueFunc, bool isIntVector)
        {
            _property = property;
            _showFields = showFields;
            _minValueFunc = minValueFunc;
            _maxValueFunc = maxValueFunc;
            _isIntVector = isIntVector;
        }

        public override float GetHeight(float width)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position)
        {
            var controlId = GUIUtility.GetControlID(FocusType.Passive);
            position = EditorGUI.PrefixLabel(position, controlId, _property.DisplayNameContent);

            float minValue = 0f;
            float maxValue = 0f;

            if (_isIntVector)
            {
                Vector2Int vector = (Vector2Int)_property.Value;
                minValue = vector.x;
                maxValue = vector.y;
            }
            else
            {
                Vector2 vector = (Vector2)_property.Value;
                minValue = vector.x;
                maxValue = vector.y;
            }

            if (_showFields)
            {
                float fieldRectWidth = EditorGUIUtility.singleLineHeight * 3f;
                Rect minRect = new Rect() { x = position.x, y = position.y, width = fieldRectWidth, height = position.height };
                Rect slideRect = new Rect() { x = minRect.x + minRect.width + 5f, y = position.y, width = position.width - ((fieldRectWidth + 5f) * 2f), height = position.height };
                Rect maxRect = new Rect() { x = slideRect.x + slideRect.width + 5f, y = position.y, width = fieldRectWidth, height = position.height };

                // Draw UI
                if (_isIntVector)
                {
                    minValue = EditorGUI.IntField(minRect, (int) minValue);
                    maxValue = EditorGUI.IntField(maxRect, (int) maxValue);
                }
                else
                {
                    minValue = EditorGUI.FloatField(minRect, minValue);
                    maxValue = EditorGUI.FloatField(maxRect, maxValue);
                }
                EditorGUI.MinMaxSlider(slideRect, ref minValue, ref maxValue, _minValueFunc.Invoke(), _maxValueFunc.Invoke());
            }
            else
            {
                EditorGUI.MinMaxSlider(position, ref minValue, ref maxValue, _minValueFunc.Invoke(), _maxValueFunc.Invoke());
            }
            if (_isIntVector)
            {
                _property.SetValue(new Vector2Int((int) minValue, (int) maxValue));
            }
            else
            {
                _property.SetValue(new Vector2(minValue, maxValue));
            }
        }
    }
}
