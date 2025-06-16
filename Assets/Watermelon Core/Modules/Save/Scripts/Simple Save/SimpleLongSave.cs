using UnityEngine;

namespace Watermelon
{
    [System.Serializable]
    public class SimpleLongSave : ISaveObject
    {
        [SerializeField] long value;
        public virtual long Value
        {
            get => value;
            set => this.value = value;
        }
 
        [System.NonSerialized]
        private string saveKey;

        public SimpleLongSave(string saveKey)
        {
            this.saveKey = saveKey;
            this.value = 0L; 
        }

        public SimpleLongSave()
        {
            this.saveKey = string.Empty;
            this.value = 0L;
        }

        public virtual void Flush() { }

        public void Save()
        {
            if (!string.IsNullOrEmpty(saveKey))
            {
                ES3.Save<long>(saveKey, this.value);
            }
        }

        public void Load()
        {
            if (!string.IsNullOrEmpty(saveKey))
            {
                this.value = ES3.Load<long>(saveKey, defaultValue: 0L);
            }
        }
    }
}