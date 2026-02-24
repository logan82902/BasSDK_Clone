using System.Collections;
using TriInspector;

namespace ThunderRoad.AI.Action
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class SetStanceFromData : ActionNode
    {
        public bool getStanceFromWeapons = true;
        public string specifiedID;
        public bool onlyOnce = false;


    }
}
