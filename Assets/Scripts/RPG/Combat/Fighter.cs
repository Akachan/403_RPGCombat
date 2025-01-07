using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Utils;
using Newtonsoft.Json.Linq;
using RPG.Attributes;
using RPG.Movement;
using UnityEngine;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using UnityEngine.Serialization;


namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, IJsonSaveable, IModifierProvider
    {
        [SerializeField] private float timeBetweenAttacks = 1f;
        [SerializeField] private Transform rightHandTransform = null;
        [SerializeField] private Transform leftHandTransform = null;
        [SerializeField] private WeaponConfig defaultWeaponConfig = null;
        
     
        private ActionScheduler _actionScheduler;
        private Health _target;
        private Animator _animator;
        private float _timeSinceLastAttack = Mathf.Infinity;
        private static readonly int Attack1 = Animator.StringToHash("attack");
        private static readonly int StopAttack = Animator.StringToHash("stopAttack");
        private WeaponConfig _currentWeaponConfig = null;
        private LazyValue<Weapon> _currentWeapon;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _actionScheduler = GetComponent<ActionScheduler>();
            _currentWeaponConfig = defaultWeaponConfig;
            _currentWeapon = new LazyValue<Weapon>(GetInitialWeapon);
        }

        private void Start()
        {
            _currentWeapon.ForceInit();
        }

        private Weapon GetInitialWeapon()
        {
            return AttachWeapon(defaultWeaponConfig);
        }
        
        private void Update()
        {
            _timeSinceLastAttack += Time.deltaTime;
            if(_target == null) {return;}
            if(_target.IsDead) {return;}
            
            
            if (!GetIsInRange(_target.transform))
            {
                GetComponent<Mover>().MoveTo(_target.transform.position, 1f);
                
            }
            else
            {
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
            }
        }
        
        public void EquipWeapon(WeaponConfig weaponConfig)
        {
            _currentWeaponConfig = weaponConfig;
            _currentWeapon.value = AttachWeapon(_currentWeaponConfig);
        }

        private Weapon AttachWeapon(WeaponConfig weaponConfig)
        {
           Animator animator = GetComponent<Animator>();
           return weaponConfig.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        public Health GetTarget()
        {
            return _target;
        }

        private void AttackBehaviour()
        {
            if(_timeSinceLastAttack< timeBetweenAttacks){return;}
            
            //This will trigger the HIT() event
            transform.LookAt(_target.transform, Vector3.up);
            TriggerAttackAnimation();
            

            _timeSinceLastAttack = 0f;
        }

        private void TriggerAttackAnimation()
        {
            _animator.ResetTrigger(StopAttack);
            _animator.SetTrigger(Attack1);
        }

        //Animation Event
        private void Hit()
        {
            if(_target == null) {return;}

            var damage = GetComponent<BaseStats>().GetStat(Stat.Damage);

            if (_currentWeapon.value != null)
            {
                _currentWeapon.value.OnHit();
            }
            
            if (_currentWeaponConfig.HasProjectile())
            {
                _currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, _target, gameObject, damage);
            }
            else
            {
                
                _target.TakeDamage(gameObject, damage);
            }
            
        }

        private void Shoot()
        {
            Hit();
        }

        private bool GetIsInRange(Transform target)
        {
            print($"My pos: transform.position: {transform.position}, Enemy pos: {target.position}, Distance: {Vector3.Distance(transform.position, _target.transform.position)}  ");
            return Vector3.Distance(transform.position, target.position) <= _currentWeaponConfig.GetRange();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false;
            
            //print($"combatTarget: {combatTarget.transform.position}, Mover: {GetComponent<Mover>().name} ");
            if (!GetComponent<Mover>().CanMoveTo(combatTarget.transform.position) &&
                !GetIsInRange(combatTarget.transform)) return false;

            Health currentTarget = GetComponent<Health>();
            return currentTarget != null && !currentTarget.IsDead;
        }
        public void Attack(GameObject combatTarget)
        {
            _actionScheduler.StartAction(this);
            _target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            StopAttackAnimation();
            _target = null;
            GetComponent<Mover>().Cancel();
        }

        private void StopAttackAnimation()
        {
            _animator.ResetTrigger(Attack1);
            _animator.SetTrigger(StopAttack);
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return _currentWeaponConfig.GetDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return _currentWeaponConfig.GetPercentageBonus();
            }
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(_currentWeaponConfig.name);
        }

        public void RestoreFromJToken(JToken state)
        {
            var weaponName = state.ToObject<string>();
            WeaponConfig weaponConfig = Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weaponConfig);
        }


        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

}

