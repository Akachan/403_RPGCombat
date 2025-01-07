using System;
using System.Collections;
using RPG.Saving;
using UnityEngine;


namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        private const string DefaultSaveFile = "saving";
        private float _fadeInTime = 1f;


        private void Awake()
        {
            StartCoroutine(LoadLastScene());
        }

        private IEnumerator LoadLastScene()
        {
            yield return GetComponent<JsonSavingSystem>().LoadLastScene(DefaultSaveFile);
            var fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();
            yield return fader.FadeIn(_fadeInTime);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                Delete();
            }
        }

        public void Save()
        {
            GetComponent<JsonSavingSystem>().Save(DefaultSaveFile);
        }

        public void Load()
        {
            GetComponent<JsonSavingSystem>().Load(DefaultSaveFile);
        }

        public void Delete()
        {
            GetComponent<JsonSavingSystem>().Delete(DefaultSaveFile);
        }
    }
}
