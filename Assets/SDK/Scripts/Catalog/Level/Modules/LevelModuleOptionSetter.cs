using System;
using System.Collections;
using System.Collections.Generic;

namespace ThunderRoad
{
    public class LevelModuleOptionSetter : LevelModule
    {
        public List<Option> levelOptions;
        public override IEnumerator OnLoadCoroutine()
        {

            return base.OnLoadCoroutine();
        }

        [Serializable]
        public struct Option
        {
            public string option;
            public string value;
        }
    }
}