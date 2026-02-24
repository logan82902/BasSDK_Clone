using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ThunderRoad.Skill.SpellPower;
using UnityEngine;
using UnityEngine.Profiling;
using Random = UnityEngine.Random;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

#if UNITY_EDITOR
using UnityEditor;
#endif
using TriInspector;
using UnityEngine.UIElements;

namespace ThunderRoad
{
    [HelpURL("https://kospy.github.io/BasSDK/Components/ThunderRoad/Items/Item.html")]
    [AddComponentMenu("ThunderRoad/Items/Item")]
    [DeclareTabGroup("Tabs")]
    [DeclareBoxGroup("Tabs/General", Title = "General")]
    [DeclareHorizontalGroup("Tabs/General/Handles")]
    [DeclareHorizontalGroup("Tabs/General/WorldParenting")]
    [DeclareBoxGroup("Tabs/Physics", Title = "Physics")]
    [DeclareHorizontalGroup("Tabs/Physics/CustomCoM", Sizes = new float[] { 190f, 0f, 15f })]
    [DeclareHorizontalGroup("Tabs/Physics/CustomInertia", Sizes = new float[] { 190f, 0f })]
    [DeclareBoxGroup("Tabs/Storage", Title = "Storage")]
    [DeclareBoxGroup("Tabs/Handling", Title = "Handling")]
    [DeclareHorizontalGroup("Tabs/Handling/FlyBools")]
    [DeclareHorizontalGroup("Tabs/Handling/FlySettings")]
    public class Item : ThunderEntity
    {
        public static List<Item> all = new List<Item>();
        public static List<Item> allActive = new List<Item>();
        public static List<Item> allThrowed = new List<Item>();
        public static HashSet<Item> allMoving = new HashSet<Item>();
        public static List<Item> allTk = new List<Item>();
        public static List<Item> allWorldAttached = new List<Item>();
        public static List<ItemContent> potentialLostItems = new List<ItemContent>(); //items that were destoyed but were not despawned to be returned to player stash if needed

        public TriDropdownList<string> GetAllItemID() => Catalog.GetDropdownAllID(Category.Item);

        [Dropdown(nameof(GetAllItemID), AppendToField = true)]
        [Tooltip("The Item ID of the item specified in the Catalog")]
        [Group("Tabs/General"), Tab("General")]
        public string itemId;
        [Tooltip("Specifies the spawn point of the item. This specifies the position and rotation of the item when spawned, it's mostly used for itemSpawner and spawning the item via an item book")]
        [Group("Tabs/General"), Tab("General")]
        public Transform spawnPoint;
        [Tooltip("Shows the point that AI will try to block with when they are holding the item.")]
        [Group("Tabs/General"), Tab("General")]
        public Transform parryPoint;
        [Tooltip("Specifies what handle is grabbed by default for the Left Hand")]
        [Group("Tabs/General/Handles"), Tab("General")]
        [AutoLabelWidth(AddedWidth = 2f)]
        public Handle mainHandleLeft;
        [Tooltip("Specifies what handle is grabbed by default for the Right Hand")]
        [Group("Tabs/General/Handles"), Tab("General")]
        [AutoLabelWidth(AddedWidth = 2f)]
        public Handle mainHandleRight;
        [Tooltip("States the Preview for the item.")]
        [Group("Tabs/General"), Tab("General")]
        public Preview preview;
        [Group("Tabs/General"), Tab("General")]
        public AudioSource audioSource;
        [Group("Tabs/General"), Tab("General")]
        public float snapPitchRange = 0.05f;
        [Tooltip("If ticked, this item won't despawn. Tick at your own risk!")]
        [Group("Tabs/General/WorldParenting"), Tab("General")]
        [AutoLabelWidth(AddedWidth = 2f)]
        public bool DisallowDespawn;
        [Tooltip("Tick if the item is attached to the world and not spawned via item spawner or item book.")]
        [Group("Tabs/General/WorldParenting"), Tab("General")]
        [AutoLabelWidth(AddedWidth = 2f)]
        public bool worldAttached;
        [Tooltip("Tick if the item should keep its parent when it loads into the scene.")]
        [Group("Tabs/General/WorldParenting"), Tab("General")]
        [AutoLabelWidth(AddedWidth = 2f)]
        public bool keepParent;
        [Tooltip("Forces layer of mesh when an item is spawned.\n\n(Items will have their layer automatically applied when spawned, unless this is set)")]
        [Group("Tabs/General"), Tab("General")]
        public int forceMeshLayer = -1;
        [Tooltip("Allows a custom reference to be able to reference specific gameobjects and scripts in External code.")]
        [Group("Tabs/General"), Tab("General")]
        public List<CustomReference> customReferences = new();

        [Tooltip("Specifies the Holder Point of the item. This specifies the position and rotation of the item when held in a holder, such as on player hips and back. The Z axis/blue arrow specifies towards the floor.")]
        [InlineButton(nameof(SetHolderPointToCenterOfMass), "To COM")]
        [Group("Tabs/Storage"), Tab("General")]
        public Transform holderPoint;
        [Tooltip("Can add additional holderpoints for different interactables.\n \nFor Items on the Item Rack, the anchor must be named HolderRackTopAnchor, or alternatively for the bow rack, HolderRackTopAnchorBow, and HolderRackSideAnchor for Shield rack")]
        [Group("Tabs/Storage"), Tab("General")]
        public List<HolderPoint> additionalHolderPoints = new();
        [Header("Holder")]
        [Group("Tabs/Storage"), Tab("General")]
        public List<ItemData.CustomSnap> customSnaps = new();
        [Group("Tabs/Storage"), Tab("General")]
        public AudioContainer audioContainerSnap;
        [Group("Tabs/Storage"), Tab("General")]
        public AudioContainer audioContainerInventory;

        [Tooltip("Radius to depict how close this item needs to be to a creature before the creatures' collision is enabled.")]
        [Group("Tabs/Physics"), Tab("General")]
        public float creaturePhysicToggleRadius = 2;
        [Tooltip("Allows user to adjust the center of mass on an object.\nIf unticked, this is automatically adjusted. When ticked, adds a custom gizmo to adjust.\n \nUse this if weight on the item is acting strange.")]
        [Group("Tabs/Physics/CustomCoM"), Tab("General")]
        [LabelWidth(175f)]
        public bool useCustomCenterOfMass;
        [Tooltip("Position of Center of Mass (if ticked)")]
        [Group("Tabs/Physics/CustomCoM"), Tab("General")]
        [EnableIf(nameof(useCustomCenterOfMass))]
        [HideLabel]
#if UNITY_EDITOR
        [InlineButton(nameof(ToggleCenterOfMassHandle))]
#endif
        public Vector3 customCenterOfMass;
#if UNITY_EDITOR
        [NonSerialized]
        [ShowInInspector]
        [Group("Tabs/Physics/CustomCoM"), Tab("General")]
        [HideLabel]
        [ReadOnly]
        public bool showCoMHandle = false;

        public void ToggleCenterOfMassHandle() => showCoMHandle = !showCoMHandle;
#endif
        [Tooltip("Used for balance adjustment on a weapon.\n \nUse this if swinging weapons are strange. Adjust the Capsule collider to the width of the weapon.")]
        [Group("Tabs/Physics/CustomInertia"), Tab("General")]
        [LabelWidth(175f)]
        public bool customInertiaTensor;
        [Tooltip("Collider of the Custom Inertia Tensor")]
        [Group("Tabs/Physics/CustomInertia"), Tab("General")]
        [EnableIf(nameof(customInertiaTensor))]
        [HideLabel]
        public CapsuleCollider customInertiaTensorCollider;
        [Tooltip("Oriented bounding box")]
        [Group("Tabs/Physics"), Tab("General")]
        public Transform obbPoint;
        [Tooltip("World Oriented transforms for a bounding box around the item.")]
        [Group("Tabs/Physics"), Tab("General")]
        [SerializeField]
        public Transform[] orientedBoundsPoints;

        [Title("Throw/Fly")]
        [Tooltip("Used to point in direction when thrown.\nZ-Axis/Blue Arrow points forwards.")]
        [Group("Tabs/Handling"), Tab("General")]
        [AutoLabelWidth]
        public Transform flyDirRef;
        [Tooltip("When ticked, item is automatically set as \"Thrown\" when spawned.")]
        [Group("Tabs/Handling/FlyBools"), Tab("General")]
        public bool forceThrown;
        [Tooltip("When ticked, item is automatically set as \"Thrown\" when spawned.")]
        [Group("Tabs/Handling/FlyBools"), Tab("General")]
        public bool flyFromThrow;
        [Tooltip("Speed of which the item rotates when thrown")]
        [Group("Tabs/Handling/FlySettings"), Tab("General")]
        public float flyRotationSpeed = 2;
        [Tooltip("Angle offset of the z-axis arrow when thrown.")]
        [Group("Tabs/Handling/FlySettings"), Tab("General")]
        public float flyThrowAngle = 0;
        [Title("Telekinesis")]
        [Group("Tabs/Handling"), Tab("General")]
        public float distantGrabSafeDistance = 1;
        [Group("Tabs/Handling"), Tab("General")]
        public bool distantGrabSpinEnabled = true;
        [Group("Tabs/Handling"), Tab("General")]
        public float distantGrabThrowRatio = 1;


        public Owner owner => _owner;
        [Group("Tabs"), Tab("Runtime")]
        [ShowInInspector]
        private Owner _owner;

        [Group("Tabs"), Tab("Runtime")]
        [NonSerialized]
        [ShowInInspector]
        public bool isUsed = true;

        public enum Owner
        {
            None,
            Player,
            Shopkeeper,
        }
        
        [NonSerialized]
        public List<Renderer> renderers = new List<Renderer>();
        [NonSerialized]
        public List<FxController> fxControllers = new List<FxController>();
        [NonSerialized]
        public List<FxModule> fxModules = new List<FxModule>();

        [NonSerialized]
        public List<RevealDecal> revealDecals = new List<RevealDecal>();
        [NonSerialized]
        public List<ColliderGroup> colliderGroups = new List<ColliderGroup>();
        [NonSerialized]
        public List<Collider> disabledColliders = new List<Collider>();
        [NonSerialized]
        public List<HingeEffect> effectHinges = new List<HingeEffect>();
        [NonSerialized]
        public List<WhooshPoint> whooshPoints = new List<WhooshPoint>();
        [HideInInspector]
        public LightVolumeReceiver lightVolumeReceiver;
        [NonSerialized]
        public List<CollisionHandler> collisionHandlers = new List<CollisionHandler>();
        [NonSerialized]
        public List<Handle> handles = new List<Handle>();
        [NonSerialized]
        [ShowInInspector]
        [Group("Tabs"), Tab("Runtime")]
        [ReadOnly]
        public PhysicBody physicBody;
        [NonSerialized]
        public Breakable breakable;
        [NonSerialized]
        public List<ParryTarget> parryTargets = new List<ParryTarget>();
        [NonSerialized]
        public Holder holder;
        [NonSerialized]
        ClothingGenderSwitcher clothingGenderSwitcher;

        [NonSerialized]
        public bool allowGrip = true;
        [NonSerialized]
        public bool hasSlash;
        [NonSerialized]
        public bool hasMetal;
        [NonSerialized]
        public List<ColliderGroup> metalColliderGroups;
        [NonSerialized]
        public Vector3 centreOfBuoyancy;
        [NonSerialized]
        public float submergedVolume;
        [NonSerialized]
        public float submergedPhysicVolume;
        
        [NonSerialized]
        public FloatHandler sliceAngleMultiplier;
        [NonSerialized]
        public FloatHandler damageMultiplier;
        [NonSerialized]
        public IntAddHandler pushLevelMultiplier;

        [ShowInInspector]
        [Group("Tabs"), Tab("Runtime")]
        [NonSerialized]
        public Container linkedContainer;
        [ShowInInspector]
        [Group("Tabs"), Tab("Runtime")]
        [NonSerialized]
        public List<ContentCustomData> contentCustomData;

        [Tooltip("The real geometric volume of the item used for water submersion calculations. Calculated from the prefabs Oriented Bounding Box")]
        [ReadOnly]
        public float itemVolume = 0;


        [Serializable]
        public class IconMarker
        {
            public string damagerId;
            public Vector2 position;
            public float directionAngle;
            public Damager.Direction direction;
            public IconMarker(string damagerId, Vector2 position, Damager.Direction direction, float directionAngle)
            {
                this.damagerId = damagerId;
                this.position = position;
                this.direction = direction;
                this.directionAngle = directionAngle;
            }
        }

        public void SetHolderPointToCenterOfMass()
        {
            holderPoint.transform.position = this.transform.TransformPoint(physicBody ? physicBody.centerOfMass : this.GetComponent<Rigidbody>().centerOfMass);
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }


        public void AddNonStorableModifierInvokable(UnityEngine.Object handler)
        {
        }

        public void RemoveNonStorableModifierInvokable(UnityEngine.Object handler)
        {
        }

        public void ClearNonStorableModifiers()
        {
        }

        protected virtual void OnValidate()
        {
            if (Application.isPlaying)
                return;
#if UNITY_EDITOR
            if (UnityEditor.BuildPipeline.isBuildingPlayer)
                return;

            if (!this.InPrefabScene())
                return;
#endif      
            if (!gameObject.activeInHierarchy)
                return;
            
            SetupDefaultComponents();
            
        }
        
#if UNITY_EDITOR
        private float CalculateOBBVolume(Transform[] orientedBoundsPoints)
        {
            if (orientedBoundsPoints == null || orientedBoundsPoints.Length != 8)
                return 0f;

            // Get the 8 corner positions
            Vector3[] pts = new Vector3[8];
            for (int i = 0; i < 8; i++)
                pts[i] = orientedBoundsPoints[i].position;

            // Calculate the three edge vectors from point 0
            // Bottom face: 0->1, 0->3, 0->4
            Vector3 width = pts[1] - pts[0]; // X-axis
            Vector3 depth = pts[3] - pts[0]; // Z-axis  
            Vector3 height = pts[4] - pts[0]; // Y-axis

            // Volume = |width| * |depth| * |height| for axis-aligned
            // For oriented box: use scalar triple product to handle rotation
            float volume = Mathf.Abs(Vector3.Dot(Vector3.Cross(width, depth), height));

            return volume;
        }
        
        [Button]
        public void SetupBoundsPoints()
        {
            var root = GetItemRoot();
            if (root == null)
            {
                Debug.LogError("Root object not found.");
                return;
            }

            // Get all colliders in children (excluding inertia tensor colliders)
            Collider[] colliders = root.GetComponentsInChildren<Collider>(true);
            List<Collider> inertiaTensorColliders = new List<Collider>();
            Item[] items = root.GetComponentsInChildren<Item>(true);
            foreach (var item in items)
            {
                if (item.customInertiaTensor && item.customInertiaTensorCollider != null)
                    inertiaTensorColliders.Add(item.customInertiaTensorCollider);
            }

            if (colliders.Length == 0)
            {
                Debug.LogWarning($"[DefaultComponents] No colliders found under {root.name}");
                return;
            }

            // --- Step 1: Compute combined bounds in LOCAL SPACE ---
            bool started = false;
            Bounds localBounds = new Bounds();
            Matrix4x4 worldToLocal = root.transform.worldToLocalMatrix;

            foreach (var col in colliders)
            {
                if (inertiaTensorColliders.Contains(col))
                    continue;

                Bounds worldBounds = col.bounds;
                Vector3 min = worldBounds.min;
                Vector3 max = worldBounds.max;

                // Convert all 8 corners to local space and encapsulate
                for (int x = 0; x <= 1; x++)
                for (int y = 0; y <= 1; y++)
                for (int z = 0; z <= 1; z++)
                {
                    Vector3 corner = new Vector3(
                        x == 0 ? min.x : max.x,
                        y == 0 ? min.y : max.y,
                        z == 0 ? min.z : max.z
                    );
                    Vector3 localCorner = worldToLocal.MultiplyPoint3x4(corner);
                    if (!started)
                    {
                        localBounds = new Bounds(localCorner, Vector3.zero);
                        started = true;
                    }
                    else
                    {
                        localBounds.Encapsulate(localCorner);
                    }
                }
            }

            // --- Step 2: Ensure there’s an OBB holder GameObject ---
            if (!obbPoint)
            {
                obbPoint = root.Find("obb");
                if (!obbPoint)
                {
                    Debug.LogWarning($"[DefaultComponents] Adding obbPoint to {this.itemId}");
                    obbPoint = new GameObject("obb").transform;
                    obbPoint.SetParent(root, false);
                }
            }
            
            // Reset transform so it inherits root orientation properly
            obbPoint.localPosition = Vector3.zero;
            obbPoint.localRotation = Quaternion.identity;
            obbPoint.localScale = Vector3.one;

            // Step 4: Create or update point transforms
            if (orientedBoundsPoints == null || orientedBoundsPoints.Length != 8)
                orientedBoundsPoints = new Transform[8];
            Vector3 localMin = localBounds.min;
            Vector3 localMax = localBounds.max;
            
            for (int i = 0; i < 8; i++)
            {
                Transform point = orientedBoundsPoints[i];
                if (point == null)
                {
                    GameObject p = new GameObject($"obb{i+1}");
                    point = p.transform;
                    point.SetParent(obbPoint, false);
                    orientedBoundsPoints[i] = point;
                }

                // Step 5: Assign local positions (not world!)
                point.localPosition = i switch
                {
                    0 => new Vector3(localMin.x, localMin.y, localMin.z),
                    1 => new Vector3(localMax.x, localMin.y, localMin.z),
                    2 => new Vector3(localMax.x, localMin.y, localMax.z),
                    3 => new Vector3(localMin.x, localMin.y, localMax.z),
                    4 => new Vector3(localMin.x, localMax.y, localMin.z),
                    5 => new Vector3(localMax.x, localMax.y, localMin.z),
                    6 => new Vector3(localMax.x, localMax.y, localMax.z),
                    7 => new Vector3(localMin.x, localMax.y, localMax.z),
                    _ => Vector3.zero
                };
                orientedBoundsPoints[i] = point;
            }
            itemVolume = CalculateOBBVolume(orientedBoundsPoints);
        }
#endif

        /// <summary>
        /// Get the root transform to use to search for child components for this item. Takes into account breakables
        /// </summary>
        /// <returns></returns>
        public Transform GetItemRoot()
        {
            //The root to use to search for child components
            Transform root = this.transform;
            //If this is a breakable item, the unbreakable parts handles/whooshpoints etc will be under the unbrokenObjectHolder. Otherwise its just under this item
            if (this.TryGetComponent(out Breakable breakableComponent))
            {
                root = breakableComponent.unbrokenObjectsHolder.transform;
            }
            return root;
        }

        [Group("Tabs/General"), Tab("General")]
        [Button]
        public void SetupDefaultComponents()
        {
            if (physicBody == null && GetComponent<Rigidbody>() == null && GetComponent<ArticulationBody>() == null)
            {
                Debug.LogWarning($"[DefaultComponents] Adding Rigidbody to {this.itemId}");
                gameObject.AddComponent<Rigidbody>();
            }

            if (!holderPoint)
            {
                holderPoint = transform.Find("HolderPoint");
            }
            if (!holderPoint)
            {
                Debug.LogWarning($"[DefaultComponents] Adding HolderPoint to {this.itemId}");
                holderPoint = new GameObject("HolderPoint").transform;
                holderPoint.SetParent(transform, false);
            }

            if (!parryPoint)
                parryPoint = transform.Find("ParryPoint");
            if (!parryPoint)
            {
                Debug.LogWarning($"[DefaultComponents] Adding ParryPoint to {this.itemId}");
                parryPoint = new GameObject("ParryPoint").transform;
                parryPoint.SetParent(transform, false);
            }

            if (!spawnPoint)
                spawnPoint = transform.Find("SpawnPoint");
            if (!spawnPoint)
            {
                Debug.LogWarning($"[DefaultComponents] Adding SpawnPoint to {this.itemId}");
                spawnPoint = new GameObject("SpawnPoint").transform;
                spawnPoint.SetParent(transform, false);
            }

            preview = GetComponentInChildren<Preview>();
            if (!preview && transform.Find("Preview"))
                preview = transform.Find("Preview").gameObject.AddComponent<Preview>();
            if (!preview)
            {
                Debug.LogWarning($"[DefaultComponents] Adding Preview to {this.itemId}");
                preview = new GameObject("Preview").AddComponent<Preview>();
                preview.transform.SetParent(transform, false);
            }

            Transform whoosh = transform.Find("Whoosh");
            if (whoosh && !whoosh.GetComponent<WhooshPoint>())
            {
                Debug.LogWarning($"[DefaultComponents] Adding WhooshPoint to {this.itemId}");
                whoosh.gameObject.AddComponent<WhooshPoint>();
            }

            if (!mainHandleRight)
            {
                Debug.LogWarning($"[DefaultComponents] Adding Handle Right to {this.itemId}");
                Handle[] children = GetComponentsInChildren<Handle>();
                for (int i = 0; i < children.Length; i++)
                {
                    Handle handle = children[i];
                    if (handle.IsAllowed(Side.Right))
                    {
                        mainHandleRight = handle;
                        break;
                    }
                }
            }

            if (!mainHandleLeft)
            {
                Debug.LogWarning($"[DefaultComponents] Adding Handle Right to {this.itemId}");
                Handle[] children = GetComponentsInChildren<Handle>();
                for (int i = 0; i < children.Length; i++)
                {
                    Handle handle = children[i];
                    if (handle.IsAllowed(Side.Left))
                    {
                        mainHandleLeft = handle;
                        break;
                    }
                }
            }
            if (!mainHandleRight)
            {
                mainHandleRight = GetComponentInChildren<Handle>();
            }

            physicBody = this.gameObject.GetPhysicBody();
            if (useCustomCenterOfMass)
            {
                physicBody.centerOfMass = customCenterOfMass;
            }
            else
            {
                physicBody.ResetCenterOfMass();
            }

            if (customInertiaTensor)
            {
                if (customInertiaTensorCollider == null)
                {
                    customInertiaTensorCollider = new GameObject("InertiaTensorCollider").AddComponent<CapsuleCollider>();
                    customInertiaTensorCollider.transform.SetParent(transform, false);
                    customInertiaTensorCollider.radius = 0.05f;
                    customInertiaTensorCollider.direction = 2;
                }
                customInertiaTensorCollider.enabled = false;
                customInertiaTensorCollider.isTrigger = true;
                customInertiaTensorCollider.gameObject.layer = 2;
            }
            if (!this.TryGetComponent(out lightVolumeReceiver))
                lightVolumeReceiver = this.gameObject.AddComponent<LightVolumeReceiver>();
            //get all the audiosources on this gameobject, if theres more than 1 warn
            var audioSources = this.GetComponents<AudioSource>();
            if (audioSources.Length > 1)
            {
                Debug.LogWarning($"[DefaultComponents] More than 1 AudioSource found on {this.itemId}, using the first one.");
                audioSource = audioSources[0];
            }
            if (!this.TryGetComponent(out audioSource))
                audioSource = gameObject.AddComponent<AudioSource>();
            
            //Find all the Breakable components and enable the Broken Objects holder so the generation works
            var breakableChildren = this.gameObject.GetComponentsInChildren<Breakable>(true);
            var breakableParents = this.gameObject.GetComponentsInParent<Breakable>(true);
#if UNITY_EDITOR
            foreach (var prefabBreakable in breakableChildren) prefabBreakable.brokenObjectsHolder?.SetActive(true);
            foreach (var prefabBreakable in breakableParents) prefabBreakable.brokenObjectsHolder?.SetActive(true);
            SetupBoundsPoints(); // this cant be done at runtime
#endif
            // disable the broken objects holder again
            foreach (var prefabBreakable in breakableChildren) prefabBreakable.brokenObjectsHolder?.SetActive(false);
            foreach (var prefabBreakable in breakableParents) prefabBreakable.brokenObjectsHolder?.SetActive(false);
        }
        
#if UNITY_EDITOR
        public static void DrawGizmoArrow(Vector3 pos, Vector3 direction, Vector3 upwards, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            Gizmos.color = color;
            Gizmos.DrawRay(pos, direction);
            Vector3 right = Quaternion.LookRotation(direction, upwards) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction, upwards) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
        }

        protected virtual void OnDrawGizmosSelected()
        {


            //draw lines between the oriented bounds points
            Gizmos.color = Color.yellow;
            for (int i = 0; i < orientedBoundsPoints.Length; i++)
            {
                //skip if null
                if (orientedBoundsPoints[i] == null) continue;
                for (int j = 0; j < orientedBoundsPoints.Length; j++)
                {
                    //skip self
                    if (i == j) continue;
                    //draw line between points
                    Gizmos.DrawLine(orientedBoundsPoints[i].position, orientedBoundsPoints[j].position);
                }
            }
            
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(centreOfBuoyancy, 0.05f);
            
            PhysicBody physicBodyInParent = gameObject.GetPhysicBodyInParent();
            if (physicBodyInParent != null)
                Gizmos.DrawWireSphere(transform.TransformPoint(physicBodyInParent.centerOfMass), 0.01f);


            if (!additionalHolderPoints.IsNullOrEmpty())
            {
                int count = additionalHolderPoints.Count;
                for (int i = 0; i < count; i++)
                {
                    HolderPoint holderPoint = additionalHolderPoints[i];
                    Gizmos.matrix = holderPoint.anchor.localToWorldMatrix;
                    DrawGizmoArrow(Vector3.zero, Vector3.forward * 0.1f, Vector3.up, Common.HueColourValue(HueColorName.Purple), 0.1f, 10);
                    DrawGizmoArrow(Vector3.zero, Vector3.up * 0.05f, Vector3.up, Common.HueColourValue(HueColorName.Green), 0.05f);
                }
            }

        }
#endif


        public void SwapWith(string itemID)
        {
        }

        public void ForceUngrabAll()
        {
        }


        public void SetValue(float value)
        {
        }

        public void SetOwner(Owner owner)
        {
        }

        public void ClearValueOverride()
        {
        }


        [ContextMenu("Convert to spawner")]
        public void CreateItemSpawnerFromItem()
        {
            if (GetComponentInChildren<RopeSimple>() != null)
            {
                // don't change for spawner if it's linked to a rope
                return;
            }

            GameObject itemGameObject = gameObject;
            GameObject newItemSpawnerRoot = Instantiate(new GameObject("itemSpawnerRoot"), holderPoint.position, holderPoint.rotation, itemGameObject.transform.parent);

            ItemSpawner newItemSpawner = newItemSpawnerRoot.AddComponent<ItemSpawner>();
            newItemSpawner.name = itemId + "_Spawner";
            newItemSpawner.itemId = itemId;
            newItemSpawner.spawnOnStart = false;

            try
            {
                DestroyImmediate(itemGameObject);
            }
            catch (Exception)
            {
                DestroyImmediate(newItemSpawnerRoot);
            }
        }


        public void OnSpawn(List<ContentCustomData> contentCustomDataList, Owner owner)
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // DESPAWN
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // Keep these method signatures outside so modders can call these events using Unity events
        public void Despawn(float delay)
        {
        }

        [ContextMenu("Despawn")]
        public override void Despawn()
        {
        }

        [Serializable]
        public class HolderPoint
        {
            public HolderPoint(Transform t, string s)
            {
                anchor = t;
                anchorName = s;
            }

            public Transform anchor;
            public string anchorName;
        }


        public void ReturnToPool()
        {
        }

        public HolderPoint GetHolderPoint(string holderPoint)
        {
            HolderPoint hp = additionalHolderPoints.Find(x => x.anchorName == holderPoint);

            if (hp != null)
                return hp;
            if (!string.IsNullOrEmpty(holderPoint))
            {
                Debug.LogWarning("HolderPoint " + holderPoint + " not found on item " + name + " : returning default HolderPoint.");
            }

            return GetDefaultHolderPoint();
        }

        public ItemData.CustomSnap GetCustomSnap(string holderName)
        {
            if (itemId.IsNullOrEmptyOrWhitespace()) return null;
            var data = Catalog.GetData<ItemData>(itemId);
            if (data == null) return null;
            foreach (ItemData.CustomSnap customSnap in data.customSnaps)
            {
                if (customSnap.holderName == holderName)
                {
                    return customSnap;
                }
            }
            return null;
        }

        public HolderPoint GetDefaultHolderPoint()
        {
            return new HolderPoint(holderPoint, "Default");
        }

#if UNITY_EDITOR
        [Button]
        [Group("Tabs/General"), Tab("General")]
        public void AlignTransforms(Transform transformA, Transform transformB)
        {
            Transform childTransform = transformA.GetComponentInParent<Item>() == this ? transformA : transformB;
            Transform otherTransform = childTransform == transformA ? transformB : transformA;
            childTransform.SetPositionAndRotation(otherTransform.position, otherTransform.rotation);
        }
#endif

        /// <summary>
        /// Assign the position and rotation of the item to the spawning cached values.
        /// Stops the physic body from moving.
        /// </summary>
        public void ResetToSpawningTransformation()
        {
        }

        public void InvokeOnImbuesChange(SpellData spellData, float amount, float change, EventTime time)
        {
        }
    }
    
}
