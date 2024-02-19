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

        [Header("Fader")] [SerializeField] private float fadeOutTime = 0.5f;
        [SerializeField] private float fadeWaitTime = 1f;
        [SerializeField] private float fadeInTime = 0.5f;
 
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
            
            Fader fader = FindObjectOfType<Fader>();
            
            print("empezo el fade out");
            yield return fader.FadeOut(fadeOutTime);
            print("termino  el fade out");
            
            
            
            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            
            var otherPortal = GetOtherPortal(); 
            UpdatePlayer(otherPortal);
            
            yield return new WaitForSeconds(fadeWaitTime);
            
            print("empezo el fade in");
            yield return fader.FadeIn(fadeInTime);
            print("termino  el fade in");
            
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