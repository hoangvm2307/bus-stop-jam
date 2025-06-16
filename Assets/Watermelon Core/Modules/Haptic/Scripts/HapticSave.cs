using UnityEngine;

namespace Watermelon
{
    [System.Serializable]
    public class HapticSave : ISaveObject
    {

        public bool IsActive = true;

        private const string SAVE_KEY = "HapticSettings";

        public void Flush()
        {

        }

        public void Save()
        {
            ES3.Save<HapticSave>(SAVE_KEY, this);
        }

        public void Load()
        {
            if (ES3.KeyExists(SAVE_KEY))
            {
                ES3.LoadInto<HapticSave>(SAVE_KEY, this);
            }
        }
    }
}