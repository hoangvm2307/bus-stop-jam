namespace Watermelon
{
    [System.Serializable]
    public class AudioSave : ISaveObject
    {
        public VolumeData[] VolumeDatas;
        private const string SAVE_KEY = "AudioSettings";
        public void Flush()
        {
            AudioType[] audioTypes = EnumUtils.GetEnumArray<AudioType>();

            VolumeDatas = new VolumeData[audioTypes.Length];

            for (int i = 0; i < audioTypes.Length; i++)
            {
                VolumeDatas[i] = new VolumeData() { AudioType = audioTypes[i], Volume = AudioController.GetVolume(audioTypes[i]) };
            }
        }
        public void Save()
        {
            Flush();

            ES3.Save<AudioSave>(SAVE_KEY, this);
        }
        public void Load()
        {
            if (ES3.KeyExists(SAVE_KEY))
            {
                ES3.Load<AudioSave>(SAVE_KEY, this);

                if (VolumeDatas != null)
                {
                    foreach (var volumeData in VolumeDatas)
                    {
                        AudioController.SetVolume(volumeData.AudioType, volumeData.Volume);
                    }
                }
            }
        }
        [System.Serializable]
        public class VolumeData
        {
            public AudioType AudioType;
            public float Volume;
        }
    }
}

