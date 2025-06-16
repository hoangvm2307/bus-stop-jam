using UnityEngine;

namespace Watermelon
{
    [System.Serializable]
    public class SimpleFloatSave : ISaveObject
    {
        [SerializeField] float value;
        public virtual float Value
        {
            get => value;
            set => this.value = value;
        }
 
        [System.NonSerialized]
        private string saveKey;

        public SimpleFloatSave(string saveKey)
        {
            this.saveKey = saveKey;
            this.value = 0f;  
        }

        public SimpleFloatSave()
        {
            this.saveKey = string.Empty;
            this.value = 0f;
        }

        public virtual void Flush() { }

        public void Save()
        {
            if (!string.IsNullOrEmpty(saveKey))
            {
                ES3.Save<float>(saveKey, this.value);
            }
        }

        public void Load()
        {
            if (!string.IsNullOrEmpty(saveKey))
            {
                this.value = ES3.Load<float>(saveKey, defaultValue: 0f);
            }
        }
    }
}