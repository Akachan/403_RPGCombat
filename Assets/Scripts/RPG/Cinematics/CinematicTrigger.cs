using System;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour
    {
        private bool _isPlayed;
        private void OnTriggerEnter(Collider other)
        {
            if(_isPlayed) {return;}
            if (!other.CompareTag("Player")) { return;}
            
            GetComponent<PlayableDirector>().Play();
            _isPlayed = true;
        }
    }
}
