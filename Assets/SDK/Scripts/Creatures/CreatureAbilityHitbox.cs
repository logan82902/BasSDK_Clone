using System;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;

namespace ThunderRoad
{
    public class CreatureAbilityHitbox : MonoBehaviour
    {
        public bool forcer = true;
        public bool damager = true;
        [TableList]
        public List<InflictedStatus> applyStatuses = new();

        [System.Serializable]
        public class InflictedStatus
        {
            private TriDropdownList<string> GetAllStatuses => Catalog.GetDropdownAllID<StatusData>();

            [Dropdown(nameof(GetAllStatuses))]
            public string data;
            public float duration = 3f;
        }

        [NonSerialized]
        public Collider hitCollider;
        public bool active => hitCollider.enabled;

        protected Creature creature;
        protected CreatureAbility ability;

        protected static Dictionary<Creature, List<PhysicBody>> affectedBodies = new Dictionary<Creature, List<PhysicBody>>();
        protected static Dictionary<Creature, List<Creature>> damagedCreatures = new Dictionary<Creature, List<Creature>>();


        // Start is called before the first frame update
        void Start()
        {
            hitCollider = GetComponent<Collider>();
            hitCollider.enabled = false;
            creature = GetComponentInParent<Creature>();
        }

        private void CheckInitCreatureAffectedList<T>(Dictionary<Creature, T> dict) where T : new()
        {
            if (dict?.ContainsKey(creature) != true)
            {
                if (dict == null) dict = new();
                dict.Add(creature, new());
            }
        }

        public void EnableHitBox(CreatureAbility ability)
        {
            this.ability = ability;
            hitCollider.enabled = true;
            CheckInitCreatureAffectedList(affectedBodies);
            CheckInitCreatureAffectedList(damagedCreatures);
        }

        public void DisableHitBox()
        {
            ability = null;
            hitCollider.enabled = false;
            affectedBodies.Clear();
            damagedCreatures.Clear();
        }

        private void OnTriggerEnter(Collider other)
        {
        }

        private void OnTriggerExit(Collider other)
        {
        }
    }
}