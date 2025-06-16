

using System;
using UnityEngine;

namespace CupkekGames.InventorySystem
{
    [Serializable]
    public abstract class InventoryItemDefinition
    {
        public string Name = "";
        public string Description = "";
        public Sprite Icon;
    }
}