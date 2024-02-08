using System;
using UnityEngine;

namespace RPG.Control
{
    public class AiController: MonoBehaviour
    {
        [SerializeField] private float chaseDistance = 5f;

        private void Update()
        {
            if (DistanceToPlayer() < chaseDistance)
            {
                print($"{gameObject.name} esta persiguiendo a player");
            }
        }

        private float DistanceToPlayer()
        {
            GameObject player = GameObject.FindWithTag("Player");
            float distance = Vector3.Distance(player.transform.position, transform.position);
            return distance;
        }
    }
}