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
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            
            //remove control this player
            PlayerController playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            playerController.enabled = false;
            
            yield return fader.FadeOut(fadeOutTime);
            
            savingWrapper.Save();
            
            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            
            //remove control other player
            PlayerController newPlayerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            newPlayerController.enabled = false;
            
            savingWrapper.Load();
            
            var otherPortal = GetOtherPortal(); 
            UpdatePlayer(otherPortal);
            
            savingWrapper.Save();
            
            yield return new WaitForSeconds(fadeWaitTime);
            fader.FadeIn(fadeInTime);

            //restore control
            newPlayerController.enabled = true;
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