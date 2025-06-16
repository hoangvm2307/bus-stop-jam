namespace Watermelon.BusStop
{
    [System.Serializable]
    public class LevelSave : ISaveObject
    {
        public int RealLevelNumber = 0;
        public int DisplayLevelNumber = 0;
        public bool ReplayingLevelAgain = false; // true when we lost level in randomization mode - so we want to reload the same level, not other random
        private const string SAVE_KEY = "Level";
        public void Flush()
        {

        }

        public void Load()
        {
            if (ES3.KeyExists(SAVE_KEY))
            {
                ES3.Load<LevelSave>(SAVE_KEY, this);
            }
        }

        public void Save()
        {
            Flush();
            ES3.Save<LevelSave>(SAVE_KEY, this);
        }

    }
}
