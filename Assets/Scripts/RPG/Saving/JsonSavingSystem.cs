using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace RPG.Saving
{
    public class JsonSavingSystem : MonoBehaviour
    {
          private const string extension = ".json";
        
        /// <summary>
        /// Will load the last scene that was saved and restore the state. This
        /// must be run as a coroutine.
        /// </summary>
        /// <param name="saveFile">The save file to consult for loading.</param>
        public IEnumerator LoadLastScene(string saveFile)               //Carga la ultima escena
        {
            JObject state = LoadJsonFromFile(saveFile);                 //Carga el archivo
            IDictionary<string, JToken> stateDict = state;              //Lo guarda en el dicc
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            if (stateDict.ContainsKey("lastSceneBuildIndex"))           //Si fija si se guardó la escena anterior
            {
                buildIndex = (int)stateDict["lastSceneBuildIndex"];     //La recupera
            }
            yield return SceneManager.LoadSceneAsync(buildIndex);       //Carga o la actual o la guardada s/corresponda
            RestoreFromToken(state);                                    //Recupera el resto de los datos
        }

        /// <summary>
        /// Save the current scene to the provided save file.
        /// </summary>
        public void Save(string saveFile)
        {
            JObject state = LoadJsonFromFile(saveFile);                 //Carga el archivo
            CaptureAsToken(state);                                      
            SaveFileAsJSon(saveFile, state);
        }

        /// <summary>
        /// Delete the state in the given save file.
        /// </summary>
        public void Delete(string saveFile)
        {
            File.Delete(GetPathFromSaveFile(saveFile));
        }

        public void Load(string saveFile)
        {
            RestoreFromToken(LoadJsonFromFile(saveFile));
        }

        public IEnumerable<string> ListSaves()
        {
            foreach (string path in Directory.EnumerateFiles(Application.persistentDataPath))
            {
                if (Path.GetExtension(path) == extension)
                {
                    yield return Path.GetFileNameWithoutExtension(path);
                }
            }
        }

        // PRIVATE

        private JObject LoadJsonFromFile(string saveFile)                   //Carga el archivo
        {
            string path = GetPathFromSaveFile(saveFile);                    //Consigue el path
            if (!File.Exists(path))                                         //Si no existe crea uno
            {
                return new JObject();                                       
            }
            
            using (var textReader = File.OpenText(path))                    //Abre el archivo (El using es pa' q se cierre
            {
                using (var reader = new JsonTextReader(textReader))         //Esto reemplaza al sistema Binario
                {
                    reader.FloatParseHandling = FloatParseHandling.Double;

                    return JObject.Load(reader);
                }
            }

        }

        private void SaveFileAsJSon(string saveFile, JObject state)
        {
            var path = GetPathFromSaveFile(saveFile);   
            print("Saving to " + path);
            using (var textWriter = File.CreateText(path))
            {
                using (var writer = new JsonTextWriter(textWriter))
                {
                    writer.Formatting = Formatting.Indented;
                    state.WriteTo(writer);
                }
            }
        }


        private void CaptureAsToken(JObject state)
        {
            IDictionary<string, JToken> stateDict = state;                                      //Crea el dicc
            foreach (JsonSaveableEntity saveable in FindObjectsOfType<JsonSaveableEntity>())    //Busca los scripts
            {
                stateDict[saveable.GetUniqueIdentifier()] = saveable.CaptureAsJtoken();         //Guarda lo que returnea
            }

            stateDict["lastSceneBuildIndex"] = SceneManager.GetActiveScene().buildIndex;      //Guarda la escena
        }


        private void RestoreFromToken(JObject state)
        {
            IDictionary<string, JToken> stateDict = state;                          //Crea el dicc con el state
            foreach (JsonSaveableEntity saveable in FindObjectsOfType<JsonSaveableEntity>())    //Revisa todas los objetos
            {
                var id = saveable.GetUniqueIdentifier();                       //Se fija su codigo unico
                if (stateDict.ContainsKey(id))                                      //Si está en el dicc    
                {   
                    saveable.RestoreFromJToken(stateDict[id]);                      //Recupera desde la entidad los datos    
                }
            }
        }

        //Se encarga de hacer la combinacion entre el nombre del archivo y el path
        private string GetPathFromSaveFile(string saveFile)
        {
            return Path.Combine(Application.persistentDataPath, saveFile + extension);
        }

    }
}