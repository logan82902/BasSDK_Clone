using System;
using UnityEngine;
using System.Collections.Generic;
using TriInspector;

namespace ThunderRoad
{
    [Serializable]
#if UNITY_EDITOR
    [EditorLoadPriority(-5)]
#endif
    [DeclareBoxGroup("HandPose", Title = "HandPose")]
    public class HandPoseData : CatalogData
    {
        [Group("HandPose")]
        [ListDrawerSettings(AlwaysExpanded = true)]
        [HideReferencePicker]
        public List<Pose> poses = new();

        public enum FingerType
        {
            Thumb,
            Index,
            Middle,
            Ring,
            Little
        }

        [Button]
        public void MirrorRightTipToLeft()
        {
            foreach (Pose pose in poses)
            {
                foreach (FingerType fingerType in Enum.GetValues(typeof(FingerType)))
                {
                    Pose.Finger rightFinger = pose.rightFingers.GetFinger(fingerType);
                    Pose.Finger leftFinger = pose.leftFingers.GetFinger(fingerType);
                    leftFinger.tipLocalPosition = new Vector3(rightFinger.tipLocalPosition.x, -rightFinger.tipLocalPosition.y, rightFinger.tipLocalPosition.z);
                }
            }
        }

        [Button]
        public void MirrorAllPoseTipLocals()
        {
            foreach (HandPoseData handPose in Catalog.GetDataList<HandPoseData>())
            {
                handPose.MirrorRightTipToLeft();
            }
        }

        [Serializable]
        [DeclareFoldoutGroup("Foldout", Expanded = false, Title = "Pose")]
        [DeclareHorizontalGroup("Foldout/Horiz")]
        [DeclareBoxGroup("Foldout/Horiz/Left", Title = "Left Fingers")]
        [DeclareBoxGroup("Foldout/Horiz/Right", Title = "Right Fingers")]
        public class Pose
        {
            [Dropdown(nameof(GetAllCreatureName))]
            [JsonMergeKey]
            public string creatureName;

            [Group("Foldout/Horiz/Left")]
            [InlineProperty]
            [HideLabel]
            public Fingers leftFingers = new();
            [Group("Foldout/Horiz/Right")]
            [InlineProperty]
            [HideLabel]
            public Fingers rightFingers = new();

            MirrorParams mirrorParams = new MirrorParams();

            [Serializable]
            public class MirrorParams
            {
                public bool invertQuaternionX = true;
                public bool invertQuaternionY;
                public bool invertQuaternionZ = true;
                public bool invertQuaternionW;
                public bool invertPositionX;
                public bool invertPositionY = true;
                public bool invertPositionZ;
                public bool gripReflectX;
                public bool gripReflectY;
                public bool gripReflectZ;
            }

            [Serializable]
            [DeclareHorizontalGroup("FingerHoriz")]
            [DeclareBoxGroup("FingerHoriz/Thumb", Title = "Thumb")]
            [DeclareBoxGroup("FingerHoriz/Index", Title = "Index")]
            [DeclareBoxGroup("FingerHoriz/Middle", Title = "Middle")]
            [DeclareBoxGroup("FingerHoriz/Ring", Title = "Ring")]
            [DeclareBoxGroup("FingerHoriz/Little", Title = "Little")]
            public class Fingers
            {
                public Vector3 gripLocalPosition;
                public Quaternion gripLocalRotation;
                public Vector3 rootLocalPosition;
                [Group("FingerHoriz/Thumb")]
                [InlineProperty]
                [HideLabel]
                public Finger thumb = new Finger();
                [Group("FingerHoriz/Index")]
                [InlineProperty]
                [HideLabel]
                public Finger index = new Finger();
                [Group("FingerHoriz/Middle")]
                [InlineProperty]
                [HideLabel]
                public Finger middle = new Finger();
                [Group("FingerHoriz/Ring")]
                [InlineProperty]
                [HideLabel]
                public Finger ring = new Finger();
                [Group("FingerHoriz/Little")]
                [InlineProperty]
                [HideLabel]
                public Finger little = new Finger();

                public Finger GetFinger(FingerType type)
                {
                    return type switch
                    {
                        FingerType.Thumb => thumb,
                        FingerType.Index => index,
                        FingerType.Middle => middle,
                        FingerType.Ring => ring,
                        FingerType.Little => little,
                        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                    };
                }
            }

            [Serializable]
            public class Finger
            {
                [Title("Proximal")]
                [InlineProperty]
                [HideLabel]
                public Bone proximal = new Bone();
                [Title("Intermediate")]
                [InlineProperty]
                [HideLabel]
                public Bone intermediate = new Bone();
                [Title("Distal")]
                [InlineProperty]
                [HideLabel]
                public Bone distal = new Bone();
                [Title("Tip Local Position")]
                [HideLabel]
                public Vector3 tipLocalPosition;

                [Serializable]
                public class Bone
                {
                    [HideLabel]
                    [Header("Local Position")]
                    public Vector3 localPosition;
                    [HideLabel]
                    [Header("Local Rotation")]
                    public Quaternion localRotation;
                }
            }

            public Pose(string creatureName)
            {
                this.creatureName = creatureName;
            }

            public void Save(Side side, MirrorParams mirrorParams)
            {
#if UNITY_EDITOR
                this.mirrorParams = mirrorParams;
                if (side == Side.Right)
                {
                    SaveFinger(leftFingers.thumb, rightFingers.thumb);
                    SaveFinger(leftFingers.index, rightFingers.index);
                    SaveFinger(leftFingers.middle, rightFingers.middle);
                    SaveFinger(leftFingers.ring, rightFingers.ring);
                    SaveFinger(leftFingers.little, rightFingers.little);
                    leftFingers.gripLocalPosition = MirrorPosition(rightFingers.gripLocalPosition);
                    leftFingers.gripLocalRotation = MirrorRotation(rightFingers.gripLocalRotation);
                    leftFingers.rootLocalPosition = MirrorPosition(rightFingers.rootLocalPosition);
                }
                else if (side == Side.Left)
                {
                    SaveFinger(rightFingers.thumb, leftFingers.thumb);
                    SaveFinger(rightFingers.index, leftFingers.index);
                    SaveFinger(rightFingers.middle, leftFingers.middle);
                    SaveFinger(rightFingers.ring, leftFingers.ring);
                    SaveFinger(rightFingers.little, leftFingers.little);
                    rightFingers.gripLocalPosition = MirrorPosition(leftFingers.gripLocalPosition);
                    rightFingers.gripLocalRotation = MirrorRotation(leftFingers.gripLocalRotation);
                    rightFingers.rootLocalPosition = MirrorPosition(leftFingers.rootLocalPosition);
                }
#endif
            }

            protected void SaveFinger(Finger finger1, Finger finger2)
            {
                finger1.proximal.localPosition = MirrorPosition(finger2.proximal.localPosition);
                finger1.proximal.localRotation = InverseRotation(finger2.proximal.localRotation);
                finger1.intermediate.localPosition = MirrorPosition(finger2.intermediate.localPosition);
                finger1.intermediate.localRotation = InverseRotation(finger2.intermediate.localRotation);
                finger1.distal.localPosition = MirrorPosition(finger2.distal.localPosition);
                finger1.distal.localRotation = InverseRotation(finger2.distal.localRotation);
                finger1.tipLocalPosition = MirrorPosition(finger2.tipLocalPosition);
            }

            public Fingers GetFingers(Side side)
            {
                return side == Side.Right ? rightFingers : leftFingers;
            }

            private Quaternion ReflectRotation(Quaternion source, Vector3 normal)
            {
                return Quaternion.LookRotation(Vector3.Reflect(source * Vector3.forward, normal), Vector3.Reflect(source * Vector3.up, normal));
            }

            protected Vector3 MirrorPosition(Vector3 position)
            {
                if (mirrorParams.invertPositionX) position.x *= -1;
                if (mirrorParams.invertPositionY) position.y *= -1;
                if (mirrorParams.invertPositionZ) position.z *= -1;
                return position;
            }

            protected Quaternion MirrorRotation(Quaternion rotation)
            {
                if (mirrorParams.gripReflectX) rotation = ReflectRotation(rotation, Vector3.right);
                if (mirrorParams.gripReflectY) rotation = ReflectRotation(rotation, Vector3.up);
                if (mirrorParams.gripReflectZ) rotation = ReflectRotation(rotation, Vector3.forward);
                return rotation;
            }

            protected Quaternion InverseRotation(Quaternion rotation)
            {
                if (mirrorParams.invertQuaternionX) rotation.x *= -1;
                if (mirrorParams.invertQuaternionY) rotation.y *= -1;
                if (mirrorParams.invertQuaternionZ) rotation.z *= -1;
                if (mirrorParams.invertQuaternionW) rotation.w *= -1;
                return rotation;
            }

            public TriDropdownList<string> GetAllCreatureName()
            {
                TriDropdownList<string> dropdownList = new();
                foreach (CreatureData creatureData in Catalog.GetDataList(Category.Creature))
                {
                    if (dropdownList.Exists(d => d.Value == creatureData.name))
                    {
                        continue;
                    }
                    dropdownList.Add(creatureData.name, creatureData.name);
                }
                return dropdownList;
            }
        }

        public override int GetCurrentVersion()
        {
            return 2;
        }

        public Pose GetCreaturePose(Creature creature)
        {
            Pose creaturePose = GetCreaturePose(creature.data.name);
            return creaturePose;
        }

        public Pose GetCreaturePose(string creatureName)
        {
            int posesCount = poses.Count;
            for (var i = 0; i < posesCount; i++)
            {
                Pose pose = poses[i];
                if (creatureName == pose.creatureName)
                {
                    return pose;
                }
            }
            #if UNITY_EDITOR
            Debug.LogWarning("No pose found for creature " + creatureName + " in hand pose data " + id);
            #endif
            return null;
        }

        public Pose AddCreaturePose(Creature creature)
        {
            return AddCreaturePose(creature.data.name);
        }

        public Pose AddCreaturePose(string creatureName)
        {
            Pose newCreaturePose = new Pose(creatureName);
            poses.Add(newCreaturePose);
            return newCreaturePose;
        }
    }
}
