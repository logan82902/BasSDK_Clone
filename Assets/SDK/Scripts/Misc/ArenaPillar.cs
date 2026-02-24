using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThunderRoad
{
    public class ArenaPillar : MonoBehaviour
    {
        public List<Animator> rewardPillarAnimators;
        public AudioContainer raiseSound;
        public AudioContainer lowerSound;
        public AudioSource rewardPillarSound;
        public GameObject rewardPillarFX;
        public Transform spawnPoint;
        public Transform tpPosition;
        public float maxItemRadius = 10f;

        public void Init(ISurvivalModule survivalModule)
        {
            if (survivalModule.TryGetPillarSounds(out var raise, out var lower))
            {
                raiseSound = raise;
                lowerSound = lower;
            }

            if (survivalModule.TryGetTeleportPosition(out var teleportPoint))
            {
                tpPosition.SetPositionAndRotation(teleportPoint,
                    Quaternion.LookRotation(transform.position - teleportPoint));
            }
        }
    }
}