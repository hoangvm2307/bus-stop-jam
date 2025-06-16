using System;
using CupkekGames.Core;
using CupkekGames.Systems;
using UnityEngine;

namespace CupkekGames.Luna.Demo.Newtonsoft
{
    [Serializable]
    public class GameSaveMetadataExample : GameSaveMetadata
    {
        public int Gold;
        public GameSaveMetadataExample() : base()
        {
            SaveDate = DateTime.Now;
            SaveVersion = "-1";
            Gold = -1;
        }
    }
}