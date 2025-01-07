using System;
using RPG.Attributes;
using RPG.Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 8f;
        [SerializeField] private bool isHoming = false;
        [SerializeField] private GameObject hitEffect = null;
        [SerializeField] private float maxLifeTime = 10f;
        [SerializeField] private GameObject[] destroyOnHit = null;
        [SerializeField] private float lifeAfterImpact = 2f;
        [SerializeField] private UnityEvent onLaunch = null;
        [SerializeField] private UnityEvent onHit = null;
        
        private Health _target = null;
        private GameObject _instigator = null;
        private float _damage = 0f;
        
        private void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        private void Update()
        {
            if( _target == null) return;
            if (isHoming && !_target.IsDead)
            {
                 transform.LookAt(GetAimLocation());
            }
            transform.Translate(Vector3.forward * (Time.deltaTime * speed));
            
           
        }

        public void SetTarget(Health target, GameObject instigator, float damage)
        {
            _target = target;
            _damage = damage;
            _instigator = instigator;
            onLaunch.Invoke();
            
            Destroy(gameObject, maxLifeTime);
        }
        private Vector3 GetAimLocation()
        {
            
            CapsuleCollider targetCapsule = _target.GetComponent<CapsuleCollider>();

            var height = targetCapsule == null ? 0f : targetCapsule.height;
            
            return _target.transform.position + Vector3.up * (height/ 2);
            
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() != _target) return;
            
            if(_target.IsDead) return;
            
            _target.TakeDamage(_instigator, _damage);

            speed = 0f;
            
            onHit.Invoke();
            
            if (hitEffect != null)
            {
                Instantiate(hitEffect, GetAimLocation(), transform.rotation);
            }

            foreach (var toDestroy in destroyOnHit)
            {
                Destroy(toDestroy);
            }
            Destroy(gameObject, lifeAfterImpact);
        }
    }
}
