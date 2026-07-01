using ThunderRoad;
using TriInspector;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RotationEventLinker : MonoBehaviour
{
    public Transform target;

    [Header("General")]
    public Axis axis = Axis.X;
    public bool invert;

    [Header("Min/Max angle")]
    public float positiveMinAngle = 5;
    public float positiveMaxAngle = 30;

    public float negativeMinAngle = 5;
    public float negativeMaxAngle = 30;

    [MinMaxSlider(0f, 1f, ShowFields = true)]
    public Vector2 positiveTriggerRange = new Vector2(0.9f, 1f);
    [MinMaxSlider(-1f, 0f, ShowFields = true)]
    public Vector2 negativeTriggerRange = new Vector2(-1f, -0.9f);

    [Header("Events")]
    public UnityEvent<float> output = new UnityEvent<float>();

    public UnityEvent positiveRangeEnter = new UnityEvent();
    public UnityEvent positiveRangeExit = new UnityEvent();

    public UnityEvent negativeRangeEnter = new UnityEvent();
    public UnityEvent negativeRangeExit = new UnityEvent();

#if UNITY_EDITOR
    [Header("Gizmo")]
    public Axis zeroAxis = Axis.Y;
    public bool invertZeroAxis = false;
#endif

    private int currentZone = 0;

    public enum Axis
    {
        X,
        Y,
        Z,
    }

    public void Invert(bool inverted)
    {
        invert = inverted;
    }

    private void Update()
    {
        float angle = 0;
        if (axis == Axis.X) angle = target.localRotation.eulerAngles.x;
        else if (axis == Axis.Y) angle = target.localRotation.eulerAngles.y;
        else if (axis == Axis.Z) angle = target.localRotation.eulerAngles.z;
        angle = (angle > 180) ? angle - 360 : angle;
        float ratio = Mathf.InverseLerp(angle >= 0 ? positiveMinAngle : negativeMinAngle, angle >= 0 ? positiveMaxAngle : negativeMaxAngle, Mathf.Abs(angle));
        if (angle < 0) ratio = -ratio;
        if (invert) ratio = -ratio;
        output?.Invoke(ratio);
        int updateZone = 0;
        if (positiveTriggerRange.IsInsideMinMax(ratio, false))
        {
            updateZone = 1;
        }
        if (negativeTriggerRange.IsInsideMinMax(ratio, false))
        {
            updateZone = -1;
        }
        if (updateZone != currentZone)
        {
            // if the current zone isn't 0, invoke the range exit event based on the value
            if (currentZone != 0) (currentZone == 1 ? positiveRangeExit : negativeRangeExit).Invoke();
            // if the update zone isn't 0, invoke the range enter event based on the current value
            if (updateZone != 0) (updateZone == 1 ? positiveRangeEnter : negativeRangeEnter).Invoke();
            currentZone = updateZone;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Selection.transforms.Length == 0) return;
        foreach (Transform selected in Selection.transforms)
        {
            if (selected == transform || selected == target) DrawGizmos();
        }
    }

    private void DrawGizmos()
    {
        float targetAngle = 0;
        if (this.axis == Axis.X) targetAngle = target.localRotation.eulerAngles.x;
        else if (this.axis == Axis.Y) targetAngle = target.localRotation.eulerAngles.y;
        else if (this.axis == Axis.Z) targetAngle = target.localRotation.eulerAngles.z;

        Vector3 axis = this.axis switch { Axis.X => transform.right, Axis.Y => transform.up, Axis.Z => transform.forward, _ => Vector3.zero };
        if (invert) axis = -axis;

        Vector3 zeroAxis = this.zeroAxis switch { Axis.X => transform.right, Axis.Y => transform.up, Axis.Z => transform.forward, _ => Vector3.zero };
        if (invertZeroAxis) zeroAxis = -zeroAxis;

        float gizmoSize = 0.05f;
        Vector3 GetAngleDirection(float angle) => Quaternion.AngleAxis(angle, axis) * zeroAxis;

        Handles.color = new Color(0f, 1f, 0f, 0.25f);
        Handles.DrawSolidArc(transform.position, axis, GetAngleDirection(positiveMinAngle), positiveMaxAngle - positiveMinAngle, gizmoSize);
        Handles.color = new Color(0f, 1f, 0f, 1f);
        Handles.DrawLine(transform.position, transform.position + (GetAngleDirection(Mathf.Lerp(positiveMinAngle, positiveMaxAngle, positiveTriggerRange.x)) * gizmoSize * 1.1f));
        Handles.DrawLine(transform.position, transform.position + (GetAngleDirection(Mathf.Lerp(positiveMinAngle, positiveMaxAngle, positiveTriggerRange.y)) * gizmoSize * 1.1f));

        Handles.color = new Color(1f, 0f, 0f, 0.25f);
        Handles.DrawSolidArc(transform.position, axis, GetAngleDirection(negativeMinAngle), -(negativeMaxAngle - negativeMinAngle), gizmoSize);
        Handles.color = new Color(1f, 0f, 0f, 1f);
        Handles.DrawLine(transform.position, transform.position + (GetAngleDirection(-Mathf.Lerp(negativeMinAngle, negativeMaxAngle, Mathf.Abs(negativeTriggerRange.x))) * gizmoSize * 1.1f));
        Handles.DrawLine(transform.position, transform.position + (GetAngleDirection(-Mathf.Lerp(negativeMinAngle, negativeMaxAngle, Mathf.Abs(negativeTriggerRange.y))) * gizmoSize * 1.1f));

        Handles.color = new Color(0f, 0f, 1f, 1f);
        Handles.DrawLine(transform.position, transform.position + (GetAngleDirection(targetAngle) * gizmoSize * 1.1f));
    }
#endif
}
