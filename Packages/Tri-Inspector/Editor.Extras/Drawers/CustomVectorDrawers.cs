using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriInspector.Drawers;
using UnityEditor;
using UnityEngine;

namespace TriInspector.Drawers
{
    public interface IVectorDrawer<T>
    {
        public bool IntegerVector => false;
        public int AxesCount { get; }

        public T DrawGUI(Rect position, GUIContent label, T value, SerializedProperty serializedProperty = null)
        {
            float[] axisValues = ToFloats(value);
            if (serializedProperty != null) EditorGUI.BeginProperty(position, GUIContent.none, serializedProperty);
            var controlId = GUIUtility.GetControlID(FocusType.Passive);
            position = EditorGUI.PrefixLabel(position, controlId, label);
            GUIStyle axisLabelStyle = new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleCenter };
            float axisLabelWidth = Mathf.Max(axisLabelStyle.CalcSize(new GUIContent("X")).x,
                axisLabelStyle.CalcSize(new GUIContent("Y")).x,
                axisLabelStyle.CalcSize(new GUIContent("Z")).x,
                axisLabelStyle.CalcSize(new GUIContent("W")).x) + 2f;
            float fieldWidth = (position.width - (axisLabelWidth * AxesCount)) / AxesCount;
            float xWidth = 0f;
            string[] axisLetters = new string[4] { "X", "Y", "Z", "W" };
            for (int i = 0; i < Mathf.Min(AxesCount, 4); i++)
            {
                EditorGUI.LabelField(new Rect(position.x + xWidth, position.y, axisLabelWidth, position.height), axisLetters[i], axisLabelStyle);
                xWidth += axisLabelWidth;
                Rect fieldRect = new Rect(position.x + xWidth, position.y, fieldWidth, position.height);
                axisValues[i] = IntegerVector ? EditorGUI.IntField(fieldRect, (int) axisValues[i]) : EditorGUI.FloatField(fieldRect, axisValues[i]);
                xWidth += fieldWidth;
            }
            if (serializedProperty != null) EditorGUI.EndProperty();
            return ToVector(axisValues);
        }

        public float[] ToFloats(T value);

        public T ToVector(float[] axisValues);
    }

    [CustomPropertyDrawer(typeof(Vector2))]
    public class Vector2Drawer : PropertyDrawer, IVectorDrawer<Vector2>
    {
        public int AxesCount => 2;

        public float[] ToFloats(Vector2 value) => new float[2] { value.x, value.y };

        public Vector2 ToVector(float[] axisValues) => new Vector2(axisValues[0], axisValues[1]);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Vector2 value = property.vector2Value;
            property.vector2Value = ((IVectorDrawer<Vector2>) this).DrawGUI(position, label, value, property);
        }
    }

    [CustomPropertyDrawer(typeof(Vector3))]
    public class Vector3Drawer : PropertyDrawer, IVectorDrawer<Vector3>
    {
        public int AxesCount => 3;

        public float[] ToFloats(Vector3 value) => new float[3] { value.x, value.y, value.z };

        public Vector3 ToVector(float[] axisValues) => new Vector3(axisValues[0], axisValues[1], axisValues[2]);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Vector3 value = property.vector3Value;
            property.vector3Value = ((IVectorDrawer<Vector3>) this).DrawGUI(position, label, value, property);
        }
    }

    [CustomPropertyDrawer(typeof(Vector4))]
    public class Vector4Drawer : PropertyDrawer, IVectorDrawer<Vector4>
    {
        public int AxesCount => 4;

        public float[] ToFloats(Vector4 value) => new float[4] { value.x, value.y, value.z, value.w };

        public Vector4 ToVector(float[] axisValues) => new Vector4(axisValues[0], axisValues[1], axisValues[2], axisValues[3]);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Vector4 value = property.vector4Value;
            property.vector4Value = ((IVectorDrawer<Vector4>) this).DrawGUI(position, label, value, property);
        }
    }

    [CustomPropertyDrawer(typeof(Vector2Int))]
    public class Vector2IntDrawer : PropertyDrawer, IVectorDrawer<Vector2Int>
    {
        public bool IntegerVector => true;

        public int AxesCount => 2;

        public float[] ToFloats(Vector2Int value) => new float[2] { value.x, value.y };

        public Vector2Int ToVector(float[] axisValues) => new Vector2Int((int) axisValues[0], (int) axisValues[1]);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Vector2Int value = property.vector2IntValue;
            property.vector2IntValue = ((IVectorDrawer<Vector2Int>) this).DrawGUI(position, label, value, property);
        }
    }

    [CustomPropertyDrawer(typeof(Vector3Int))]
    public class Vector3IntDrawer : PropertyDrawer, IVectorDrawer<Vector3Int>
    {
        public bool IntegerVector => true;

        public int AxesCount => 3;

        public float[] ToFloats(Vector3Int value) => new float[3] { value.x, value.y, value.z };

        public Vector3Int ToVector(float[] axisValues) => new Vector3Int((int) axisValues[0], (int) axisValues[1], (int) axisValues[2]);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Vector3Int value = property.vector3IntValue;
            property.vector3IntValue = ((IVectorDrawer<Vector3Int>) this).DrawGUI(position, label, value, property);
        }
    }
}
