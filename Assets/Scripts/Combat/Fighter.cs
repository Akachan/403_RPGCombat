using System;
using RPG.Movement;
using UnityEngine;
using RPG.Core;



namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] private float weaponRange = 2f;
        [SerializeField] private float timeBetweenAttacks = 1f;
        [SerializeField] private float weaponDamage = 5f;

        private ActionScheduler _actionScheduler;
        private Health _target;
        private Animator _animator;
        private float _timeSinceLastAttack = Mathf.Infinity;
        private static readonly int Attack1 = Animator.StringToHash("attack");
        private static readonly int StopAttack = Animator.StringToHash("stopAttack");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _actionScheduler = GetComponent<ActionScheduler>();
        }

        private void Update()
        {
            _timeSinceLastAttack += Time.deltaTime;
            if(_target == null) {return;}
            if(_target.IsDead) {return;}
            
            if (!GetIsInRange())
            {
                GetComponent<Mover>().MoveTo(_target.transform.position, 1f);
            }
            else
            {
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
            }
        }

        private void AttackBehaviour()
        {
            if(_timeSinceLastAttack< timeBetweenAttacks){return;}
            
            //This will trigger the HIT() event
            transform.LookAt(_target.transform, Vector3.up);
            TriggerAttackAnimation();

            _timeSinceLastAttack = 0f;
        }

        private void TriggerAttackAnimation()
        {
            _animator.ResetTrigger(StopAttack);
            _animator.SetTrigger(Attack1);
        }

        //Animation Event
        private void Hit()
        {
            if(_target == null) {return;}
            _target.TakeDamage(weaponDamage);
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, _target.transform.position) <= weaponRange;
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) { return false; }

            Health currentTarget = GetComponent<Health>();
            return currentTarget != null && !currentTarget.IsDead;
        }
        public void Attack(GameObject combatTarget)
        {
            _actionScheduler.StartAction(this);
            _target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            StopAttackAnimation();
            _target = null;
        }

        private void StopAttackAnimation()
        {
            _animator.ResetTrigger(Attack1);
            _animator.SetTrigger(StopAttack);
        }
    }

}

