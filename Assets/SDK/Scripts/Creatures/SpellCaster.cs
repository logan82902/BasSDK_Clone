using UnityEngine;
using System;
using System.Collections.Generic;
using ThunderRoad.Skill.SpellPower;


using TriInspector;

namespace ThunderRoad
{
    [HelpURL("https://kospy.github.io/BasSDK/Components/ThunderRoad/SpellCaster")]
    [AddComponentMenu("ThunderRoad/Creatures/Spell caster")]
    public class SpellCaster : MonoBehaviour
    {
        public Transform fire;
        public Transform magicSource;
        public Transform rayDir;
        public FloatHandler chargeSpeedMult;

    }
}