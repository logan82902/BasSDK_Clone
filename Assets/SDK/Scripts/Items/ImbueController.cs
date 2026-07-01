using System.Collections.Generic;
using ThunderRoad.Skill;
using TriInspector;
using UnityEngine;

namespace ThunderRoad
{
    [HelpURL("https://kospy.github.io/BasSDK/Components/ThunderRoad/Items/ImbueController.html")]
	[AddComponentMenu("ThunderRoad/Items/Imbue Controller")]
    [DisallowMultipleComponent]
    public class ImbueController : ThunderBehaviour
    {
        public ColliderGroup imbueGroup;
        [Range(-100f, 100f)]
        public float imbueRate;
        [Range(0f, 100f)]
        public float imbueMaxPercent = 50f;
        [Dropdown(nameof(GetAllSpellCastChargeID), AppendToField = true)]
        public string imbueSpellId;
        [DropdownList(nameof(GetAllSpellSkillID))]
        public List<string> imbueSkillIDs;

        public TriDropdownList<string> GetAllSpellCastChargeID() => Catalog.GetDropdownAllID<SpellCastCharge>();
        public TriDropdownList<string> GetAllSpellSkillID() => Catalog.GetDropdownAllID<SpellSkillData>();

        private void OnValidate()
        {
            TryAssignGroup();
        }

        private void TryAssignGroup() => imbueGroup ??= GetComponent<ColliderGroup>();

        [Button]
        public void SetImbueID(string newID)
        {
        }

        [Button]
        public void SetImbueRate(float newRate)
        {
            imbueRate = newRate;
        }

        [Button]
        public void SetImbueMaxPercent(float newMax)
        {
            imbueMaxPercent = newMax;
        }

        [Button]
        public void ClearImbueID() => SetImbueID(string.Empty);

        [Button]
        public void ImbueUseStart()
        {
        }

        [Button]
        public void ImbueUseEnd()
        {
        }

    }
}
