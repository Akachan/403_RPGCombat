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
        public enum DestinationIdentifier
        {
         A, B, C, D   
        }
        
        [SerializeField] private int sceneToLoad =-1;
        [field: SerializeField] public Transform SpawnPoint { get; private set; }
        [field: SerializeField] public DestinationIdentifier destination;
 
        private void OnTriggerEnter(Collider other)
        {
            if(!other.CompareTag("Player")) {return;}

            StartCoroutine(Transition());
        }

        private IEnumerator Transition()
        {
            if (sceneToLoad < 0)
            {
                Debug.LogError("Scene to Load not set yet");
                yield break;
            }
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
                if (portal == this) {continue;}
                if (portal.destination == destination)
                {
                    print($"El sale del portal {destination} y va al portal {portal.destination}");
                    return portal;
                }
            }
            print("no se encontró portal igual al destino");
            return null;
            
        }
    }
}