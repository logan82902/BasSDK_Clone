using System.Collections.Generic;
using ThunderRoad.Skill.SpellPower;
using UnityEngine;
using TriInspector;


namespace ThunderRoad
{
    [System.Serializable]
    public class ItemModuleTkGroup : ItemModule
    {
        public float range = 0.1f;
        public float spring = 100;
        public float damper = 10f;
        public float massScale = 10f;
        [MinMaxSlider(0, 0.03f, true)]
        public Vector2 distance = new(0, 0.03f);

    }

}
