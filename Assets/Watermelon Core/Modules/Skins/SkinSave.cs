namespace Watermelon
{
    [System.Serializable]
    public class SkinSave : ISaveObject
    {
        public bool IsUnlocked = false;
        private const string SAVE_KEY = "Skin";
        public void Save()
        {
            Flush();
            ES3.Save<SkinSave>(SAVE_KEY, this);
        }
        public void Load()
        {
            if (ES3.KeyExists(SAVE_KEY))
            {
                ES3.Load<SkinSave>(SAVE_KEY, this);
            }
        }
        public void Flush()
        {

        }
    }
}
