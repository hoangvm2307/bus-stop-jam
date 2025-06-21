namespace Watermelon
{
    [System.Serializable]
    public class PUSave : ISaveObject
    {
        public int Amount = -1;
        public bool IsUnlocked = false;
        [System.NonSerialized]
        private PUType powerUpType;
        private const string SAVE_KEY_PREFIX = "PowerUp_";
        private string SaveKey => SAVE_KEY_PREFIX + powerUpType.ToString();

        public PUSave(PUType powerUpType)
        {
            this.powerUpType = powerUpType;
        }
        public void Save()
        {
            Flush();
            ES3.Save<PUSave>(SaveKey, this);
        }
        public void Load()
        {
            if (ES3.KeyExists(SaveKey))
            {
                ES3.LoadInto<PUSave>(SaveKey, this);
            }
        }
        public void Flush()
        {

        }
    }
}
