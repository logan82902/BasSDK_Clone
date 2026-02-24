using System;
using UnityEngine;
using UnityEngine.UI;

namespace ThunderRoad
{
    public class UIWorldMapBookmark : MonoBehaviour
    {
        public Image icon;
        public Image background;
        public int order;
        [NonSerialized]
        public UIWorldMapBoard mapBoard;
        
    }
}
