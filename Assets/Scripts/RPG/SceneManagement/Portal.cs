using System;
using System.Collections;
using RPG.Control;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        [SerializeField] private int sceneToLoad;
        [field: SerializeField] public Transform SpawnPoint { get; private set; }
 
        private void OnTriggerEnter(Collider other)
        {
            if(!other.CompareTag("Player")) {return;}

            StartCoroutine(Transition());
        }

        private IEnumerator Transition()
        {
            DontDestroyOnLoad(gameObject);
            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            
            var otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);
            
            Destroy(gameObject);
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            if (otherPortal == null ) {return;}
            
            var player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().Warp(otherPortal.SpawnPoint.position);
            player.transform.rotation = otherPortal.SpawnPoint.rotation;

        }

        private Portal GetOtherPortal()
        {
            var portals = FindObjectsOfType<Portal>();
            foreach (var portal in portals)
            {
                if (portal != this)
                {
                    return portal;
                }
            }

            return null;
            
        }
    }
}