using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using System;


using TriInspector;

namespace ThunderRoad
{
    [HelpURL("https://kospy.github.io/BasSDK/Components/ThunderRoad/Items/HandlePose.html")]
	public class HandlePose : MonoBehaviour
    {
        [Tooltip("References the handle this handpose is attached to.")]
        [Group("General")]
        [PropertyOrder(0)]
        public Handle handle;
        [Tooltip("Depicts which hand this handpose directs to.")]
        [Group("General")]
        [PropertyOrder(2)]
        public Side side = Side.Right;
        [Tooltip("ID of the handpose that is set to default if the target weight is zero.")]
        [Dropdown(nameof(GetAllHandPoseIds))]
        [Group("Pose")]
        [PropertyOrder(0)]
        public string defaultHandPoseId = "HandleDefault";

        [Tooltip("A per-HandlePose override for the Handle's SpellOrbTarget")]
        [Group("General")]
        [PropertyOrder(1)]
        public Transform spellOrbTarget;
        [NonSerialized]
        public HandPoseData defaultHandPoseData;
        protected HandPoseData.Pose defaultHandPose;

        [Range(0f, 1f)]
        [Tooltip("Blends the \"Default\" handpose and the \"Target\" handpose, allowing you to create more unique and fitting handposes without needing to create new ones.")]
        [Group("Pose")]
        [PropertyOrder(2)]
        public float targetWeight;
        [NonSerialized]
        public float lastTargetWeight = -1;

        [Tooltip("ID of the handpose that is used to blend against the default handpose. Handpose that is used if the target weight is one.")]
        [Dropdown(nameof(GetAllHandPoseIds))]
        [Group("Pose")]
        [PropertyOrder(1)]
        public string targetHandPoseId;
        [NonSerialized]
        public HandPoseData targetHandPoseData;
        protected HandPoseData.Pose targetHandPose;

        private void Awake()
        {
            if (!handle) handle = this.GetComponentInParent<Handle>();
        }

        private void Start()
        {
            LoadHandPosesData();
        }

        public void LoadHandPosesData()
        {
            defaultHandPoseData = Catalog.GetData<HandPoseData>(defaultHandPoseId) ?? 
                                  Catalog.GetData<HandPoseData>("HandleDefault");
            targetHandPoseData = Catalog.GetData<HandPoseData>(targetHandPoseId) ??
                                  Catalog.GetData<HandPoseData>("HandleDefault");
        }
 
        /// <summary>Returns a dropdown list of all creature IDs in the catalog.</summary>
        public TriDropdownList<string> GetAllCreatureIDs() => Catalog.GetDropdownAllID(Category.Creature);

        /// <summary>Returns a dropdown list of all hand pose IDs in the catalog.</summary>
        public TriDropdownList<string> GetAllHandPoseIds() => Catalog.GetDropdownAllID(Category.HandPose);

#if UNITY_EDITOR
 
        /// <summary>
        /// The type of preview the hand pose shows.
        /// <see cref="PreviewMode.GizmoMesh"/> shows a gizmo wire mesh, <see cref="PreviewMode.GizmoWire"/> shows the legacy wire gizmo and <see cref="PreviewMode.Mesh"/> shows a solid mesh.
        /// </summary>
        protected enum PreviewMode
        {
            GizmoMesh,
            GizmoWire,
            Mesh
        }

        /// <summary>The constant player pref id that saves the <see cref="PreviewMode"/> selection.</summary>
        public const string _HANDLEPOSE_PREVIEWMODE = "_HANDLEPOSE_PREVIEWMODE";

        /// <summary>Returns the saved <see cref="PreviewMode"/> selection from the PlayerPrefs</summary>
        [ShowInInspector]
        [Title("Editor Only", HorizontalLine = false)]
        [Group("Editor")]
        [PropertyOrder(0)]
        protected PreviewMode previewMode
        {
            get => (PreviewMode)PlayerPrefs.GetInt(_HANDLEPOSE_PREVIEWMODE);
            set => PlayerPrefs.SetInt(_HANDLEPOSE_PREVIEWMODE, (int)value);
        }

        [Group("Editor")]
        [PropertyOrder(2)]
        [Tooltip("Allows you to select a creature to test the handpose on.")]
        public Creature creature;
        [Dropdown(nameof(GetAllCreatureIDs))]
        [Tooltip("Uses the ID of the creature to ensure that the hand bones are correct.")]
        [Group("Editor")]
        [PropertyOrder(1)]
        public string creatureName = "HumanMale";

        /// <summary>The scriptable object that contains the creature to previews mapping.</summary>
        private HandPosePreviewSO handPosePreview;
        /// <summary>The preview instance that contains the hand mesh.</summary>
        private GameObject previewInstance;
        /// <summary>The <see cref="RagdollHand"/> on the preview instance.</summary>
        private RagdollHand previewRagdollHand;
        /// <summary>The <see cref="RagdollHandPoser"/> on the preview instance.</summary>
        private RagdollHandPoser previewPoser;
        /// <summary>The <see cref="SkinnedMeshRenderer"/> on the preview instance.</summary>
        private SkinnedMeshRenderer previewSmr;
        /// <summary>The cached baked mesh to be used in the gizmo draw.</summary>
        private Mesh cachedGizmoMesh;
        /// <summary>The previous <see cref="targetWeight"/> to check if we need to update the baked mesh.</summary>
        private float previousTargetWeight;
        /// <summary>Flag to see if we need to update the baked mesh.</summary>
        private bool hasDataChanged;
        /// <summary>Flag if the parent handle is selected.</summary>
        private bool isHandleSelected;
        /// <summary>Flag if this object is selected.</summary>
        private bool isSelfSelected;
        
#if UNITY_EDITOR
        public virtual void OnValidate()
        {
#if TESTINGLOCALLY
            return;
#endif
            if (!Application.isBatchMode)
            {
                UnityEditor.EditorApplication.delayCall += _OnValidate;
            }
        }

        private void OnDestroy()
        {
#if UNITY_EDITOR
            if (previewInstance != null)
            {
                DestroyImmediate(previewInstance);
                previewInstance = null;
            }

            UnityEditor.EditorApplication.delayCall -= _OnValidate;
#endif
        }

        private void _OnValidate()
        {
            UnityEditor.EditorApplication.delayCall -= _OnValidate;
            if(this == null) return;
            if (!gameObject.activeInHierarchy) return;
            if (!handle) handle = this.GetComponentInParent<Handle>();
            if (!Application.isPlaying)
            {
#if UNITY_EDITOR
                
                UpdateName();
                if (creature)
                {
                    Catalog.EditorLoadAllJson();
                    defaultHandPoseData = Catalog.GetData<HandPoseData>(defaultHandPoseId);
                    if (defaultHandPoseData != null) defaultHandPose = defaultHandPoseData.GetCreaturePose(creatureName);

                    targetHandPoseData = Catalog.GetData<HandPoseData>(targetHandPoseId);
                    if (targetHandPoseData != null) targetHandPose = targetHandPoseData.GetCreaturePose(creatureName);
                    foreach (RagdollHand hand in creature.GetComponentsInChildren<RagdollHand>())
                    {
                        if (hand.side == Side.Right) creature.handRight = hand;
                        if (hand.side == Side.Left) creature.handLeft = hand;
                    }
                    creature.GetHand(side).poser.defaultHandPoseData = defaultHandPoseData;
                    creature.GetHand(side).poser.targetHandPoseData = targetHandPoseData;
                    creature.GetHand(side).poser.globalRatio = true;
                    creature.GetHand(side).poser.EditorRefreshPose(creature);
                    creature.GetHand(side).poser.SetTargetWeight(targetWeight);
                }
#endif
            }
        }
#endif
        

        [Button]
        [Group("Editor")]
        [PropertyOrder(3)]
        public void EditorCreatureGrab()
        {
            if (creature)
            {
                Item item = handle.GetComponentInParent<Item>();
                Transform alignObject = handle.transform.root;
                if (item != null) alignObject = item.transform;

                Transform objectGrip = new GameObject("ObjectGrip").transform;
                objectGrip.SetParent(item ? item.transform : handle.transform.root);
                objectGrip.position = this.transform.position + (handle.transform.up * handle.GetDefaultAxisLocalPosition());
                objectGrip.rotation = this.transform.rotation;

                foreach (RagdollHand hand in creature.GetComponentsInChildren<RagdollHand>())
                {
                    if (hand.side == Side.Right) creature.handRight = hand;
                    if (hand.side == Side.Left) creature.handLeft = hand;
                }
                creature.GetHand(side).poser.SetGripFromPose(defaultHandPoseData);
                creature.GetHand(side).poser.targetWeight = targetWeight;
                creature.GetHand(side).poser.EditorRefreshPose(creature);

                alignObject.MoveAlign(objectGrip, creature.GetHand(side).grip.position, creature.GetHand(side).grip.rotation);

                DestroyImmediate(objectGrip.gameObject);
            }
        }

       /// <summary>Ensures a preview instance is present or creates a new one.</summary>
       protected void EnsurePreviewInstance()
       {
           HandPosePreviewSO preview = null;
           // find preview SO if not yet present or creature id changed
           if (handPosePreview == null || handPosePreview.creatureId != creatureName)
           {
               preview = HandPosePreviewSO.FindHandPosePreview(creatureName);
           }

           if (previewInstance != null)
           {
               if (preview == null && previewRagdollHand.side == side)
                   return;
               // destroy preview instance if creature id or side changed
               DestroyImmediate(previewInstance);
               previewInstance = null;
           }

           handPosePreview = preview;
           if (handPosePreview == null || handPosePreview.GetPrefab(side) == null)
               return;

           // instantiate preview prefab
           previewInstance = Instantiate(handPosePreview.GetPrefab(side));
           previewInstance.hideFlags = HideFlags.HideAndDontSave;
           previewInstance.SetActive(false);
           StageUtility.PlaceGameObjectInCurrentStage(previewInstance);

           previewRagdollHand = previewInstance.GetComponentInChildren<RagdollHand>(true);
           previewPoser = previewInstance.GetComponentInChildren<RagdollHandPoser>(true);
           previewSmr = previewInstance.GetComponentInChildren<SkinnedMeshRenderer>(true);
           hasDataChanged = true;

           if (previewRagdollHand == null || previewPoser == null || previewSmr == null)
           {
               Debug.LogError($"{previewInstance.name} is missing RagdollHand/RagdollHandPoser/SMR.");
               DestroyImmediate(previewInstance);
               previewInstance = null;
           }
       }

       /// <summary>Disables hand preview when  not selected.</summary>
       protected void OnDrawGizmos()
       {
           isSelfSelected = false;
           isHandleSelected = false;
           foreach (var selection in UnityEditor.Selection.gameObjects)
           {
               isSelfSelected |= selection == gameObject;
               isHandleSelected |= selection == handle.gameObject;
           }
           // only show the actual hand renderer if mesh preview mode is enabled and it is selected
           if (previewInstance != null)
               previewInstance.SetActive(isSelfSelected && previewMode == PreviewMode.Mesh);
       }

        protected void OnDrawGizmosSelected()
        {
            // only show gizmos if it or the handle is selected
            if (!isSelfSelected && !isHandleSelected)
                return;
            if (!Mathf.Approximately(previousTargetWeight, targetWeight))
                hasDataChanged = true;
            previousTargetWeight = targetWeight;

            if (!Catalog.IsJsonLoaded()) Catalog.EditorLoadAllJson(true, true, true);

            if (defaultHandPoseData == null || defaultHandPoseData.id != defaultHandPoseId)
            {
                defaultHandPoseData = Catalog.GetData<HandPoseData>(defaultHandPoseId);
                if (defaultHandPoseData != null) defaultHandPose = defaultHandPoseData.GetCreaturePose(creatureName);
                hasDataChanged = true;
            }

            if (targetHandPoseData == null || targetHandPoseData.id != targetHandPoseId)
            {
                targetHandPoseData = Catalog.GetData<HandPoseData>(targetHandPoseId);
                if (targetHandPoseData != null) targetHandPose = targetHandPoseData.GetCreaturePose(creatureName);
                hasDataChanged = true;
            }

            if (!handle) handle = GetComponentInParent<Handle>();
            Vector3 defaultAxisPosition = transform.position;
            Vector3 offset = -transform.position;
            if (handle)
            {
                defaultAxisPosition = handle.GetDefaultAxisPosition(side);
                offset = -handle.transform.position;
            }
            if (defaultHandPose != null && CheckQuaternion(defaultHandPose.GetFingers(side).gripLocalRotation))
            {
                Matrix4x4 gripMatrix = Matrix4x4.TRS(defaultAxisPosition + (transform.position + offset), this.transform.rotation * Quaternion.Inverse(defaultHandPose.GetFingers(side).gripLocalRotation), Vector3.one);
                gripMatrix *= Matrix4x4.TRS(-defaultHandPose.GetFingers(side).gripLocalPosition, Quaternion.identity, Vector3.one);

                if (previewMode != PreviewMode.GizmoWire)
                    EnsurePreviewInstance();
                // only draw the mesh (gizmos) if it is selected
                if (isSelfSelected && previewRagdollHand != null && previewPoser != null && previewSmr != null && previewMode != PreviewMode.GizmoWire)
                {
                    GizmoDrawMeshHand(gripMatrix);
                }
                else
                    GizmoDrawWireHand(gripMatrix, defaultAxisPosition, offset);
            }
        }

        /// <summary>Draws a gizmo mesh representation of a posed hand.</summary>
        protected void GizmoDrawMeshHand(Matrix4x4 gripMatrix)
        {
            previewRagdollHand.transform.SetPositionAndRotation(gripMatrix.MultiplyPoint3x4(Vector3.zero), gripMatrix.rotation);

            var defaultFingers = defaultHandPose.GetFingers(side);
            var targetFingers = targetHandPose.GetFingers(side);
            bool blend = targetWeight > 0 && targetFingers != null;
            previewPoser.UpdateFinger(previewRagdollHand.fingerThumb, defaultFingers.thumb, blend ? targetFingers.thumb : defaultFingers.thumb, blend ? targetWeight : 0);
            previewPoser.UpdateFinger(previewRagdollHand.fingerIndex, defaultFingers.index, blend ? targetFingers.index : defaultFingers.index, blend ? targetWeight : 0);
            previewPoser.UpdateFinger(previewRagdollHand.fingerMiddle, defaultFingers.middle, blend ? targetFingers.middle : defaultFingers.middle, blend ? targetWeight : 0);
            previewPoser.UpdateFinger(previewRagdollHand.fingerRing, defaultFingers.ring, blend ? targetFingers.ring : defaultFingers.ring, blend ? targetWeight : 0);
            previewPoser.UpdateFinger(previewRagdollHand.fingerLittle, defaultFingers.little, blend ? targetFingers.little : defaultFingers.little, blend ? targetWeight : 0);

            if (previewMode == PreviewMode.GizmoMesh)
            {
                // only bake the mesh if there was an actual change
                if (hasDataChanged)
                {
                    cachedGizmoMesh = new Mesh();
                    previewSmr.BakeMesh(cachedGizmoMesh);
                    hasDataChanged = false;
                }
                Gizmos.matrix = previewSmr.transform.localToWorldMatrix;
                Gizmos.color = Common.HueColourValue(HueColorName.Orange);
                Gizmos.DrawWireMesh(cachedGizmoMesh);
            }
        }

        /// <summary>Draws a gizmo wire representation of a posed hand.</summary>
        protected void GizmoDrawWireHand(Matrix4x4 gripMatrix, Vector3 defaultAxisPosition, Vector3 offset)
        {
            Gizmos.matrix = gripMatrix;
            Gizmos.color = Common.HueColourValue(HueColorName.Red);

            // Need to spin the transform so the transform.TransformPoint works correctly for left hand
            Quaternion originalRotation = transform.rotation;
            if (side == Side.Left) transform.rotation *= Quaternion.Euler(0f, 0f, 180f);
            Vector3 rootLocalPosition = gripMatrix.inverse.MultiplyPoint3x4(this.transform.TransformPoint(defaultHandPose.GetFingers(side).rootLocalPosition) + (defaultAxisPosition + offset) + (transform.position + offset));
            if (side == Side.Left) transform.rotation = originalRotation;

            if (targetWeight > 0 && targetHandPoseId != null && targetHandPoseId != "" && targetHandPose != null)
            {
                Gizmos.DrawLine(rootLocalPosition, Vector3.Lerp(defaultHandPose.GetFingers(side).thumb.proximal.localPosition, targetHandPose.GetFingers(side).thumb.proximal.localPosition, targetWeight));
                Gizmos.DrawLine(rootLocalPosition, Vector3.Lerp(defaultHandPose.GetFingers(side).index.proximal.localPosition, targetHandPose.GetFingers(side).index.proximal.localPosition, targetWeight));
                Gizmos.DrawLine(rootLocalPosition, Vector3.Lerp(defaultHandPose.GetFingers(side).middle.proximal.localPosition, targetHandPose.GetFingers(side).middle.proximal.localPosition, targetWeight));
                Gizmos.DrawLine(rootLocalPosition, Vector3.Lerp(defaultHandPose.GetFingers(side).ring.proximal.localPosition, targetHandPose.GetFingers(side).ring.proximal.localPosition, targetWeight));
                Gizmos.DrawLine(rootLocalPosition, Vector3.Lerp(defaultHandPose.GetFingers(side).little.proximal.localPosition, targetHandPose.GetFingers(side).little.proximal.localPosition, targetWeight));
                Gizmos.color = Common.HueColourValue(HueColorName.Orange);
                GizmoDrawFinger(defaultHandPose.GetFingers(side).thumb, targetHandPose.GetFingers(side).thumb, gripMatrix);
                GizmoDrawFinger(defaultHandPose.GetFingers(side).index, targetHandPose.GetFingers(side).index, gripMatrix);
                GizmoDrawFinger(defaultHandPose.GetFingers(side).middle, targetHandPose.GetFingers(side).middle, gripMatrix);
                GizmoDrawFinger(defaultHandPose.GetFingers(side).ring, targetHandPose.GetFingers(side).ring, gripMatrix);
                GizmoDrawFinger(defaultHandPose.GetFingers(side).little, targetHandPose.GetFingers(side).little, gripMatrix);
            }
            else
            {
                Gizmos.DrawLine(rootLocalPosition, defaultHandPose.GetFingers(side).thumb.proximal.localPosition);
                Gizmos.DrawLine(rootLocalPosition, defaultHandPose.GetFingers(side).index.proximal.localPosition);
                Gizmos.DrawLine(rootLocalPosition, defaultHandPose.GetFingers(side).middle.proximal.localPosition);
                Gizmos.DrawLine(rootLocalPosition, defaultHandPose.GetFingers(side).ring.proximal.localPosition);
                Gizmos.DrawLine(rootLocalPosition, defaultHandPose.GetFingers(side).little.proximal.localPosition);
                Gizmos.color = Common.HueColourValue(HueColorName.Orange);
                GizmoDrawFinger(defaultHandPose.GetFingers(side).thumb, null, gripMatrix);
                GizmoDrawFinger(defaultHandPose.GetFingers(side).index, null, gripMatrix);
                GizmoDrawFinger(defaultHandPose.GetFingers(side).middle, null, gripMatrix);
                GizmoDrawFinger(defaultHandPose.GetFingers(side).ring, null, gripMatrix);
                GizmoDrawFinger(defaultHandPose.GetFingers(side).little, null, gripMatrix);
            }
        }

#endif

        public void UpdateName()
        {
            string requiredName = side == Side.Right ? "OrientRight" : "OrientLeft";
            if (handle && (handle.orientationDefaultRight == this || handle.orientationDefaultLeft == this)) requiredName += "_Default";
            if (!name.Contains(requiredName)) name = requiredName;
        }

        protected void GizmoDrawFinger(HandPoseData.Pose.Finger finger, HandPoseData.Pose.Finger finger2, Matrix4x4 matrix)
        {
            Vector3 proximalLocalPosition = finger.proximal.localPosition;
            Quaternion proximalLocalRotation = finger.proximal.localRotation;

            Vector3 intermediateLocalPosition = finger.intermediate.localPosition;
            Quaternion intermediateLocalRotation = finger.intermediate.localRotation;

            Vector3 distalLocalPosition = finger.distal.localPosition;
            Quaternion distalLocalRotation = finger.distal.localRotation;

            Vector3 tipLocalPosition = finger.tipLocalPosition;

            if (finger2 != null)
            {
                proximalLocalPosition = Vector3.Lerp(finger.proximal.localPosition, finger2.proximal.localPosition, targetWeight);
                proximalLocalRotation = Quaternion.Lerp(finger.proximal.localRotation, finger2.proximal.localRotation, targetWeight);

                intermediateLocalPosition = Vector3.Lerp(finger.intermediate.localPosition, finger2.intermediate.localPosition, targetWeight);
                intermediateLocalRotation = Quaternion.Lerp(finger.intermediate.localRotation, finger2.intermediate.localRotation, targetWeight);

                distalLocalPosition = Vector3.Lerp(finger.distal.localPosition, finger2.distal.localPosition, targetWeight);
                distalLocalRotation = Quaternion.Lerp(finger.distal.localRotation, finger2.distal.localRotation, targetWeight);

                tipLocalPosition = Vector3.Lerp(finger.tipLocalPosition, finger2.tipLocalPosition, targetWeight);
            }

            Gizmos.matrix = matrix;
            Gizmos.DrawWireSphere(proximalLocalPosition, 0.006f);

            if (CheckQuaternion(proximalLocalRotation))
            {
                Gizmos.matrix *= Matrix4x4.TRS(proximalLocalPosition, proximalLocalRotation, Vector3.one);
                Gizmos.DrawWireSphere(intermediateLocalPosition, 0.005f);
                Gizmos.DrawLine(Vector3.zero, intermediateLocalPosition);
            }

            if (CheckQuaternion(intermediateLocalRotation))
            {
                Gizmos.matrix *= Matrix4x4.TRS(intermediateLocalPosition, intermediateLocalRotation, Vector3.one);
                Gizmos.DrawWireSphere(distalLocalPosition, 0.003f);
                Gizmos.DrawLine(Vector3.zero, distalLocalPosition);
            }

            if (CheckQuaternion(distalLocalRotation))
            {
                Gizmos.matrix *= Matrix4x4.TRS(distalLocalPosition, distalLocalRotation, Vector3.one);
                Gizmos.DrawWireSphere(tipLocalPosition, 0.002f);
                Gizmos.DrawLine(Vector3.zero, tipLocalPosition);
            }
        }

        protected bool CheckQuaternion(Quaternion quaternion)
        {
            if ((quaternion.x + quaternion.y + quaternion.z + quaternion.w) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}