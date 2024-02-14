using System;
using System.Linq.Expressions;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine;
using UnityEngine.Serialization;

namespace RPG.Control
{
    public class AiController: MonoBehaviour
    {
        [SerializeField] private float chaseDistance = 5f;
        [SerializeField] private float suspicionTime = 10f;
        [SerializeField] private float waypointDwellTime = 3f;
        [SerializeField] private PatrolPath patrolPath;
        [SerializeField] private float waypointTolerance = 1f;      
        
        private Vector3 _guardPosition;
        private int _currentWaypointIndex= 0;
        private float _timeSinceLastSawPlayer = Mathf.Infinity;
        private float _timeSinceArriveToWaypoint = Mathf.Infinity;
        
        private ActionScheduler _actionScheduler;
        private GameObject _target;
        private Fighter _fighter;
        private Health _health;
        private Mover _mover;

        private void Awake()
        {
            _actionScheduler = GetComponent<ActionScheduler>();
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
                AttackBehaviour();
            }
            else if (_timeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();
            } 

            UpdateTimers();
        }

        private void UpdateTimers()
        {
            _timeSinceLastSawPlayer += Time.deltaTime;
            _timeSinceArriveToWaypoint += Time.deltaTime;
        }

        private void PatrolBehaviour()
        {
            var nextPosition = _guardPosition;

            if (patrolPath != null)
            {
                if (AtWaypoint())
                {
                    _timeSinceArriveToWaypoint = 0f;
                    CycleWaypoint();
                }

                nextPosition = GetCurrentWayPoint();
            }
            if (_timeSinceArriveToWaypoint > waypointDwellTime)
            {
                _mover.StarMoveAction(nextPosition);
            }
        }

        private Vector3 GetCurrentWayPoint()
        {
            return patrolPath.GetWaypoint(_currentWaypointIndex);
        }

        private void CycleWaypoint()
        {
            _currentWaypointIndex = patrolPath.GetNextIndex(_currentWaypointIndex);
        }

        private bool AtWaypoint()
        {

            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWayPoint());
            return distanceToWaypoint < waypointTolerance;
        }
        

        private void SuspicionBehaviour()
        {
            _actionScheduler.CancelCurrentAction();
        }

        private void AttackBehaviour()
        {
            _timeSinceLastSawPlayer = 0f;
            _fighter.Attack(_target.gameObject);
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