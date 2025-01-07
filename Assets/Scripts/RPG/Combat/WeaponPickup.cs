using System;
using System.Collections;
using RPG.Attributes;
using RPG.Control;
using UnityEngine;
using UnityEngine.Serialization;

namespace RPG.Combat
{
    public class WeaponPickup: MonoBehaviour, IRaycastable
    {
        [SerializeField] private WeaponConfig weaponConfig;
        [SerializeField] private float healthToRestore = 0f;
        [SerializeField] private float seconds = 10f;

        
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Pickup(other.gameObject);
            }
            
        }

        private void Pickup(GameObject subject)
        {
            if (weaponConfig != null)
            {
                subject.GetComponent<Fighter>().EquipWeapon(weaponConfig);
            }

            if (healthToRestore > 0)
            {
                subject.GetComponent<Health>().Heal(healthToRestore);
            }
            StartCoroutine(HideForSeconds(seconds));
        }

        IEnumerator HideForSeconds(float seconds)
        {
            ShowPickup(false);
            yield return new WaitForSeconds(seconds);
            ShowPickup(true);
        }

        private void ShowPickup(bool state)
        {
            GetComponent<Collider>().enabled = state;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(state);
            }
        }


        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }

        public bool HandleRaycast(PlayerController cullingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Pickup(cullingController.gameObject);
            }
            return true;
        }
    }
}