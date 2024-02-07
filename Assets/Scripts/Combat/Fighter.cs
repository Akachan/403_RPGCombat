using RPG.Movement;
using UnityEngine;
using RPG.Core;



namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] private float weaponRange = 2f;
        [SerializeField] private float timeBetweenAttacks = 1f;

        private Transform _target;
        private float timeSinceLastAttack = 0f;
        private static readonly int Attack1 = Animator.StringToHash("attack");

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            if (_target == null) return; 
            if (!GetIsInRange())
            {
                GetComponent<Mover>().MoveTo(_target.position);
            }
            else
            {
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
            }

        }

        private void AttackBehaviour()
        {
            if(timeSinceLastAttack< timeBetweenAttacks){return;}
            GetComponent<Animator>().SetTrigger(Attack1);
            timeSinceLastAttack = 0f;
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, _target.position) <= weaponRange;
        }

        public void Attack(CombatTarget combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            _target = combatTarget.transform;
        }

        public void Cancel()
        {
            _target = null;
        }
        
        //Animation Event
        private void Hit()
        {
            
        }


    }

}

