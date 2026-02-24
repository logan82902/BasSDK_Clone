using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ThunderRoad
{
    public class UISelectionListButtonsLevelModeOption : UISelectionListButtons, IPointerEnterHandler
    {
        // This works I guess
        public static UISelectionListButtonsLevelModeOption presetOption;

        public Image starImagePrefab = null;
        public Sprite starImage = null;
        public Sprite starImageFill = null;
        public Transform starPlace = null;
        public int maxIcon = 5;
        public bool isCustom;

        public void OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
        {
        }
    }
}