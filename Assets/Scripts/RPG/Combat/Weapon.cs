using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private UnityEvent onHit = null;
        public void OnHit()
        {
           onHit.Invoke();
        }
    }
}