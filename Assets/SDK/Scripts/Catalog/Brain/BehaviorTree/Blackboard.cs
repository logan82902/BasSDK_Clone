using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;

namespace ThunderRoad.AI
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class Blackboard
    {
        public List<IDictionary> dictionaries;

    }
}