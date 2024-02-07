using System;
using UnityEngine;

namespace RPG.Combat
{
    public class Health: MonoBehaviour
    {
        [SerializeField] private float health =100f;
        private float _currentHealth;

        private void Start()
        {
            _currentHealth = health;
        }

        public void TakeDamage(float damage)
        {
            _currentHealth = Mathf.Max(_currentHealth - damage, 0);
            print($"Enemy Health: {_currentHealth}");
        }
    }
}