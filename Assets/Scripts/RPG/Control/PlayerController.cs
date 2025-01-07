
using RPG.Movement;
using System;
using RPG.Attributes;
using UnityEngine;
using RPG.Combat;
using RPG.Core;
using UnityEngine.AI;
using UnityEngine.EventSystems;


namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private CursorMapping[] cursorMappings;
        [SerializeField] private float maxNavMeshProjectionDistance = 1f;
        [SerializeField] private float sphereRadius = 1f;
        
        private Health _health;
        


        [Serializable]
        struct CursorMapping
        {
            public CursorType cursorType;
            public Texture2D texture;
            public Vector2 hotspot;

        }

        private void Awake()
        {
            
            _health = GetComponent<Health>();
        }

        void Update()
        {
            if (InteractWithUI()) return;
            
            if (_health.IsDead)
            {
                SetCursor(CursorType.None); 
                return;
            }
            if(InteractWithComponent())return;
            
            if(InteractWithMovement()) return;
            
            SetCursor(CursorType.None);
  
        }

     

        private bool InteractWithUI()
        {
            if(EventSystem.current.IsPointerOverGameObject()) //sobre UI
            {
                SetCursor(CursorType.UI);
                return true;
            }
            return false;
        }
        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSorted();

            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (var raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        private  RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), sphereRadius);
            
            float[] distances = new float[hits.Length];
            for (int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }
            Array.Sort(distances, hits);
            return hits ;
        }


        private bool InteractWithMovement()
        {
        
            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);
            if (hasHit)
            {
                if (!GetComponent<Mover>().CanMoveTo(target)) return false;
                if (Input.GetMouseButton(0))
                {
                    
                    GetComponent<Mover>().StarMoveAction(target,1f);
                }
                SetCursor(CursorType.Movement);
                return true;     
                
            }
            return false;
        }

        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();
            
            
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
         
            if (!hasHit) return false;
            
            NavMeshHit navMeshHit;
            bool hasCastToNaveMesh =NavMesh.SamplePosition(hit.point, out navMeshHit, 
                                    maxNavMeshProjectionDistance, 
                                    NavMesh.AllAreas);
       
            if (!hasCastToNaveMesh) return false;
            
            target = navMeshHit.position;
            
            return true;
        }

        

        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType cursorType)
        {
            foreach (var cursorMapping in cursorMappings)
            {
                if(cursorMapping.cursorType != cursorType) continue;
                return cursorMapping;
            }
            return cursorMappings[0];
        }
        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}
