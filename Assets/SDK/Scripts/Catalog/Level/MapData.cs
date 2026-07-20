using System;
using System.Collections;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace ThunderRoad
{
    public class MapData : CatalogData
    {
        public string worldmapPrefabAddress = "Bas.Image.Map.Default";
        public string worldmapTextureAddress = "Bas.Worldmap.Eraden";
        public string worldmapLocalisationId;
        public string bookmarkIconAddress = "Bas.Icon.Location.Default";
        public Color bookmarkColor = Color.gray;
        public Color iconColor = Color.white;
        public int order;
        [NonSerialized]
        public IResourceLocation bookmarkIconLocation;
        [NonSerialized]
        public Sprite bookmarkIcon;
        
        [NonSerialized]
        public IResourceLocation worldmapPrefabLocation;
        [NonSerialized]
        public GameObject worldmapPrefab;
        
        [NonSerialized]
        public IResourceLocation worldmapTextureLocation;
        [NonSerialized]
        public Texture2D worldmapTexture2D;


        public override IEnumerator OnCatalogRefreshCoroutine()
        {
                yield return Catalog.LoadLocationCoroutine<GameObject>(worldmapPrefabAddress, value => worldmapPrefabLocation = value, id);
                yield return Catalog.LoadLocationCoroutine<Texture2D>(worldmapTextureAddress, value => worldmapTextureLocation = value, id);
                yield return Catalog.LoadLocationCoroutine<Sprite>(bookmarkIconAddress, value => bookmarkIconLocation = value, id);
        }

        public void PreloadMapData()
        {
            if (worldmapPrefabLocation != null)
            {
                Catalog.LoadAssetAsync<GameObject>(worldmapPrefabLocation, value => worldmapPrefab = value, "worldmap");
            }

            if (worldmapTextureLocation != null)
            {
                Catalog.LoadAssetAsync<Texture2D>(worldmapTextureLocation, value => worldmapTexture2D = value, "worldmapTexture");
            }
            
            if (bookmarkIconLocation != null)
            {
                Catalog.LoadAssetAsync<Sprite>(bookmarkIconLocation, value => bookmarkIcon = value, "bookmarkIcon");
            }
        }

        public bool IsMapDataPreloaded()
        {
            if (worldmapPrefabLocation != null && worldmapPrefab == null)
            {
                return false;
            }

            if (worldmapTextureLocation != null && worldmapTexture2D == null)
            {
                return false;
            }
            if (bookmarkIconLocation != null && bookmarkIcon == null)
            {
                return false;
            }
            return true;
        }
        
        public void ReleaseMapData()
        {
            Catalog.ReleaseAsset(worldmapPrefab);
            Catalog.ReleaseAsset(worldmapTexture2D);
            Catalog.ReleaseAsset(bookmarkIcon);
            worldmapPrefab = null;
            worldmapTexture2D = null;
            bookmarkIcon = null;

        }
    }
}
