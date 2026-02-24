using System;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace ThunderRoad
{
    [Serializable]
    public class InventoryStart
    {
        [Dropdown(nameof(GetAllContainerID))]
        public string containerID;

        [Dropdown(nameof(GetAllTextGroupID))]
        public string textGroupId;

        [Dropdown(nameof(GetAllTextId))]
        public string titleTextID;

        [ShowIf(nameof(titleTextID), null)]
        public string titleText;

        [Dropdown(nameof(GetAllTextId))]
        public string descriptionTextID;
        
        [ShowIf(nameof(descriptionTextID), null)]
        public string descriptionText;

        public string titleImageAddress;

        [ShowInInspector]
        public Dictionary<string, float> currencies;

        [NonSerialized]
        public Sprite titleImageSprite;
        public event Action<Sprite> onSpriteTitleImageLoade;


        public TriDropdownList<string> GetAllContainerID() => Catalog.GetDropdownAllID(Category.Container);

        public TriDropdownList<string> GetAllTextGroupID() => Catalog.GetTextData().GetDropdownAllTextGroups();

        public TriDropdownList<string> GetAllTextId() => Catalog.GetTextData().GetDropdownAllTexts(textGroupId);
    }
}