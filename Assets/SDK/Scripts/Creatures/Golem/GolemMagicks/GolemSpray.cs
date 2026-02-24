using System;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;

namespace ThunderRoad
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "ThunderRoad/Creatures/Golem/Magic spray config")]
    public class GolemSpray : GolemAbility
    {
        public TriDropdownList<string> GetValidSkills()
        {
            TriDropdownList<string> result = new() { new TriDropdownItem<string>() { Text = "None", Value = "" } };
            result.needsValidation = Catalog.IsJsonLoaded();
            if (!Catalog.IsJsonLoaded())
            {
                return result;
            }
            foreach (SkillData skillData in Catalog.GetDataList<SkillData>())
            {
                if (skillData is IGolemSprayable)
                {
                    result.Add(skillData.id, skillData.id);
                }
            }
            return result;
        }

        [Dropdown(nameof(GetValidSkills))]
        public string spraySkillID;
        public List<string> spraySources = new List<string>();
        public GolemController.AttackMotion sprayMotion = GolemController.AttackMotion.Spray;
        public float sprayAngle = 90f;

    }
}
