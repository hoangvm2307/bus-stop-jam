using UnityEngine;

namespace Watermelon
{
    [System.Serializable]
    public class AdSave : ISaveObject
    {
        public bool IsForcedAdEnabled = true;
 
        private const string SAVE_KEY = "AdSettings";

        public void Flush()
        { 
        } 
        public void Save()
        { 
            ES3.Save<AdSave>(SAVE_KEY, this);
        }
 
        public void Load()
        { 
            if (ES3.KeyExists(SAVE_KEY))
            {
                ES3.LoadInto<AdSave>(SAVE_KEY, this);
            } 
        }
    }
}