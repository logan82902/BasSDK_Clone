using UnityEngine;
using TriInspector;

namespace ThunderRoad.AI.Action
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class ConsoleLog : ActionNode
    {
        public string consoleOutput = "Console log output";
        public State returnValue = State.SUCCESS;
        public int alertLevel = 0;

    }
}
