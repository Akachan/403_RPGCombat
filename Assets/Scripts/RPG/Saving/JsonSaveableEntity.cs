using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace RPG.Saving
{
    [ExecuteAlways]
    public class JsonSaveableEntity : MonoBehaviour
    {
       [SerializeField] string uniqueIdentifier = "";

        // CACHED STATE
        static Dictionary<string, JsonSaveableEntity> globalLookup = new Dictionary<string, JsonSaveableEntity>();

        public string GetUniqueIdentifier()
        {
            return uniqueIdentifier;
        }
        
        //El objetivo de este método es encontrar todos los scritps del objeto que tenga que guardar informacion y
        //guardar esa informacion en un Diccionario
        public JToken CaptureAsJtoken() 
        {
            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state;              //A diferencia de la version anterior
                                                                        //Se usa Jtoken en lugar de Object
            foreach (IJsonSaveable jsonSaveable in GetComponents<IJsonSaveable>())  
                                                                        //Busca todos los scrips que usen
                                                                        //la interface IJsonSaveable
            {
                JToken token = jsonSaveable.CaptureAsJToken();          //ElJtoken que return del CaptureAsJToken
                string component = jsonSaveable.GetType().ToString();   //Guarda el nombre del componente/script como key
                Debug.Log($"{name} Capture {component} = {token.ToString()}");
                stateDict[jsonSaveable.GetType().ToString()] = token;   //Guarda el Jtoken a nombre del componente
            }
            return state;
        }

        //El objeetivo es encontrar todos los scripts del objeto  que tengan que restituir informacion y recuperar
        //desde el diccionario los datos para restituirlos
        public void RestoreFromJToken(JToken s) 
        {
            JObject state = s.ToObject<JObject>();
            IDictionary<string, JToken> stateDict = state;
            foreach (IJsonSaveable jsonSaveable in GetComponents<IJsonSaveable>())
                                                                        //Busca todos los scrips que usen
                                                                        //la interface IJsonSaveable
            {
                string component = jsonSaveable.GetType().ToString();   //Guarda el nombre del script
                if (stateDict.ContainsKey(component))                   //Se fija si existe en el diccionario
                {

                    Debug.Log($"{name} Restore {component} =>{stateDict[component].ToString()}");
                    jsonSaveable.RestoreFromJToken(stateDict[component]);//Le manda el JToken para que el Script restituya
                                                                        //Los datos
                }
            }
        }

    #if UNITY_EDITOR
        private void Update() {
            if (Application.IsPlaying(gameObject)) return;
            if (string.IsNullOrEmpty(gameObject.scene.path)) return;

            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty property = serializedObject.FindProperty("uniqueIdentifier");
            
            if (string.IsNullOrEmpty(property.stringValue) || !IsUnique(property.stringValue))
            {
                property.stringValue = System.Guid.NewGuid().ToString();
                serializedObject.ApplyModifiedProperties();
            }

            globalLookup[property.stringValue] = this;
        }
    #endif

        private bool IsUnique(string candidate)
        {
            if (!globalLookup.ContainsKey(candidate)) return true;      //Si no existe es unico

            if (globalLookup[candidate] == this) return true;           //Si es éste, es unico

            if (globalLookup[candidate] == null)                        //Si está vacio, lo removemos y ahora es unico
            {
                globalLookup.Remove(candidate);
                return true;
            }

            if (globalLookup[candidate].GetUniqueIdentifier() != candidate) //Si el su identificador es distinto al candidato
                                                                            //Se borra y es unico
            {
                globalLookup.Remove(candidate);
                return true;
            }

            return false;                                               //no es unico
        }
        
    
 
    }
}