using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using TriInspector;
using UnityEditor;
using UnityEngine;

namespace ThunderRoad
{
    public class CatalogFavouritesSO : ScriptableObject
    {
        [ShowInInspector]
        public static CatalogFavouritesSO Instance
        {
            get
            {
                var filePath = "Assets/Personal/catalog_favourites.asset";
                var instance = AssetDatabase.LoadAssetAtPath<CatalogFavouritesSO>(filePath);
                if (instance != null)
                {
                    return instance;
                }
                
                instance = CreateInstance<CatalogFavouritesSO>();
                instance.favourites = new CatalogFavouriteMap();

                string directoryName = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName);

                AssetDatabase.CreateAsset(instance, filePath);
                AssetDatabase.SaveAssets();

                return instance;
            }
        }

        [SerializeField]
        public CatalogFavouriteMap favourites;

        public bool IsFavourite(CatalogData data)
        {
            return favourites.TryGetValue(Catalog.GetCategory(data.GetType()), out var list)
                   && list.Contains(data.id);
        }

        public bool ToggleFavourite(CatalogData data) => IsFavourite(data) ? RemoveFavourite(data) : AddFavourite(data);

        public bool AddFavourite(CatalogData data)
        {
            var category = Catalog.GetCategory(data.GetType());
            if (favourites.TryGetValue(category, out var list))
            {
                list.Add(data.id);
                EditorUtility.SetDirty(this);
                return true;
            }

            list = new ListOfStrings();
            favourites[category] = list;

            if (list.Contains(data.id)) return false;
            list.Add(data.id);
            EditorUtility.SetDirty(this);
            return true;
        }

        public bool RemoveFavourite(CatalogData data)
        {
            var category = Catalog.GetCategory(data.GetType());
            bool removed = favourites.TryGetValue(category, out var list) && list.Remove(data.id);
            EditorUtility.SetDirty(this);
            return removed;
        }
    }

    [Serializable]
    public class CatalogFavouriteMap : UnitySerializedDictionary<Category, ListOfStrings>
    { }

    [Serializable]
    public class ListOfStrings : IEnumerable<string>
    {
        [SerializeField]
        public List<string> strings = new();

        public bool Contains(string value) => strings.Contains(value);
        public void Add(string value) => strings.Add(value);
        public bool Remove(string value) => strings.Remove(value);
        public IEnumerator<string> GetEnumerator() => strings.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => strings.GetEnumerator();
    }
}
