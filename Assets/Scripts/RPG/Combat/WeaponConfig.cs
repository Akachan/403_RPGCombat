
using RPG.Attributes;
using RPG.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Meke New Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField] private AnimatorOverrideController animatorOverrideController = null;
        [SerializeField] private Weapon equippedPrefab = null;
        [SerializeField] private float weaponRange = 2f;
        [SerializeField] private float weaponDamage = 5f;
        [SerializeField] private float percentageBonus = 0;
        [SerializeField] private bool isRightHanded = true;
        [SerializeField] private Projectile projectile = null;
       

        private const string WeaponName = "Weapon"; 
        public Weapon Spawn(Transform rightHand,Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);
            Weapon weapon = null;
            if (equippedPrefab != null)
            {
                var handTransform = GetTransform(rightHand, leftHand);
                weapon = Instantiate(equippedPrefab, handTransform);
                weapon.gameObject.name = WeaponName;
                return weapon;
            }

            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (animatorOverrideController != null)
            {
                animator.runtimeAnimatorController = animatorOverrideController;
            }
            else if (overrideController != null)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }

            return weapon;
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(WeaponName);
            if (oldWeapon == null)
            {
                oldWeapon = leftHand.Find(WeaponName);
            }
            if(oldWeapon == null) return;
            
            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);
        }

        private Transform GetTransform(Transform rightHand, Transform leftHand)
        {
            Transform handTransform = isRightHanded ? rightHand : leftHand;
            return handTransform;
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, 
                                        GameObject instigator, float calculateDamage)
        {
            Projectile projectileInstance = Instantiate(projectile, 
                                                        GetTransform(rightHand, leftHand).position, 
                                                        Quaternion.identity);
            projectileInstance.SetTarget(target, instigator, calculateDamage);
        } 
        public float GetRange()
        {
            return weaponRange;
        }

        public float GetDamage()
        {
            return weaponDamage;
        }

        public float GetPercentageBonus()
        {
            return percentageBonus;
        }
    }
}
