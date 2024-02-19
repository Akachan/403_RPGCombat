using System;
using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            StartCoroutine(FadeInOut());
        }

        public IEnumerator FadeInOut()
        {
            yield return FadeOut(3f);
            print("fade out");
            yield return FadeIn(1f);
            print("fade in");
        }

        public IEnumerator FadeOut(float time)
        {
            while (_canvasGroup.alpha < 1f)
            {
                _canvasGroup.alpha += Time.deltaTime / time;
                
            }
            yield return null;
        }
        public IEnumerator FadeIn(float time)
        {
            while (_canvasGroup.alpha > 0f)
            {
                _canvasGroup.alpha -= Time.deltaTime / time;
                yield return null;
            }
        }
    }
}