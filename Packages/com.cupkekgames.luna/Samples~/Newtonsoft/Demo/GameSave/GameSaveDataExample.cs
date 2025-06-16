using System;
using CupkekGames.Core;
using CupkekGames.InventorySystem;
using CupkekGames.Systems;
using Newtonsoft.Json;
using UnityEngine;

namespace CupkekGames.Luna.Demo.Newtonsoft
{
    [Serializable]
    public class GameSaveDataExample : IGameSaveData
    {
        // Order is used to ensure that the Metadata is at top of the json file
        // This is important for optimization when loading only the metadata
        [JsonProperty(Order = -100)]
        public GameSaveMetadata Metadata { get; set; }
        public string PlayerName;
        public int Gold;
        public int Diamond;
        public int Exp;
        public int Lvl;
        public int ExpReq;
        
        [JsonIgnore]
        public float TargetValue => (float)Exp / (float)ExpReq;
        public Inventory Inventory;

        public GameSaveDataExample()
        {
            // New Save
            PlayerName = "Luna Yuai";
            Gold = 500; // Starting gold
            Diamond = 100; // Starting diamond
            Exp = 865; // Starting exp
            Lvl = 12; // Starting lvl
            ExpReq = 100 * Lvl; //Exp Required for next level
            Inventory = new Inventory();
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
                Inventory = loaded.Inventory;
            }
            else
            {
                Debug.LogError("other is null");
            }
        }
    }
}