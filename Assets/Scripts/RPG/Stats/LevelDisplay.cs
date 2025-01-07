using TMPro;
using UnityEngine;

namespace RPG.Stats
{
    public class LevelDisplay : MonoBehaviour
    {
        private BaseStats _baseStats;
        private TextMeshProUGUI _textMesh;

        private void Awake()
        {
            _baseStats = GameObject.FindGameObjectWithTag("Player").GetComponent<BaseStats>();
            _textMesh = GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            _textMesh.text = _baseStats.GetLevel().ToString();
        }
    }
}