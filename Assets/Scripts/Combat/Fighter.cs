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

        private Health _target;
        private float timeSinceLastAttack = 0f;
        private static readonly int Attack1 = Animator.StringToHash("attack");
        private static readonly int StopAttack = Animator.StringToHash("stopAttack");

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            if (_target == null) return; 
            if (_target.IsDead){return;}
            
            if (!GetIsInRange())
            {
                GetComponent<Mover>().MoveTo(_target.transform.position);
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
            //This will trigger the HIT() event
            transform.LookAt(_target.transform, Vector3.up);
            GetComponent<Animator>().SetTrigger(Attack1);
            timeSinceLastAttack = 0f;
            
           
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

        public void Attack(CombatTarget combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            _target = combatTarget.GetComponent<Health>();
            
            
        }

        public void Cancel()
        {
            GetComponent<Animator>().SetTrigger(StopAttack);
            _target = null;
        }
        
   


    }

}

