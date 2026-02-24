using System;
using System.Collections.Generic;


using TriInspector;

namespace ThunderRoad
{
    [Serializable]
    public class DropTable<T>
    {
        #region Data

        [TableList(AlwaysExpanded = true)]
        public List<Drop> drops;
        private float probabilityTotalWeight;
        //private static int maxPickCount = 20;
        #endregion Data

        [Serializable]
        public class Drop
        {
            [PropertyOrder(0)]
            [JsonMergeKey]
            public T dropItem;

            [PropertyOrder(1)]
            public float probabilityWeight;

            [PropertyOrder(2)]
            [ShowInInspector]
            [ReadOnly]
            //[ProgressBar(0, 100)]
            [NonSerialized]
            public float probabilityPercent;

            [NonSerialized]
            public float probabilityRangeFrom;
            [NonSerialized]
            public float probabilityRangeTo;

            public Drop Clone()
            {
                return MemberwiseClone() as Drop;
            }
        }


        #region Methods
        public DropTable()
        {
            drops = new List<Drop>();
        }


        /// <summary>
        /// Calculates the percentage and asigns the probabilities how many times
        /// the items can be picked. Function used also to validate data when tweaking numbers in editor.
        /// </summary>	
        public void CalculateWeight()
        {
        }

        private static float ComputeDropListRange(ref List<Drop> dropList)
        {
return 0;
        }

        #endregion Methods
    }
}