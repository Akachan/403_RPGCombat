using System;
using UnityEngine;

namespace RPG.Core
{
    public class Health: MonoBehaviour
    {
        [SerializeField] private float health = 100f;
        private float _currentHealth;
        private bool _isDead;
        private static readonly int Death = Animator.StringToHash("death");

        public bool IsDead => _isDead;
        private void Start()
        {
            _currentHealth = health;
        }

        public void TakeDamage(float damage)
        {
            _currentHealth = Mathf.Max(_currentHealth - damage, 0);
            print($"Enemy Health: {_currentHealth}");

            if (_currentHealth != 0 || _isDead) return;
            Die();
        }

        private void Die()
        {
            _isDead = true;
            GetComponent<ActionScheduler>().CancelCurrentAction();
            GetComponent<Animator>().SetTrigger(Death);
            
        }
    }
}