using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PatrolPath : MonoBehaviour
{
    [SerializeField] private float waypointGizmoRadius = 0.5f;
    private void Start()
    {
        
    }

    private void OnDrawGizmos()
    {
        
          for (int i = 0; i < transform.childCount; i++)
          {
              Gizmos.DrawSphere(transform.GetChild(i).position, waypointGizmoRadius);
              Gizmos.DrawLine(  transform.GetChild(i).position,
                                transform.GetChild(GetNextIndex(i)).position);
          }
    }

    private int GetNextIndex(int i)
    {
        return i + 1 < transform.childCount ? i + 1 : 0;
    }
}
