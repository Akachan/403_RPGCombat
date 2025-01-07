using System;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace RPG.UI.DamageText
{
    
    public class DamageText: MonoBehaviour
    {
        [SerializeField ]private TextMeshProUGUI textMesh = null;

        private void Awake()
        {
           
        }

        public void SetValue(float damage)
        {
            textMesh.text = $"{damage}";
        }

        private void DestroyText()
        {
            Destroy(gameObject);
        }
    }
}