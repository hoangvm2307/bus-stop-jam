using UnityEngine;

namespace Watermelon
{
    [System.Serializable]
    public class SimpleStringSave : ISaveObject
    {
        [SerializeField] string value;
        public virtual string Value
        {
            get => value;
            set => this.value = value;
        }
 
        [System.NonSerialized]
        private string saveKey;

        public SimpleStringSave(string saveKey)
        {
            this.saveKey = saveKey;
            this.value = string.Empty; 
        }

        public SimpleStringSave()
        {
            this.saveKey = string.Empty;
            this.value = string.Empty;
        }

        public virtual void Flush() { }

        public void Save()
        {
            if (!string.IsNullOrEmpty(saveKey))
            {
                ES3.Save<string>(saveKey, this.value);
            }
        }

        public void Load()
        {
            if (!string.IsNullOrEmpty(saveKey))
            {
                this.value = ES3.Load<string>(saveKey, defaultValue: string.Empty);
            }
        }
    }
}