using TriInspector;
using UnityEngine;
using UnityEngine.Events;

namespace ThunderRoad
{
    [DeclareHorizontalGroup("Horiz", Sizes = new float[] { 0f, 140f })]
    public class RandomChanceActivator : ThunderBehaviour
    {
        [Group("Horiz")]
        [LabelText("Chance: 1/")]
        [AutoLabelWidth]
        public int chance = 2;
        [Group("Horiz")]
        [AutoLabelWidth]
        public bool forceNextActivation = false;
        [Tooltip("This event runs if the random chance succeeds.")]
        public UnityEvent action;
        [Tooltip("This event runs if the random chance fails.")]
        public UnityEvent failAction;

        [Button]
        public void TriggerRandom()
        {
            if (forceNextActivation || UnityEngine.Random.Range(0, chance) == 0)
            {
                action?.Invoke();
                forceNextActivation = false;
            }
            else
            {
                failAction?.Invoke();
            }
        }
    }
}
