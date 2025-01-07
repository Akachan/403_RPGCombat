using System;
using RPG.Control;
using RPG.Core;
using UnityEngine;
using UnityEngine.Playables;


namespace RPG.Cinematics
{
    public class CinematicsControlRemover : MonoBehaviour
    {
        private GameObject _player;
        PlayableDirector _director;
        private void Awake()
        {
            _player = GameObject.FindWithTag("Player");
            _director = GetComponent<PlayableDirector>();
       
        }

        private void OnEnable()
        {
            _director.played += DisableControl;
            _director.stopped += EnableControl;
        }

        private void OnDisable()
        {
            _director.played -= DisableControl;
            _director.stopped -= EnableControl;
        }

        private void DisableControl(PlayableDirector pd)
        {
            
            _player.GetComponent<ActionScheduler>().CancelCurrentAction();
            _player.GetComponent<PlayerController>().enabled = false;
        }

        private void EnableControl(PlayableDirector pd)
        {
            print("EnableControl");
            _player.GetComponent<PlayerController>().enabled = true;
        }
    }
}