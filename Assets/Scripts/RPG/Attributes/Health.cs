using System;
using GameDevTV.Utils;
using Newtonsoft.Json.Linq;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health: MonoBehaviour, IJsonSaveable
    {
        [SerializeField] private float percentageToRegenerate = 70f;
        [SerializeField] private UnityEvent<float> takeDamage;
        [SerializeField] private UnityEvent onDie;
        LazyValue<float> _currentHealth;
        
        private BaseStats _baseStats;
        private bool _isDead;
        private static readonly int Death = Animator.StringToHash("death");
        

        public bool IsDead => _isDead;

        private void Awake()
        {
            _baseStats = GetComponent<BaseStats>();
            _currentHealth = new LazyValue<float>(GetInitialHealth);
        }

        private void Start()
        {
            _currentHealth.ForceInit();
        }

        private float GetInitialHealth()
        {
            return _baseStats.GetStat(Stat.Health);
        }

        private void OnEnable()
        {
            _baseStats.OnLevelUp += RestoreHealth;
        }

        private void OnDisable()
        {
            _baseStats.OnLevelUp -= RestoreHealth;
        }

        private void RestoreHealth()
        {
            var healthToRestore =_baseStats.GetStat(Stat.Health) * percentageToRegenerate / 100;
            _currentHealth.value = Mathf.Max(_currentHealth.value, healthToRestore);
        }
        

        public float GetPercentage()
        {
            return GetFraction() * 100f;
        }

        public float GetFraction()
        {
            return _currentHealth.value / _baseStats.GetStat(Stat.Health);
        }

        public float GetHealthPoints()
        {
            return _currentHealth.value;
        }

        public float MaxHealthPoints()
        {
            return _baseStats.GetStat(Stat.Health);
        }
        public void TakeDamage(GameObject instigator, float damage)
        {
            
           
            _currentHealth.value = Mathf.Max( _currentHealth.value - damage, 0f);
            takeDamage.Invoke(damage);
            if (_currentHealth.value != 0 || _isDead) return;
            Die();
            AwardExperience(instigator);
            
        }
        
        private void Die()
        {
            onDie.Invoke();
            _isDead = true;
            GetComponent<ActionScheduler>().CancelCurrentAction();
            GetComponent<Animator>().SetTrigger(Death);
            
        }
        private void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if (experience == null) return;
            
            float xp = _baseStats.GetStat(Stat.ExperienceReward);
            experience.GainExperience(xp);
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(_currentHealth.value);
        }

        public void RestoreFromJToken(JToken state)
        {
            _currentHealth.value = state.ToObject<float>();
            if (_currentHealth.value <= 0)
            {
                Die();
            }

        }

        public void Heal(float healthToRestore)
        {
            _currentHealth.value = Mathf.Min(_currentHealth.value + healthToRestore, MaxHealthPoints());
        }
    }
}