using System;
using System.Collections.Generic;


using TriInspector;

namespace ThunderRoad
{
    [Serializable]
#if UNITY_EDITOR
    [EditorLoadPriority(0)]
#endif
    public abstract class LootTableBase : CatalogData
    {
        public virtual ItemData PickOne(int level = 0, int searchDepth = 0, System.Random randomGen = null)
        {
            return null;
        }

        // Can return null
        public virtual List<ItemData> Pick(int level = 0, int searchDepth = 0, System.Random randomGen = null)
        {
            return null;
        }
        
        public virtual List<ItemData> GetAll(int level = -1, int pickCount = 0)
        {
            List<ItemData> itemDataList = new List<ItemData>();
            return itemDataList;
        }

        public virtual int Count(int level = -1)
        {
            return 0;
        }

        public virtual bool DoesLootTableContainItemID(string id, int level = -1, int depth = 0)
        {
	        return false;
        }
        
        public virtual void RenameItem(string itemId, string newName)
        {
	        
        }
    }
}
