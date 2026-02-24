using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

using TriInspector;

namespace ThunderRoad
{
    public class Shop : ThunderBehaviour
    {
        public static Shop local { get; protected set; }

        public TriDropdownList<string> GetAllShopID() => Catalog.GetDropdownAllID(Category.Shop);

        [Dropdown(nameof(GetAllShopID))]
        public string shopDataID;
        [NonSerialized]
        [ShowInInspector]
        [ReadOnly]
        public ShopData data;
        public Transform defaultShopkeeperSpawnPoint;
        public Transform tutorialShopkeeperSpawnPoint;
        [NonSerialized]
        [ShowInInspector]
        [ReadOnly]
        public Creature shopkeeper;
        [NonSerialized]
        [ShowInInspector]
        [ReadOnly]
        [HideInInspector]
        public BrainModuleShopkeeper moduleShopkeeper;
        public Container shopContents;

        public GameObject priceTagPrefab;

        public string tutorialOverThereAnimationId;
        public float shopKeeperKillReloadDuration = 10;

        [NonSerialized]
        [ShowInInspector]
        [ReadOnly]
        public List<Item> buyingItems = new();

        [NonSerialized]
        [ShowInInspector]
        [ReadOnly]
        public List<Item> sellingItems = new();

        [NonSerialized]
        [ShowInInspector]
        [ReadOnly]
        public int buyingValue = 0;

        [NonSerialized]
        [ShowInInspector]
        [ReadOnly]
        public int sellingValue = 0;

        public Transform shopkeeperBuyWaypoint;
        public Transform shopkeeperSellWaypoint;
        public Transform shopkeeperStoreWaypoint;

        public WayPoint[] idleWaypoints;

        public Zone playerBuyZone;
        public Zone playerSellZone;
        public Zone playerStoreZone;

        public Zone itemBuyZone;
        public Zone itemSellZone;

        public List<ShopDisplay> displayStands { get; protected set; }

        protected Coroutine tutorialCoroutine;

        public enum Transaction
        {
            Buy,
            Sell,
        }

        public enum ShopZone
        {
            Buy,
            Sell,
            Store,
        }

        public event Action<EventTime> onShopContentChanged;
        public event Action<ShopZone, bool> onPlayerChangeZone;
        public event Action<Transaction, Item, bool> onZoneItemChanged;
        public event Action<Transaction, Item> onItemEnterZone;
        public event Action<Transaction, Item> onItemExitZone;
        public event Action<Transaction> onTransactionStart;
        public event Action<Transaction, bool> onTransactionCompleted;

 // ProjectCore
    }
}
