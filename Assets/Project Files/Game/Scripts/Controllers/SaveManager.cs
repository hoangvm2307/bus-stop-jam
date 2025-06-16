using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public class SaveManager : MonoBehaviour
    {
        private static SaveManager instance;
        private List<ISaveObject> registeredSaveObjects = new List<ISaveObject>();

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public static void Register(ISaveObject saveObject)
        {
            if (instance != null && !instance.registeredSaveObjects.Contains(saveObject))
            {
                instance.registeredSaveObjects.Add(saveObject);
            }
        }

        public static void Unregister(ISaveObject saveObject)
        {
            if (instance != null)
            {
                instance.registeredSaveObjects.Remove(saveObject);
            }
        }

        public static void SaveAll()
        {
            if (instance == null) return;

            foreach (var saveObject in instance.registeredSaveObjects)
            {
                saveObject.Save(); 
            }
            Debug.Log("All data saved with Easy Save 3.");
        }
 
        private void OnApplicationQuit()
        {
            SaveAll();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                SaveAll();
            }
        }
    }
}