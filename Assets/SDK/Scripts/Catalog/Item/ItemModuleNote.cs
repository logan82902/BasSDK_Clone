
using TriInspector;

namespace ThunderRoad
{
    [System.Serializable]
    public class ItemModuleNote : ItemModule
    {
        public bool isCustomNote = false;
        [ShowIf("isCustomNote")]
        public string contentPrefabAddress = "";
        [ShowIf("isCustomNote")]
        public string groupTextId;
        [ShowIf("isCustomNote")]
        public string textId;
    }
}