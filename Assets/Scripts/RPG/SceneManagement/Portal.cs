using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        [SerializeField] private int sceneToLoad;

 
        private void OnTriggerEnter(Collider other)
        {
            if(!other.CompareTag("Player")) {return;}

            StartCoroutine(Transition());
        }

        private IEnumerator Transition()
        {
            DontDestroyOnLoad(gameObject);
            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            print("scene loaded");
            Destroy(gameObject);
        }
    }
}