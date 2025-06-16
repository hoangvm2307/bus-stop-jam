using UnityEngine;

namespace Watermelon
{
    [System.Serializable]
    public class SimpleIntSave : ISaveObject
    {
        [SerializeField] int value;
        public virtual int Value
        {
            get => value;
            set => this.value = value;
        } 
        [System.NonSerialized]
        private string saveKey;

        public SimpleIntSave(string saveKey)
        {
            this.saveKey = saveKey;
            this.value = 0;  
        }

        public SimpleIntSave()
        {
            this.saveKey = string.Empty;
            this.value = 0;
        }

        public virtual void Flush() { }

        public void Save()
        {
            if (!string.IsNullOrEmpty(saveKey))
            {
                ES3.Save<int>(saveKey, this.value);
            }
        }

        public void Load()
        {
            if (!string.IsNullOrEmpty(saveKey))
            {
                this.value = ES3.Load<int>(saveKey, defaultValue: 0);
            }
        }
    }
}