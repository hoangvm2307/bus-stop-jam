using System;

namespace Watermelon
{
    [Serializable]
    public class LivesSave : ISaveObject
    {
        public int LivesCount = -1;

        public bool LifeLocked = false;
        public long NewLifeDateBinary;

        public bool InfiniteLives = false;
        public long InfiniteLivesDateBinary;

        [NonSerialized] LivesStatus status;
        private const string SAVE_KEY = "Lives";

        public void Init(LivesStatus status)
        {
            this.status = status;
        }

        public void Flush()
        {
            if (status == null) return;

            LivesCount = status.LivesCount;

            InfiniteLives = status.InfiniteMode;
            InfiniteLivesDateBinary = status.InfiniteModeDate.ToBinary();

            if (status.NewLifeTimerEnabled)
            {
                NewLifeDateBinary = status.NewLifeDate.ToBinary();
            }
            else
            {
                NewLifeDateBinary = (DateTime.Now + LivesSystem.OneLifeSpan).ToBinary();
            }
        }

        public void Save()
        {
            Flush();
            ES3.Save<LivesSave>(SAVE_KEY, this);
        }

        public void Load()
        {
            if (ES3.KeyExists(SAVE_KEY))
            {
                ES3.LoadInto<LivesSave>(SAVE_KEY, this);
            }
        }
    }
}