using TMPro;
using UnityEngine;

namespace RPG.Attributes
{
    public class HealthDisplay : MonoBehaviour
    {
        private Health _health;
        private TextMeshProUGUI _textMesh;

        private void Awake()
        {
            _health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();    
            _textMesh = GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            _textMesh.text = $"{_health.GetHealthPoints()} / {_health.MaxHealthPoints()}";

        }
    }
}