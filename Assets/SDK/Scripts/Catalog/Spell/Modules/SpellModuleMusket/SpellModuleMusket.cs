using System;
using TriInspector;
using Newtonsoft.Json;

namespace ThunderRoad
{
    sealed internal class SpellModuleMusketCompatibility : SpellModuleMusket
    {
        public void SetIdsFromSpell(SpellCastCharge spell)
        {
            musketHammerEffectId = spell.musketHammerEffectId;
            musketShootEffectId = spell.musketShootEffectId;
            musketChargeEffectId = spell.musketChargeEffectId;
            //clear the old fields
            spell.musketHammerEffectId = string.Empty;
            spell.musketShootEffectId = string.Empty;
            spell.musketChargeEffectId = string.Empty;
        }

    }

    public abstract class SpellModuleMusket<T> : SpellModuleMusket where T: SpellCastCharge
    {
    }

    public abstract class SpellModuleMusket : SpellModule
    {
        [Dropdown(nameof(GetAllEffectID))]
        public string musketHammerEffectId;

        [NonSerialized]
        [JsonIgnore]
        public EffectData musketHammerEffectData;

        [Dropdown(nameof(GetAllEffectID))]
        public string musketShootEffectId;

        [NonSerialized]
        [JsonIgnore]
        public EffectData musketShootEffectData;

        [Dropdown(nameof(GetAllEffectID))]
        public string musketChargeEffectId;

        [NonSerialized]
        [JsonIgnore]
        public EffectData musketChargeEffectData;

    }
}