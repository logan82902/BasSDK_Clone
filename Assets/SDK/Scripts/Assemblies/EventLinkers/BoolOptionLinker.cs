using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;
using TriInspector;

namespace ThunderRoad
{
    public class BoolOptionLinker : ModOptionLinker<bool, BoolChecker>
    {
        public override void UnsubscribeNamedMethods() { }
    }

    [System.Serializable]
    public class BoolChecker : UpdateChecker<bool>
    {
        public override TriDropdownList<Comparison> AllowedComparisons()
        {
            return new TriDropdownList<Comparison>()
            {
                new TriDropdownItem<Comparison>() { Text = "Equals", Value = Comparison.Equals },
                new TriDropdownItem<Comparison>() { Text = "Not Equals", Value = Comparison.NotEquals },
            };
        }

        public override bool Check(bool value)
        {
            switch (comparison)
            {
                case Comparison.NotEquals:
                    return value != comparedValue;
                case Comparison.Equals:
                    return value == comparedValue;
            }
            return false;
        }
    }
}
