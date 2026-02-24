using System.Collections;
using System.Collections.Generic;
using TriInspector;

namespace ThunderRoad
{
    public class LevelModuleShop : LevelModule
    {
        private TriDropdownList<string> AllShopIDs() => Catalog.GetDropdownAllID<ShopData>();

        [Dropdown(nameof(AllShopIDs))]
        public string shopID;

        public override IEnumerator OnLoadCoroutine()
        {
            yield return null;
            
        }
    }
}
