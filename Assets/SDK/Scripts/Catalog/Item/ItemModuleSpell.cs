using System;
using System.Collections.Generic;
using TriInspector;

namespace ThunderRoad
{
    [System.Serializable]
    public class ItemModuleSpell : ItemModule
    {
        [Dropdown(nameof(GetAllSpellID))]
        public string spellId;
        [NonSerialized]
        public SpellData spellData;

        public TriDropdownList<string> GetAllSpellID() => Catalog.GetDropdownAllID<SpellData>();

    }
}
