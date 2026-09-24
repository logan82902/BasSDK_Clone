using System;
using TriInspector;
using TriInspector.Drawers;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[assembly: RegisterTriValueDrawer(typeof(IntegerDrawer), TriDrawerOrder.Fallback)]
[assembly: RegisterTriValueDrawer(typeof(LongDrawer), TriDrawerOrder.Fallback)]
[assembly: RegisterTriValueDrawer(typeof(BooleanDrawer), TriDrawerOrder.Fallback)]
[assembly: RegisterTriValueDrawer(typeof(FloatDrawer), TriDrawerOrder.Fallback)]
[assembly: RegisterTriValueDrawer(typeof(StringDrawer), TriDrawerOrder.Fallback)]
[assembly: RegisterTriValueDrawer(typeof(ColorDrawer), TriDrawerOrder.Fallback)]
[assembly: RegisterTriValueDrawer(typeof(Color32Drawer), TriDrawerOrder.Fallback)]
[assembly: RegisterTriValueDrawer(typeof(LayerMaskDrawer), TriDrawerOrder.Fallback)]
[assembly: RegisterTriValueDrawer(typeof(EnumDrawer), TriDrawerOrder.Fallback)]
[assembly: RegisterTriValueDrawer(typeof(Vector2PropDrawer), TriDrawerOrder.Drawer)]
[assembly: RegisterTriValueDrawer(typeof(Vector3PropDrawer), TriDrawerOrder.Drawer)]
[assembly: RegisterTriValueDrawer(typeof(Vector4PropDrawer), TriDrawerOrder.Drawer)]
[assembly: RegisterTriValueDrawer(typeof(Vector2IntPropDrawer), TriDrawerOrder.Drawer)]
[assembly: RegisterTriValueDrawer(typeof(Vector3IntPropDrawer), TriDrawerOrder.Drawer)]
[assembly: RegisterTriValueDrawer(typeof(RectDrawer), TriDrawerOrder.Fallback)]
[assembly: RegisterTriValueDrawer(typeof(AnimationCurveDrawer), TriDrawerOrder.Fallback)]
[assembly: RegisterTriValueDrawer(typeof(BoundsDrawer), TriDrawerOrder.Fallback)]
[assembly: RegisterTriValueDrawer(typeof(GradientDrawer), TriDrawerOrder.Fallback)]
[assembly: RegisterTriValueDrawer(typeof(RectIntDrawer), TriDrawerOrder.Fallback)]
[assembly: RegisterTriValueDrawer(typeof(BoundsIntDrawer), TriDrawerOrder.Fallback)]

namespace TriInspector.Drawers
{
    public class StringDrawer : BuiltinDrawerBase<string>
    {
        protected override string OnValueGUI(Rect position, GUIContent label, string value)
        {
            return EditorGUI.TextField(position, label, value);
        }
    }

    public class BooleanDrawer : BuiltinDrawerBase<bool>
    {
        protected override bool OnValueGUI(Rect position, GUIContent label, bool value)
        {
            return EditorGUI.Toggle(position, label, value);
        }
    }

    public class IntegerDrawer : BuiltinDrawerBase<int>
    {
        protected override int OnValueGUI(Rect position, GUIContent label, int value)
        {
            return EditorGUI.IntField(position, label, value);
        }
    }

    public class LongDrawer : BuiltinDrawerBase<long>
    {
        protected override long OnValueGUI(Rect position, GUIContent label, long value)
        {
            return EditorGUI.LongField(position, label, value);
        }
    }

    public class FloatDrawer : BuiltinDrawerBase<float>
    {
        protected override float OnValueGUI(Rect position, GUIContent label, float value)
        {
            return EditorGUI.FloatField(position, label, value);
        }
    }

    public class ColorDrawer : BuiltinDrawerBase<Color>
    {
        protected override Color OnValueGUI(Rect position, GUIContent label, Color value)
        {
            return EditorGUI.ColorField(position, label, value);
        }
    }

    public class Color32Drawer : BuiltinDrawerBase<Color32>
    {
        protected override Color32 OnValueGUI(Rect position, GUIContent label, Color32 value)
        {
            return EditorGUI.ColorField(position, label, value);
        }
    }

    public class LayerMaskDrawer : BuiltinDrawerBase<LayerMask>
    {
        protected override LayerMask OnValueGUI(Rect position, GUIContent label, LayerMask value)
        {
            var mask = InternalEditorUtility.LayerMaskToConcatenatedLayersMask(value);
            var layers = InternalEditorUtility.layers;

            position = EditorGUI.PrefixLabel(position, label);
            return EditorGUI.MaskField(position, mask, layers);
        }
    }

    public class EnumDrawer : BuiltinDrawerBase<Enum>
    {
        protected override Enum OnValueGUI(Rect position, GUIContent label, Enum value)
        {
            if (value.GetType().IsDefined(typeof(FlagsAttribute), false))
            {
                return EditorGUI.EnumFlagsField(position, label, value);
            }
            return EditorGUI.EnumPopup(position, label, value);
        }
    }

    public class Vector2PropDrawer : BuiltinDrawerBase<Vector2>, IVectorDrawer<Vector2>
    {
        public int AxesCount => 2;

        public float[] ToFloats(Vector2 value) => new float[2] { value.x, value.y };

        public Vector2 ToVector(float[] axisValues) => new Vector2(axisValues[0], axisValues[1]);

        protected override Vector2 OnValueGUI(Rect position, GUIContent label, Vector2 value)
        {
            return ((IVectorDrawer<Vector2>)this).DrawGUI(position, label, value);
        }
    }

    public class Vector3PropDrawer : BuiltinDrawerBase<Vector3>, IVectorDrawer<Vector3>
    {
        public int AxesCount => 3;

        public float[] ToFloats(Vector3 value) => new float[3] { value.x, value.y, value.z };

        public Vector3 ToVector(float[] axisValues) => new Vector3(axisValues[0], axisValues[1], axisValues[2]);

        protected override Vector3 OnValueGUI(Rect position, GUIContent label, Vector3 value)
        {
            return ((IVectorDrawer<Vector3>) this).DrawGUI(position, label, value);
        }
    }

    public class Vector4PropDrawer : BuiltinDrawerBase<Vector4>, IVectorDrawer<Vector4>
    {
        public int AxesCount => 4;

        public float[] ToFloats(Vector4 value) => new float[4] { value.x, value.y, value.z, value.w };

        public Vector4 ToVector(float[] axisValues) => new Vector4(axisValues[0], axisValues[1], axisValues[2], axisValues[3]);

        protected override Vector4 OnValueGUI(Rect position, GUIContent label, Vector4 value)
        {
            return ((IVectorDrawer<Vector4>) this).DrawGUI(position, label, value);
        }
    }

    public class Vector2IntPropDrawer : BuiltinDrawerBase<Vector2Int>, IVectorDrawer<Vector2Int>
    {
        public bool IntegerVector => true;

        public int AxesCount => 2;

        public float[] ToFloats(Vector2Int value) => new float[2] { value.x, value.y };

        public Vector2Int ToVector(float[] axisValues) => new Vector2Int((int) axisValues[0], (int) axisValues[1]);

        protected override Vector2Int OnValueGUI(Rect position, GUIContent label, Vector2Int value)
        {
            return ((IVectorDrawer<Vector2Int>) this).DrawGUI(position, label, value);
        }
    }

    public class Vector3IntPropDrawer : BuiltinDrawerBase<Vector3Int>, IVectorDrawer<Vector3Int>
    {
        public bool IntegerVector => true;

        public int AxesCount => 3;

        public float[] ToFloats(Vector3Int value) => new float[3] { value.x, value.y, value.z };

        public Vector3Int ToVector(float[] axisValues) => new Vector3Int((int) axisValues[0], (int) axisValues[1], (int) axisValues[2]);

        protected override Vector3Int OnValueGUI(Rect position, GUIContent label, Vector3Int value)
        {
            return ((IVectorDrawer<Vector3Int>) this).DrawGUI(position, label, value);
        }
    }

    public class RectDrawer : BuiltinDrawerBase<Rect>
    {
        public override int CompactModeLines => 3;
        public override int WideModeLines => 2;

        protected override Rect OnValueGUI(Rect position, GUIContent label, Rect value)
        {
            return EditorGUI.RectField(position, label, value);
        }
    }

    public class AnimationCurveDrawer : BuiltinDrawerBase<AnimationCurve>
    {
        protected override AnimationCurve OnValueGUI(Rect position, GUIContent label, AnimationCurve value)
        {
            if (value == null) value = new AnimationCurve();
            return EditorGUI.CurveField(position, label, value);
        }
    }

    public class BoundsDrawer : BuiltinDrawerBase<Bounds>
    {
        public override int CompactModeLines => 3;
        public override int WideModeLines => 3;

        protected override Bounds OnValueGUI(Rect position, GUIContent label, Bounds value)
        {
            return EditorGUI.BoundsField(position, label, value);
        }
    }

    public class GradientDrawer : BuiltinDrawerBase<Gradient>
    {
        private static readonly GUIContent NullLabel = new GUIContent("Gradient is null");

        protected override Gradient OnValueGUI(Rect position, GUIContent label, Gradient value)
        {
            if (value == null)
            {
                EditorGUI.LabelField(position, label, NullLabel);
                return null;
            }

            return EditorGUI.GradientField(position, label, value);
        }
    }

    public class RectIntDrawer : BuiltinDrawerBase<RectInt>
    {
        public override int CompactModeLines => 3;
        public override int WideModeLines => 2;

        protected override RectInt OnValueGUI(Rect position, GUIContent label, RectInt value)
        {
            return EditorGUI.RectIntField(position, label, value);
        }
    }

    public class BoundsIntDrawer : BuiltinDrawerBase<BoundsInt>
    {
        public override int CompactModeLines => 3;
        public override int WideModeLines => 3;

        protected override BoundsInt OnValueGUI(Rect position, GUIContent label, BoundsInt value)
        {
            return EditorGUI.BoundsIntField(position, label, value);
        }
    }
}