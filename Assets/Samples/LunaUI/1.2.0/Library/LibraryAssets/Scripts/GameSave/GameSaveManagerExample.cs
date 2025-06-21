using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CupkekGames.Core;
using CupkekGames.Systems;
using UnityEngine;

namespace CupkekGames.Luna.Library
{
    [CreateAssetMenu(fileName = "GameSaveManagerExample", menuName = "CupkekGames/Samples/GameSaveManagerExample")]
    public class GameSaveManagerExample : GameSaveManager<GameSaveDataExample, GameSaveMetadataExample>
    {
        [MultiLineHeader("This is a mockup. The save system is not included.\nFor a working example, please refer to the Newtonsoft sample.")]
        [SerializeField] private int _mockSaveAmount = 50;
        private Dictionary<string, GameSaveDataExample> _saveMockUp = new Dictionary<string, GameSaveDataExample>();
        public Dictionary<string, GameSaveDataExample> SaveMockUp => _saveMockUp;

        public void OnEnable()
        {
            DateTime endDate = DateTime.Now;
            DateTime startDate = endDate.AddMonths(-3);

            for (int saveSlot = 0; saveSlot < _mockSaveAmount; saveSlot++)
            {
                string fileName = GetSaveFileName(saveSlot);

                GameSaveDataExample data = new GameSaveDataExample();
                
                bool isAutosave = UnityEngine.Random.Range(0, 100) < 50;
                data.Metadata = data.CreateMetadata(GetSaveVersion(), isAutosave);

                data.Metadata.SaveDate = GetRandomDate(startDate, endDate);
                data.Gold = UnityEngine.Random.Range(100, 1000);
                UpdateMetadata(data.Metadata as GameSaveMetadataExample, data);

                _saveMockUp.Add(fileName, data);
            }
        }

        public DateTime GetRandomDate(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("startDate should be less than or equal to endDate");

            // Get the range of ticks between the two dates
            long range = (endDate - startDate).Ticks;

            // Generate a random number of ticks within this range
            var random = new System.Random();
            long randomTicks = (long)(random.NextDouble() * range);

            // Return the random date by adding random ticks to the start date
            return startDate.AddTicks(randomTicks);
        }
        protected override List<string> GetAllFileNames()
        {
            return _saveMockUp.Keys.ToList();
        }

        protected override GameSaveDataExample GetNewSave(string saveVersion)
        {
            var newSave = new GameSaveDataExample();
            newSave.Metadata = newSave.CreateMetadata(saveVersion, false);
            return newSave;
        }

        protected override void OnDeleteRequest(int saveSlot, string fileName)
        {
            _saveMockUp.Remove(fileName);
        }

        protected override void OnSaveRequest(int saveSlot, string fileName, GameSaveDataExample data, bool autosave)
        {
            if (autosave)
            {
                GameSaveEvents.AutosaveStart?.Invoke();
            }

            GameSaveDataExample clone = new();

            clone.PlayerName = data.PlayerName;
            clone.Gold = data.Gold;
            clone.Diamond = data.Diamond;
            clone.Exp = data.Exp;
            clone.Lvl = data.Lvl;
            clone.ExpReq = data.ExpReq;
            clone.NotificationHistory = data.NotificationHistory;

            clone.Metadata = clone.CreateMetadata(GetSaveVersion(), autosave);
            var metadata = clone.Metadata as GameSaveMetadataExample;
            UpdateMetadata(metadata, clone);

            if (_saveMockUp.ContainsKey(fileName))
            {
                _saveMockUp[fileName] = clone;
            }
            else
            {
                _saveMockUp.Add(fileName, clone);
            }

            if (autosave)
            {
                GameSaveEvents.AutosaveComplete?.Invoke();
            }
        }

        private void UpdateMetadata(GameSaveMetadataExample metadata, GameSaveDataExample playerData)
        {
            metadata.Gold = playerData.Gold;
            // Add more metadata here if needed
        }

        protected override string GetFileExtenstion() => "json";

        protected override GameSaveDataExample LoadFromFile(string fileName)
        {
            return _saveMockUp[fileName];
        }

        protected override GameSaveMetadataExample LoadMetadataFromFile(string fileName)
        {
            return _saveMockUp[fileName].Metadata as GameSaveMetadataExample;
        }

        protected override string GetSaveVersion() => "SaveVersion";
    }
}