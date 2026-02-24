using UnityEngine;
using TriInspector;

namespace ThunderRoad.AI.Action
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class SaveTimeInBlackboard : ActionNode
    {
        public string variableName;
        public bool onlyOnce = true;

    }
}
