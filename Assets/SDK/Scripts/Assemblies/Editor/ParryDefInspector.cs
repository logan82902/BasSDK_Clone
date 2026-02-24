using UnityEditor;

namespace ThunderRoad
{
    [CustomEditor(typeof(ParryTarget))]
    public class ParryDefInspector : PointToPoint
    {
        ParryTarget parry;

        protected override void OnCreateGUIPreCoreElement()
        {
            script = parry = (ParryTarget)target;

            PointToPointButton(parry.GetLineStart(), parry.GetLineEnd());
        }

        protected override void OnCreateGUIPostCoreElement()
        {
            if (parry.length < 0)
            {
                EditorGUILayout.HelpBox("Parry Length can not be a negative number.", MessageType.Error);
            }
        }

        protected override void OnEnablePostCore()
        {
            script = parry = (ParryTarget)target;
            ResetPoints(parry.GetLineStart(), parry.GetLineEnd());
        }

        private void OnSceneGUI()
        {
            script = parry = (ParryTarget)target;
            UpdatePoints(ref parry.length, true);
        }
    }
}