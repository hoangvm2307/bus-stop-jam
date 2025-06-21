

using System;
using UnityEngine;

namespace CupkekGames.InventorySystem
{
    public abstract class InventoryItemDefinitionSO<T> : ScriptableObject where T : InventoryItemDefinition
    {
        public T ItemDefinition;
    }
}