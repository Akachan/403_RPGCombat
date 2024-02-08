using System;
using System.Linq.Expressions;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Control
{
    public class AiController: MonoBehaviour
    {
        [SerializeField] private float chaseDistance = 5f;
        private Vector3 _guardPosition;
        private GameObject _target;
        private Fighter _fighter;
        private Health _health;
        private Mover _mover;

        private void Awake()
        {
            _target = GameObject.FindWithTag("Player");
            _fighter = GetComponent<Fighter>();
            _health = GetComponent<Health>();
            _mover = GetComponent<Mover>();
        }

        private void Start()
        {
            _guardPosition = transform.position;
        }

        private void Update()
        {
            if(_health.IsDead){return;}
            
            if (InAttackRange()  && _fighter.CanAttack(_target))
            {
                _fighter.Attack(_target.gameObject);
            }
            else
            {
                _mover.StarMoveAction(_guardPosition);
                //_fighter.Cancel();
            }
        }

        private bool InAttackRange()
        {
          var distance = Vector3.Distance(_target.transform.position, transform.position);
          return distance < chaseDistance;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}