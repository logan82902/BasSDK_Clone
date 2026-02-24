using System.Collections;
using System.Collections.Generic;
using TriInspector;

namespace ThunderRoad
{
    public class LevelModuleMusic : LevelModule
    {
        [Dropdown(nameof(GetAllMusicID))]
        public string dynamicMusic = "";

        public TriDropdownList<string> GetAllMusicID() => Catalog.GetDropdownAllID(Category.Music);

 //ProjectCore
    }
}
