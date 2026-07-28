using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using ThunderRoad.Skill;

using TriInspector;

namespace ThunderRoad
{
    [HelpURL("https://kospy.github.io/BasSDK/Components/ThunderRoad/Levels/ImbueZone.html")]
    [RequireComponent(typeof(Collider))]
    public class ImbueZone : MonoBehaviour
    {
        public float transferRate = 0.1f;
        public float transferMaxPercent = 50f;

        [Dropdown(nameof(GetAllSpellCastChargeID), AppendToField = true)]
        public string imbueSpellId;
        [FormerlySerializedAs("imbueSkillIds")]
        [DropdownList(nameof(GetAllSpellSkillID))]
        public List<string> imbueSkillIDs;

        public TriDropdownList<string> GetAllSpellCastChargeID()
            => Catalog.GetDropdownAllID<SpellCastCharge>();
        public TriDropdownList<string> GetAllSpellSkillID()
            => Catalog.GetDropdownAllID<SpellSkillData>();

    }
}