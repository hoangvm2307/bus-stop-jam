namespace Watermelon
{
    [System.Serializable]
    public class SkinSave : ISaveObject
    {
        public bool IsUnlocked = false;
        [System.NonSerialized]
        private string skinID;
        private string SAVE_KEY => "Skin_" + skinID;
        public SkinSave(string skinID)
        {
            this.skinID = skinID;
        }
        public void Save()
        {
            if (!string.IsNullOrEmpty(skinID))
            {
                ES3.Save<SkinSave>(SAVE_KEY, this);
            }
        }

        public void Load()
        {
            if (!string.IsNullOrEmpty(skinID) && ES3.KeyExists(SAVE_KEY))
            {
                ES3.LoadInto<SkinSave>(SAVE_KEY, this);
            }
        }
        public void Flush()
        {

        }
    }
}
