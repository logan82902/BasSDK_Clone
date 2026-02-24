using System;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;

namespace TriInspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [Conditional("UNITY_EDITOR")]
    public class MinMaxSliderAttribute : Attribute
    {
        public float min;
        public float max;
        public string minProperty;
        public string maxProperty;
        public bool ShowFields { get; set; } = false;

        public MinMaxSliderAttribute(float min, float max, bool showFields = false)
        {
            this.min = min;
            this.max = max;
            if (showFields) ShowFields = true;
        }

        public MinMaxSliderAttribute(string minSource, float max, bool showFields = false)
        {
            this.minProperty = minSource;
            this.max = max;
            if (showFields) ShowFields = true;
        }

        public MinMaxSliderAttribute(float min, string maxSource, bool showFields = false)
        {
            this.min = min;
            this.maxProperty = maxSource;
            if (showFields) ShowFields = true;
        }

        public MinMaxSliderAttribute(string minSource, string maxSource, bool showFields = false)
        {
            this.minProperty = minSource;
            this.maxProperty = maxSource;
            if (showFields) ShowFields = true;
        }

        public MinMaxSliderAttribute(string minMaxSource, bool showFields = false)
        {
            this.minProperty = minMaxSource;
            this.maxProperty = minMaxSource;
            if (showFields) ShowFields = true;
        }

        public float GetMin(object source = null) => GetClamp(true, source);

        public float GetMax(object source = null) => GetClamp(false, source);

        private float GetClamp(bool min, object source = null)
        {
            float clamp = min ? this.min : this.max;
            string propertyName = min ? minProperty : maxProperty;
            if (propertyName != null && source != null)
            {
                MemberInfo member = source.GetType().GetMember(propertyName)[0];
                if (member is FieldInfo field)
                {
                    if (field.FieldType == typeof(Vector2))
                    {
                        Vector2 minMax = (Vector2)field.GetValue(source);
                        clamp = min ? minMax.x : minMax.y;
                    }
                    else if (field.FieldType == typeof(Vector2Int))
                    {
                        Vector2Int minMax = (Vector2Int)field.GetValue(source);
                        clamp = min ? minMax.x : minMax.y;
                    }
                    else
                    {
                        clamp = (float) field.GetValue(source);
                    }
                }
                if (member is PropertyInfo property)
                {
                    if (property.PropertyType == typeof(Vector2))
                    {
                        Vector2 minMax = (Vector2)property.GetValue(source);
                        clamp = min ? minMax.x : minMax.y;
                    }
                    else if (property.PropertyType == typeof(Vector2Int))
                    {
                        Vector2Int minMax = (Vector2Int)property.GetValue(source);
                        clamp = min ? minMax.x : minMax.y;
                    }
                    else
                    {
                        clamp = (float)property.GetValue(source);
                    }
                }
                if (member is MethodInfo method)
                {
                    if (method.ReturnType == typeof(Vector2))
                    {
                        Vector2 minMax = (Vector2)method.Invoke(source, new object[0] { });
                        clamp = min ? minMax.x : minMax.y;
                    }
                    if (method.ReturnType == typeof(Vector2Int))
                    {
                        Vector2Int minMax = (Vector2Int)method.Invoke(source, new object[0] { });
                        clamp = min ? minMax.x : minMax.y;
                    }
                    else
                    {
                        clamp = (float)method.Invoke(source, new object[0] { });
                    }
                }
            }
            return clamp;
        }
    }
}
