
using RPG.Movement;
using System;
using UnityEngine;
using RPG.Combat;
using RPG.Core;


namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        private Fighter _fighter;
        private Health _health;

        private void Awake()
        {
            _fighter = GetComponent<Fighter>();
            _health = GetComponent<Health>();
        }

        void Update()
        {
            if(_health.IsDead){return;}
            if(InteractWithCombat()) return;
            if(InteractWithMovement()) return;
            print("Nothing to do");
        }   

        private bool InteractWithCombat()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            foreach (RaycastHit hit in hits) 
            {
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();
                if (target == null) { continue; }
                
                if (!_fighter.CanAttack(target.gameObject)) { continue; }
                
                if (Input.GetMouseButtonDown(0))
                {
                    _fighter.Attack(target.gameObject);
                }
                return true;
            }
            return false;
        }

        private bool InteractWithMovement()
        {
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);

            if (hasHit)
            {
                if (Input.GetMouseButton(0))
                {
                    
                    GetComponent<Mover>().StarMoveAction(hit.point,1f);
                }
                return true;     
            }
            return false;
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}
