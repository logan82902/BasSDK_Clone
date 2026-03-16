using Newtonsoft.Json;
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

        [JsonProperty("currencies")]
        private Dictionary<string, float> currenciesJsonProp
        {
            get
            {
                return currencies;
            }
            set
            {
                currencies = new CurrencyDictionary();
                if (value != null)
                {
                    foreach (KeyValuePair<string, float> pair in value)
                    {
                        currencies[pair.Key] = pair.Value;
                    }
                }
            }
        }

        [JsonIgnore]
        [ShowInInspector]
        [LabelWidth(0.0001f)]
        public CurrencyDictionary currencies;

        [NonSerialized]
        public Sprite titleImageSprite;
        public event Action<Sprite> onSpriteTitleImageLoade;

        [Serializable]
        public class CurrencyDictionary : UnitySerializedDictionary<string, float> { }


        public TriDropdownList<string> GetAllContainerID() => Catalog.GetDropdownAllID(Category.Container);

        public TriDropdownList<string> GetAllTextGroupID() => Catalog.GetTextData().GetDropdownAllTextGroups();

        public TriDropdownList<string> GetAllTextId() => Catalog.GetTextData().GetDropdownAllTexts(textGroupId);
    }
}