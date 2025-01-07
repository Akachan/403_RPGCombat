using RPG.Stats;
using TMPro;
using UnityEngine;


namespace RPG.Attributes
{
    public class ExperienceDisplay : MonoBehaviour
    {
        private Experience _experience;
        private TextMeshProUGUI _textMesh;

        private void Awake()
        {
            _experience = GameObject.FindGameObjectWithTag("Player").GetComponent<Experience>();
            _textMesh = GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            _textMesh.text = _experience.GetExperience().ToString();
        }
    }
}