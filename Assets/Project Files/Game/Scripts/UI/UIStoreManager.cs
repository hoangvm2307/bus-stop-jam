using UnityEngine;
using Watermelon.IAPStore;

namespace Watermelon
{
    /// <summary>
    /// Manager to switch between old UIStore system and new Luna UI IAPStoreView system
    /// </summary>
    public static class UIStoreManager
    {
        public enum StoreType
        {
            LegacyStore,    // Original UIStore system
            LunaUIStore     // New Luna UI system
        }

        private static StoreType currentStoreType = StoreType.LegacyStore;
        
        /// <summary>
        /// Set which store system to use
        /// </summary>
        public static void SetStoreType(StoreType storeType)
        {
            currentStoreType = storeType;
            Debug.Log($"[UIStoreManager] Switched to {storeType}");
        }

        /// <summary>
        /// Get current active store type
        /// </summary>
        public static StoreType GetCurrentStoreType()
        {
            return currentStoreType;
        }

        /// <summary>
        /// Show the appropriate store based on current store type
        /// </summary>
        public static void ShowStore()
        {
            switch (currentStoreType)
            {
                case StoreType.LegacyStore:
                    ShowLegacyStore();
                    break;
                case StoreType.LunaUIStore:
                    ShowLunaUIStore();
                    break;
            }
        }

        /// <summary>
        /// Hide the appropriate store based on current store type
        /// </summary>
        public static void HideStore()
        {
            switch (currentStoreType)
            {
                case StoreType.LegacyStore:
                    HideLegacyStore();
                    break;
                case StoreType.LunaUIStore:
                    HideLunaUIStore();
                    break;
            }
        }

        /// <summary>
        /// Check if any store is currently displayed
        /// </summary>
        public static bool IsStoreDisplayed()
        {
            switch (currentStoreType)
            {
                case StoreType.LegacyStore:
                    return UIController.IsDisplayed<UIStore>();
                case StoreType.LunaUIStore:
                    return IsLunaStoreDisplayed();
                default:
                    return false;
            }
        }

        #region Legacy Store Methods
        
        private static void ShowLegacyStore()
        {
            UIController.ShowPage<UIStore>();
        }

        private static void HideLegacyStore()
        {
            UIController.HidePage<UIStore>();
        }

        #endregion

        #region Luna UI Store Methods

        private static void ShowLunaUIStore()
        {
            IAPStoreView.ShowLunaStore();
        }

        private static void HideLunaUIStore()
        {
            IAPStoreView.HideLunaStore();
        }

        private static bool IsLunaStoreDisplayed()
        {
            IAPStoreView storeView = UnityEngine.Object.FindObjectOfType<IAPStoreView>();
            return storeView != null && storeView.UIView.IsVisible;
        }

        #endregion

        #region Developer Testing Methods

        /// <summary>
        /// For testing purposes - show both stores simultaneously
        /// </summary>
        [System.Obsolete("Only for testing purposes")]
        public static void ShowBothStores()
        {
            Debug.LogWarning("[UIStoreManager] Showing both stores for testing - this should not be used in production!");
            ShowLegacyStore();
            ShowLunaUIStore();
        }

        /// <summary>
        /// For testing purposes - hide both stores
        /// </summary>
        [System.Obsolete("Only for testing purposes")]
        public static void HideBothStores()
        {
            HideLegacyStore();
            HideLunaUIStore();
        }

        #endregion
    }
} 