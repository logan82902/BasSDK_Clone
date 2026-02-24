using System;
using System.Collections.Generic;

using TriInspector;

namespace ThunderRoad
{
    [System.Serializable]
    public class GuardPose : StanceNode
    {
        [DropdownList(nameof(GetRiposteOptions), Searchable = false)]
        [Group("StanceNode")]
        [ListDrawerSettings(AlwaysExpanded = true)]
        public List<string> riposteOptions;

        [NonSerialized]
        public List<AttackMotion> ripostes;

        public override bool customID => false;

        public TriDropdownList<string> GetRiposteOptions()
        {
            TriDropdownList<string> options = new();
            if (stanceData is MeleeStanceData meleeStanceData)
            {
                for (int i = 0; i < meleeStanceData.ripostes.Count; i++)
                {
                    options.Add(meleeStanceData.ripostes[i].id, meleeStanceData.ripostes[i].id);
                }
            }
            return options;
        }

    }
}
