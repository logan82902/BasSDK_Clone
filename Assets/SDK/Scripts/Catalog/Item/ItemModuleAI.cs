using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using TriInspector;

namespace ThunderRoad
{
    [System.Serializable]
    [DeclareBoxGroup("Weapon")]
    [DeclareBoxGroup("Attack")]
    [DeclareBoxGroup("Defense")]
    public class ItemModuleAI : ItemModule
    {
        public enum WeaponClass
        {
            None,
            Melee,
            Bow,
            Arrow,
            Shield,
            Wand,
            Crossbow,
            Bolt,
            Firearm,
            Throwable
        }

        public enum AttackType
        {
            None = 0,
            Swing = 1,
            Thrust = 2,
        }

        [Flags]
        public enum AttackTypeFlags
        {
            None = 0,
            Swing = 1,
            Thrust = 2,
        }

        public enum WeaponHandling
        {
            None,
            OneHanded,
            TwoHanded,
        }

        [Serializable]
        public class StanceInfo
        {
            public enum Offhand
            {
                Empty,
                SameItem,
                ItemDuplicate,
                AnyMelee,
                AnyShield,
                AnyFirearm,
                AnyThrowable,
                Anything,
            }

            [JsonMergeKey]
            public Offhand offhand;
            public float grabAIHandleRadius = 0f;
            public string stanceDataID;
        }

        [Serializable]
        public class RangedWeaponData
        {
            public TriDropdownList<string> GetAllHolderSlots() => Catalog.GetDropdownHolderSlots();

            [Dropdown(nameof(GetAllHolderSlots))]
            public string ammoType = "";
            public float tooCloseDistance = 5f;
            public Vector2 spread = new(1f, 1f);
            public float projectileSpeed = 20f;
            public bool accountForGravity = true;
            public Vector3 weaponAimAngleOffset = new();
            public Vector3 weaponHoldPositionOffset = new();
            public Vector3 weaponHoldAngleOffset = new();
            public string customRangedAttackAnimationData = "";
        }

        [GroupNext("Weapon")]
        public WeaponClass primaryClass = WeaponClass.None;
        public WeaponClass secondaryClass = WeaponClass.None;
        public WeaponHandling weaponHandling = WeaponHandling.None;
        public WeaponHandling secondaryHandling = WeaponHandling.None;

        [GroupNext("Attack")]
        public AttackTypeFlags weaponAttackTypes = AttackTypeFlags.Swing | AttackTypeFlags.Thrust;
        public bool alwaysPrimary = false;
        public StanceInfo defaultStanceInfo;
        [HideReferencePicker]
        [ListDrawerSettings]
        public List<StanceInfo> stanceInfosByOffhand;
        [NullCheckbox]
        [InlineProperty]
        [SerializeReference]
        [HideReferencePicker]
        public RangedWeaponData rangedWeaponData;

        [GroupNext("Defense")]
        public bool ignoredByDefense = false;
        public float armResistanceMultiplier = 3;
        public bool allowDynamicHeight;
        public bool defenseHasPriority;

        public override void OnItemDataRefresh(ItemData data)
        {
            base.OnItemDataRefresh(data);
            if (primaryClass == WeaponClass.Melee)
            {
                if (defaultStanceInfo == null)
                {
                    defaultStanceInfo = new StanceInfo()
                    {
                        offhand = StanceInfo.Offhand.Anything,
                        stanceDataID = "HumanMelee1hStance"
                    };
                }
                if (stanceInfosByOffhand == null)
                {
                    if (data.id.Contains("Axe") || data.id.Contains("Mace")) weaponAttackTypes = AttackTypeFlags.Swing;
                    if (weaponHandling == WeaponHandling.OneHanded)
                    {
                        stanceInfosByOffhand = new List<StanceInfo>()
                        {
                            new StanceInfo()
                            {
                                offhand = StanceInfo.Offhand.Empty,
                                stanceDataID = "HumanMelee1hStance"
                            },
                            new StanceInfo()
                            {
                                offhand = StanceInfo.Offhand.AnyShield,
                                stanceDataID = "HumanMeleeShieldStance"
                            },
                            new StanceInfo()
                            {
                                offhand = StanceInfo.Offhand.AnyMelee,
                                stanceDataID = "HumanMeleeDualWieldStance"
                            }
                        };
                    }
                    else
                    {
                        stanceInfosByOffhand = new List<StanceInfo>()
                        {
                            new StanceInfo()
                            {
                                offhand = StanceInfo.Offhand.SameItem,
                                stanceDataID = ""
                            }
                        };
                    }
                }
            }
        }

    }
}
