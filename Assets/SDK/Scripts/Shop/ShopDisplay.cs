using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

using TriInspector;

namespace ThunderRoad
{
    public class ShopDisplay : ThunderBehaviour
    {
        public enum DisplayType
        {
            Holders,
            Shelf,
            Basket,
        }

        public string displayCategory;
        [NonSerialized]
        public ShopData.ShopStockCategory connectedCategory;

        public DisplayType displayType;

        public List<Transform> includeTransforms = new List<Transform>();


        [NonSerialized]
        [ShowInInspector]
        [ReadOnly]
        [ShowIf("displayType", DisplayType.Holders)]
        public List<Holder> holders;
        [NonSerialized]
        [ShowInInspector]
        [ReadOnly]
        [ShowIf("displayType", DisplayType.Shelf)]
        public List<ItemShelf> shelves;
        [NonSerialized]
        [ShowInInspector]
        [ReadOnly]
        [ShowIf("displayType", DisplayType.Basket)]
        public List<ItemBasket> baskets;

        public Shop shop;
        [NonSerialized]
        [ShowInInspector]
        [ReadOnly]
        public List<Item> displayItems;

    }
}
