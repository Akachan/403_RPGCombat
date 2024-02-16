using System;
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

            SceneManager.LoadScene(sceneToLoad);
        }
    }
}