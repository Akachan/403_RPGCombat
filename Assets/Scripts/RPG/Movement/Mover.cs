
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPG.Attributes;
using RPG.Core;
using RPG.Saving;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, IJsonSaveable
    {
        [SerializeField] private float maxSpeed = 6f;
        [SerializeField] private float maxNavMeshPathLength = 30f;
        private ActionScheduler _actionScheduler;
        private NavMeshAgent _navMeshAgent;
        private Animator _animator;
        private Health _health;
        private static readonly int ForwardSpeed = Animator.StringToHash("forwardSpeed");

        private void Awake()
        {
            _actionScheduler = GetComponent<ActionScheduler>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
            _health = GetComponent<Health>();

        }


        void Update()
        {
            _navMeshAgent.enabled = !_health.IsDead;
            UpdateAnimator();
        }

        private void UpdateAnimator()
        {
            Vector3 localVelocity = transform.InverseTransformDirection(_navMeshAgent.velocity);
            _animator.SetFloat(ForwardSpeed, localVelocity.z);
        }
        public void StarMoveAction(Vector3 destination, float speedFraction)
        {
            _actionScheduler.StartAction(this);
           
            MoveTo(destination, speedFraction);
        }

        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, 
                NavMesh.AllAreas, path);
            if(!hasPath) return false;
            if (path.status != NavMeshPathStatus.PathComplete) return false;
            if(GetPathLength(path) > maxNavMeshPathLength) return false;
            return true;
        }
        private float GetPathLength(NavMeshPath path)
        {
            var pathLength = 0f;
            if (path.corners.Length < 2) return pathLength;
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                pathLength += Vector3.Distance(path.corners[i], path.corners[i + 1]); 
            }
            return pathLength; 
        }
        public void MoveTo(Vector3 destination, float speedFraction)
        {
            _navMeshAgent.destination = destination;
            _navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            _navMeshAgent.isStopped = false;
        }
        public void Cancel()
        {
            _navMeshAgent.isStopped = true;
        }

        // [System.Serializable]
        // struct MoverSaveData
        // {
        //     public SerializableVector3 position;
        //     public SerializableVector3 rotation;
        // }
        // public object CaptureState()
        // {
        //     MoverSaveData data = new MoverSaveData();
        //     data.position = new SerializableVector3(transform.position);
        //     data.rotation= new SerializableVector3(transform.eulerAngles);
        //     return data;
        // }
        //
        // public void RestoreState(object state)
        // {
        //     MoverSaveData data = new MoverSaveData();
        //     data = (MoverSaveData)state;
        //     
        //     _navMeshAgent.enabled = false;
        //     transform.position = data.position.ToVector();
        //     _navMeshAgent.enabled = true;
        //    
        //     transform.eulerAngles =data.rotation.ToVector();
        // }

        public JToken CaptureAsJToken()
        {
            return transform.position.ToToken();
        }

        public void RestoreFromJToken(JToken state)
        {
            _navMeshAgent.enabled = false;
            transform.position = state.ToVector3();
            _navMeshAgent.enabled = true;
            GetComponent<ActionScheduler>().CancelCurrentAction();
            

        }
    }
}
 