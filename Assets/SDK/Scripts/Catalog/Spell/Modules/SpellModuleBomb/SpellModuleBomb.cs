using UnityEngine;

namespace ThunderRoad
{
    public abstract class SpellModuleBomb<T> : SpellModuleBomb where T : SpellCastCharge
    {
    }

    public abstract class SpellModuleBomb : SpellModule
    {
    }
}

