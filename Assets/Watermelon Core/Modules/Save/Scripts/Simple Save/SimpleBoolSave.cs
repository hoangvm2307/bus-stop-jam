using UnityEngine;

namespace Watermelon
{
    [System.Serializable]
    public class SimpleBoolSave : ISaveObject
    {
        [SerializeField] bool value;
        public virtual bool Value
        {
            get => value;
            set => this.value = value;
        }
        [System.NonSerialized]
        private string saveKey;
        public SimpleBoolSave(string saveKey)
        {
            this.saveKey = saveKey;
            this.value = false;
        }
        public SimpleBoolSave()
        {
            this.saveKey = string.Empty;
            this.value = false;
        }

        public virtual void Flush() { }
        public void Save()
        {
            if (!string.IsNullOrEmpty(saveKey))
            {
                ES3.Save<bool>(saveKey, this.value);
            }
        }
        public void Load()
        { 
            if (!string.IsNullOrEmpty(saveKey))
            { 
                this.value = ES3.Load<bool>(saveKey, defaultValue: false);
            }
        }
    }
}