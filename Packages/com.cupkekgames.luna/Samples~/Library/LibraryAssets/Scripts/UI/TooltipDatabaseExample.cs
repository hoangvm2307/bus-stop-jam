using CupkekGames.Core;
using UnityEngine;

namespace CupkekGames.Luna.Library
{
    /// <summary>
    /// Provides centralized access to the default tooltip instance.
    /// This implementation uses a Singleton pattern for global accessibility,
    /// but alternative approaches like dependency injection or service locators
    /// can be considered for better modularity and testability.
    /// </summary>
    public class TooltipDatabaseExample : Singleton<TooltipDatabaseExample>
    {
        [SerializeField] TooltipController _tooltipController;
        public TooltipController TooltipController => _tooltipController;
    }
}