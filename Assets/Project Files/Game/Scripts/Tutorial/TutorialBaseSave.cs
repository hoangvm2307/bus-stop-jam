using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    [System.Serializable]
    public class TutorialBaseSave : ISaveObject
    {
        public bool isActive;
        public bool isFinished;

        public int progress;
        private const string SAVE_KEY = "TutorialBase";
        public void Save()
        {
            Flush();
            ES3.Save<TutorialBaseSave>(SAVE_KEY, this);
        }
        public void Load()
        {
            if (ES3.KeyExists(SAVE_KEY))
            {
                ES3.Load<TutorialBaseSave>(SAVE_KEY, this);
            }
        }        
        public void Flush()
        {

        }
    }
}