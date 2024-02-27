
using RPG.Core;
using RPG.Saving;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] private float maxSpeed = 6f;
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


        public object CaptureState()
        {
            return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state)
        {
            SerializableVector3 position = (SerializableVector3)state;
            _navMeshAgent.enabled = false;
            transform.position = position.ToVector();
            _navMeshAgent.enabled = true;
        }
    }
}
 