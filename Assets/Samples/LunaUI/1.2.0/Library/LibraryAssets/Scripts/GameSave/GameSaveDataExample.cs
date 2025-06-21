using System;
using CupkekGames.Core;
using CupkekGames.Systems;
using UnityEngine;

namespace CupkekGames.Luna.Library
{
    [Serializable]
    public class GameSaveDataExample : IGameSaveData
    {
        public GameSaveMetadata Metadata { get; set; }
        public string PlayerName;
        public int Gold;
        public int Diamond;
        public int Exp;
        public int Lvl;
        public int ExpReq;
        
        public float TargetValue => (float)Exp / (float)ExpReq;
        
        public NotificationHistory NotificationHistory;

        public GameSaveDataExample()
        {
            // New Save
            PlayerName = "Luna Yuai";
            Gold = 500; // Starting gold
            Diamond = 100; // Starting diamond
            Exp = 865; // Starting exp
            Lvl = 12; // Starting lvl
            ExpReq = 100 * Lvl; //Exp Required for next level
            NotificationHistory = new();
        }
        public GameSaveMetadata CreateMetadata(string saveVersion, bool isAutosave)
        {
            GameSaveMetadataExample metadata = new GameSaveMetadataExample();
            metadata.SaveVersion = saveVersion;
            metadata.SaveDate = DateTime.Now;
            metadata.IsAutosave = isAutosave;
            return metadata;
        }
        public void LoadFrom(IGameSaveData other, int saveSlot)
        {
            if (other != null)
            {
                GameSaveDataExample loaded = (GameSaveDataExample)other;

                PlayerName = loaded.PlayerName;
                Metadata = loaded.Metadata;
                Gold = loaded.Gold;
                Diamond = loaded.Diamond;
                Exp = loaded.Exp;
                Lvl = loaded.Lvl;
                ExpReq = loaded.ExpReq;
                NotificationHistory = loaded.NotificationHistory;
            }
            else
            {
                Debug.LogError("other is null");
            }
        }
    }
}