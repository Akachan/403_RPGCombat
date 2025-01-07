using System;
using Unity.VisualScripting;
using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Health health = null;
        [SerializeField] private RectTransform foreground = null;
        [SerializeField] private Canvas rootCanvas = null;
        private void Update()
        {
            if (Mathf.Approximately(health.GetFraction(),0) || 
                Mathf.Approximately(health.GetFraction(),1))
            {
                rootCanvas.enabled = false;
                return;
            }
            
            rootCanvas.enabled = true;
            var scale = foreground.localScale;
            scale.x = health.GetFraction();
            foreground.localScale = scale;
        }
    }
}