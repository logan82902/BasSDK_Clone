using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThunderRoad.Modules;
using Object = UnityEngine.Object;
using TriInspector;
using Newtonsoft.Json;

namespace ThunderRoad
{
    public class LevelModuleWaveAssault : LevelModule, ISurvivalModule
    {
        public string rewardPillarAddress;

        public int defaultLength = 3;

        [Dropdown(nameof(GetAllTextGroupID))]
        public string textFightGroupId;
        [Dropdown(nameof(GetAllTextFightGroupId))]
        public string textFightId;

        [Dropdown(nameof(GetAllTextFightGroupId))]
        public string textWaveId;

        [Dropdown(nameof(GetAllTextGroupID))]
        public string textReturnHomeGroupId;
        [Dropdown(nameof(GetAllTextReturnHomeGroupId))]
        public string textReturnHomeId;
        
        public string raiseSoundAudioContainerAddress = "Bas.AudioGroup.Survival.Pillar.Raise.Stone";
        public string lowerSoundAudioContainerAddress = "Bas.AudioGroup.Survival.Pillar.Lower.Stone";
        
        [NonSerialized]
        public AudioContainer raiseSound;
        [NonSerialized]
        public AudioContainer lowerSound;
        
        public bool useCustomTeleportPoint = false;
        public Vector3 customTeleportPoint = Vector3.zero;

        public bool TryGetTeleportPosition(out Vector3 position)
        {
            position = customTeleportPoint;
            return useCustomTeleportPoint;
        }

        public bool TryGetPillarSounds(out AudioContainer raise, out AudioContainer lower)
        {
            raise = raiseSound;
            lower = lowerSound;
            return true;
        }

        [JsonIgnore]
        [ShowInInspector]
        public bool hasPillarZone
        {
            get
            {
                return pillarZone.HasValue;
            }
            set
            {
                if (value)
                {
                    pillarZone = new Bounds(Vector3.zero, Vector3.one);
                }
                else
                {
                    pillarZone = null;
                }
            }
        }
        [JsonIgnore]
        [ShowInInspector]
        [HideLabel]
        [ShowIf(nameof(hasPillarZone))]
        public Bounds pillarZoneBounds
        {
            get
            {
                return pillarZone.GetValueOrDefault();
            }
            set
            {
                pillarZone = value;
            }
        }
        [HideInInspector]
        public Bounds? pillarZone;
        public float returnHomeFadeInDuration = 2.0f;



        public TriDropdownList<string> GetAllTextGroupID() => Catalog.GetTextData().GetDropdownAllTextGroups();

        public TriDropdownList<string> GetAllTextFightGroupId() => Catalog.GetTextData().GetDropdownAllTexts(textFightGroupId);

        public TriDropdownList<string> GetAllTextReturnHomeGroupId() => Catalog.GetTextData().GetDropdownAllTexts(textReturnHomeGroupId);
    }
}