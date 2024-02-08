using System;
using RPG.Combat;
using UnityEngine;

namespace RPG.Control
{
    public class AiController: MonoBehaviour
    {
        [SerializeField] private float chaseDistance = 5f;
        private GameObject _target;
        private Fighter _fighter;

        private void Start()
        {
            _target = GameObject.FindWithTag("Player");
            _fighter = GetComponent<Fighter>();
        }

        private void Update()
        {
            if (InAttackRange()  && _fighter.CanAttack(_target))
            {
                _fighter.Attack(_target.gameObject);
            }
            else
            {
                _fighter.Cancel();
            }
        }

        private bool InAttackRange()
        {
          var distance = Vector3.Distance(_target.transform.position, transform.position);
          return distance < chaseDistance;
        }


    }
}