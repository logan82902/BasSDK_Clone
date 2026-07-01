using System;
using System.Collections.Generic;
using TriInspector;
using UnityEngine.Events;

namespace ThunderRoad
{
    public class CrossbowLoader : ThunderBehaviour
    {
        public Item item;
        public Holder sourceHolder;
        [ShowIf(nameof(sourceHolder), null)]
        [ListDrawerSettings(AlwaysExpanded = true)]
        public List<Bolt> bolts = new List<Bolt>();
        public bool infinite = false;
        public float boltLoadDelay = 0f;
        public UnityEvent onLoadBolt;
        public UnityEvent onEmpty;

        public bool isLoading { get; protected set; } = false;

        [System.Serializable]
        [DeclareHorizontalGroup("Horiz")]
        [AutoLabelWidth]
        public class Bolt
        {
            [Group("Horiz")]
            public string id;
            [Group("Horiz")]
            [ReadOnly]
            public Item item;

            public Bolt(string itemId)
            {
                id = itemId;
            }

        }

        private void OnValidate()
        {
            TryGetComponents();
        }

        private void TryGetComponents()
        {
            if (item == null) item = GetComponent<Item>();
        }

        public void SetHolder(Holder holder)
        {
        }

        [Button]
        public void SetContents(string content)
        {
            bolts = new List<Bolt>();
            if (content.IsNullOrEmptyOrWhitespace()) return;
            int amount = 1;
            if (content.Contains('*'))
            {
                amount = int.Parse(content.Split('*')[1]);
                content = content.Split('*')[0];
            }
            for (int i = 0; i < amount; i++)
            {
                bolts.Add(new Bolt(content));
            }
        }

        public void SetInfinite(bool infinite)
        {
            this.infinite = infinite;
        }

    }
}
