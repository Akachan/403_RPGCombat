using RPG.Movement;
using UnityEngine;
using RPG.Core;



namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] private float weaponRange = 2f;

        Transform _target;
        private void Update()
        {
            if (_target == null) return; 
            if (!GetIsInRange())
            {
                GetComponent<Mover>().MoveTo(_target.position);
                

            }
            else
            {
                GetComponent<Mover>().Cancel();
                
            }

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


    }

}

