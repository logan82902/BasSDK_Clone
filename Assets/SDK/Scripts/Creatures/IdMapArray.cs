using System;
using UnityEngine;
using TriInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace ThunderRoad
{
    [DrawWithTriInspector]
    [PreferBinarySerialization]
    public class IdMapArray : ScriptableObject
    {
        [Tooltip("The path to the ID map texture.")]
        [ReadOnly]
        public string idMapPath;
        [Tooltip("The factor to scale the ID map down by.")]
        [ReadOnly]
        public int scale = 2;
        [ReadOnly]
        [HideInInspector]
        public NibbleArray nibbleArray;
        //mainly for recreating the texture from the array
        [ReadOnly]
        public int originalWidth;
        [ReadOnly]
        public int originalHeight;
        [ReadOnly]
        public int width;
        [ReadOnly]
        public int height;
        [ShowInInspector]
        [ReadOnly]
        private int estimatedIDMapSize;
        [ShowInInspector]
        [ReadOnly]
        private int estimatedIDMapArraySize;
        [NonSerialized]
        [ShowInInspector]
        [ReadOnly]
        public Texture2D debugTexture;

        public int GetIdAtUV(float ux, float uy)
        {
            if (nibbleArray == null || nibbleArray.Length == 0)
            {
                Debug.LogError("ID map array is null.");
                return -1;
            }
            //handle uvs outside 0-1 range by wrapping them
            ux -= Mathf.Floor(ux);
            uy -= Mathf.Floor(uy);
            
            //convert the uv to pixel coordinates
            int x = (int)(ux * width);
            int y = (int)(uy * height);

            int i = y * width + x;
            byte id = nibbleArray[i];
            return id;    
        }
        
        [Button]
        public void ConvertArrayToTexture()
        {
            if(nibbleArray == null || nibbleArray.Length == 0){
                Debug.LogError("ID map array is null.");
                return;
            }
            
            //we need to load the catalog to get the colour ids
            Catalog.EditorLoadAllJson();
            
            Texture2D texture = new Texture2D(width,height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    try
                    {
                        int i = y * width + x;
                        byte id = nibbleArray[i];
                        Color color = Catalog.gameData.GetIDMapColor(id);
                        texture.SetPixel(x, y, color);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"Error setting pixel at {x}, {y}: {e.Message}");
                    }
                }
            }
            texture.Apply();
            debugTexture = texture;
            //update the estimated sizes
            //ID map is RGB so 3 bytes per pixel
            estimatedIDMapSize = originalWidth * originalHeight * 3;
            //ID map array is 1 byte per 2 pixels
            estimatedIDMapArraySize =  nibbleArray.Length / 2;
            //open the texture in the inspector
#if UNITY_EDITOR
            EditorUtility.OpenPropertyEditor(debugTexture);
#endif
        }

    }
}
