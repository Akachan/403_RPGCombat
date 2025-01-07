using RPG.Attributes;
using RPG.Control;
using RPG.Core;
using UnityEngine;
namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }

        public bool HandleRaycast(PlayerController cullingController)
        {

            if (!cullingController.GetComponent<Fighter>().CanAttack(gameObject))
            {
                return false; 
            }
                
            if (Input.GetMouseButtonDown(0))
            {
                
                cullingController.GetComponent<Fighter>().Attack(gameObject);
            }
            
            return true;
        }
    }
}

