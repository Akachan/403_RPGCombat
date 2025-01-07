using RPG.Attributes;
using TMPro;
using UnityEngine;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        private Fighter _fighter;
     
        private TextMeshProUGUI _textMesh;

        private void Awake()
        {
            _fighter = GameObject.FindGameObjectWithTag("Player").GetComponent<Fighter>();    
            _textMesh = GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            Health health = _fighter.GetTarget();
            _textMesh.text = health != null ? $"{health.GetHealthPoints()} / {health.MaxHealthPoints()}" : "N/A";
        }
    }
}