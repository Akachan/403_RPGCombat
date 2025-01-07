using System;
using Newtonsoft.Json.Linq;
using RPG.Saving;
using Unity.VisualScripting;
using UnityEngine;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, IJsonSaveable
    {
        [SerializeField] private float experiencePoints = 0f;
        public event Action OnExperienceGained;
        

        public void GainExperience(float experience)
        {
            experiencePoints += experience;
            if (OnExperienceGained != null) OnExperienceGained();
        }

        public float GetExperience()
        {
            return experiencePoints;
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(experiencePoints);
        }

        public void RestoreFromJToken(JToken state)
        {
            experiencePoints = state.ToObject<float>();
        }
    }
}