using RPG.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;



namespace RPG.Combat
{
    public class Fighter : MonoBehaviour
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
                GetComponent<Mover>().Stop();
                
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

        public void Stop()
        {
            _target = null;
        }

    }

}

