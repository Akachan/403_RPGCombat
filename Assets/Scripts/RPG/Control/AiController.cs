﻿using System;
using System.Linq.Expressions;
using GameDevTV.Utils;
using RPG.Attributes;
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
        [SerializeField] private float aggroCooldownTime = 5f;
        [SerializeField] private PatrolPath patrolPath;
        [SerializeField] private float waypointTolerance = 1f;      
        [SerializeField] private float waypointDwellTime = 3f;
        [SerializeField] [Range(0f,1f)] private float patrolSpeedFraction = 0.2f;
        [SerializeField] private float shoutDistance = 3f;
        
        private LazyValue<Vector3> _guardPosition;
        private int _currentWaypointIndex= 0;
        private float _timeSinceLastSawPlayer = Mathf.Infinity;
        private float _timeSinceArriveToWaypoint = Mathf.Infinity;
        private float _timeSinceAggrevated = Mathf.Infinity;
        
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
            _guardPosition = new LazyValue<Vector3>(GetInitialPosition);
        }

        private void Start()
        {
            _guardPosition.ForceInit();
        }

        private Vector3 GetInitialPosition()
        {
            return transform.position;
        }

        private void Update()
        {
            if(_health.IsDead){return;}
            
            
            if (IsAggravated()  && _fighter.CanAttack(_target))
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

        public void Aggravate()
        {
            _timeSinceAggrevated = 0f;
        }
        private void UpdateTimers()
        {
            _timeSinceLastSawPlayer += Time.deltaTime;
            _timeSinceArriveToWaypoint += Time.deltaTime;
            _timeSinceAggrevated += Time.deltaTime;
        }

        private void PatrolBehaviour()
        {
            var nextPosition = _guardPosition.value;

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
                _mover.StarMoveAction(nextPosition, patrolSpeedFraction);
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

            AggrevateNearbyEnemies();
        }

        private void AggrevateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);
            foreach (var hit in hits)
            {
                AiController ai =hit.collider.GetComponent<AiController>();
                if(ai == null) continue;
                
                ai.Aggravate();
                
            }
        
        }

        private bool IsAggravated()
        {
          var distance = Vector3.Distance(_target.transform.position, transform.position);
          return distance < chaseDistance || _timeSinceAggrevated < aggroCooldownTime;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}