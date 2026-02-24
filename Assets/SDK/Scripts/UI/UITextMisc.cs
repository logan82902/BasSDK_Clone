using UnityEngine;
using TMPro;
using System.Collections.Generic;


using TriInspector;

namespace ThunderRoad
{
    public class UITextMisc : UIText
    {
        public Value value;

        public enum Value
        {
            GameVersion,
            DungeonInfo,
        }

    }
}
