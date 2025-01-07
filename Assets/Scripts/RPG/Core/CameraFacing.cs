using System;
using System.Linq;
using UnityEngine;

namespace RPG.Core
{
    public class CameraFacing: MonoBehaviour
    {
        private Camera cam;

        private void Awake()
        {
            cam = Camera.main;
        }

        private void LateUpdate()
        {
            
            transform.forward = cam.transform.forward;
        }
    }
}