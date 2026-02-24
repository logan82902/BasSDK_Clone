using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

using TriInspector;

namespace ThunderRoad
{
    [Serializable]
    public class DataIdContainer<T> where T : CatalogData
    {
        #region Data

        [Dropdown(nameof(GetAllCategoryTypeId))]
        public string dataId;
        #endregion Data

        #region Properties
        protected virtual bool ShowDataInInspector => true;
        [ShowInInspector]
        [JsonIgnore]
        public Category Category
        {
            get { return Catalog.GetCategory(typeof(T)); }
        }
        [ShowInInspector]
        [ShowIf("ShowDataInInspector")]
        [JsonIgnore]
        public T Data
        {
            get
            {
                if (string.IsNullOrEmpty(dataId))
                {
                    return null;
                }

                T data;

                if (Catalog.IsJsonLoaded())
                {
	                if (Catalog.TryGetData<T>(dataId, out data))
	                {
		                return data;
	                }

	                Debug.LogError("Catalog, can not find data id : " + dataId + " of type : " + Category);
                }
                return null;
            }
        }
        #endregion Properties


        #region Methods
        public DataIdContainer(string dataId)
        {
            this.dataId = dataId;
        }

        public DataIdContainer()
        {
            this.dataId = string.Empty;
        }

        public override bool Equals(object obj)
        {
            if (obj is string otherId)
            {
                if (dataId == null && otherId == null)
                {
                    return true;
                }

                return dataId.Equals(otherId);
            }

            return false;
        }

        public TriDropdownList<string> GetAllCategoryTypeId() => Catalog.GetDropdownAllID(Category);

        public override int GetHashCode() => HashCode.Combine(dataId, ShowDataInInspector, Category, Data);

        #endregion Methods
    }


    [Serializable]
    public abstract class DataIdContainerChoice
    {
        #region Data

        [Dropdown(nameof(GetAllCategoryTypeId))]
        public string dataId;

        [Dropdown(nameof(GetAllowedCategoryType))]
        public Category category;

        #endregion Data

        #region Properties
        protected virtual bool ShowDataInInspector => true;

        [ShowInInspector]
        [ShowIf("ShowDataInInspector")]
        [JsonIgnore]
        public CatalogData Data
        {
            get
            {
                if (string.IsNullOrEmpty(dataId))
                {
                    return null;
                }

                CatalogData data = Catalog.GetData(category, dataId);
                if (data != null) return data;

                Debug.LogError("Catalog, can not find data id : " + dataId + " of type : " + category);
                return null;
            }
        }
        #endregion Properties


        #region Methods
        public DataIdContainerChoice(string dataId, ThunderRoad.Category category)
        {
            this.dataId = dataId;
            this.category = category;
        }

        public DataIdContainerChoice()
        {
            this.dataId = string.Empty;
            this.category = GetCategoryAllowed()[0];
        }

        public abstract ThunderRoad.Category[] GetCategoryAllowed ();

        public TriDropdownList<string> GetAllCategoryTypeId() => Catalog.GetDropdownAllID(category);

        public TriDropdownList<Category> GetAllowedCategoryType()
        {
            TriDropdownList<Category> list = new();
            Category[] allowed = GetCategoryAllowed();
            for (int i = 0; i < allowed.Length; i++)
            {
                list.Add(allowed[i].ToString(), allowed[i]);
            }

            return list;
        }

        #endregion Methods
    }
}