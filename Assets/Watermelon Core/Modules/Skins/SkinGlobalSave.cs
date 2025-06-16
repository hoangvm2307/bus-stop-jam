using UnityEngine;

namespace Watermelon
{
    [System.Serializable]
    public class SkinGlobalSave : ISaveObject
    {
        public string SelectedSkinID;
 
        private const string SAVE_KEY = "SkinGlobalSave";

        public void Flush()
        { 
        } 
        public void Save()
        { 
            ES3.Save<SkinGlobalSave>(SAVE_KEY, this);
        }
 
        public void Load()
        { 
            if (ES3.KeyExists(SAVE_KEY))
            {
                ES3.Load<SkinGlobalSave>(SAVE_KEY, this);
            } 
        }
    }
}