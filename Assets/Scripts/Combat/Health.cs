using System;
using UnityEngine;

namespace RPG.Combat
{
    public class Health: MonoBehaviour
    {
        [SerializeField] private float health =100f;
        private float _currentHealth;
        private bool isDead;
        private static readonly int Death = Animator.StringToHash("death");

        private void Start()
        {
            _currentHealth = health;
        }

        public void TakeDamage(float damage)
        {
            _currentHealth = Mathf.Max(_currentHealth - damage, 0);
            print($"Enemy Health: {_currentHealth}");

            if (_currentHealth != 0 || isDead) return;
            GetComponent<Animator>().SetTrigger(Death);
            isDead = true;

        }
    }
}