using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ThunderRoad
{
    public class UISelectionListButtons : MonoBehaviour
    {
        [SerializeField] public UICustomisableButton nextButton;
        [SerializeField] public UICustomisableButton previousButton;
        [SerializeField] public TextMeshProUGUI title;
        [SerializeField] public TextMeshProUGUI value;

        public event Action<UISelectionListButtons> onUpdateValueEvent;

        public virtual bool Loop => true;

    }
}