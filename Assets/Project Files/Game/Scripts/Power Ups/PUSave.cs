namespace Watermelon
{
    [System.Serializable]
    public class PUSave : ISaveObject
    {
        public int Amount = -1;
        public bool IsUnlocked = false;
        private const string SAVE_KEY = "PowerUp";
        public void Save()
        {
            Flush();
            ES3.Save<PUSave>(SAVE_KEY, this);
        }
        public void Load()
        {
            if (ES3.KeyExists(SAVE_KEY))
            {
                ES3.Load<PUSave>(SAVE_KEY, this);
            }
        }
        public void Flush()
        {

        }
    }
}
