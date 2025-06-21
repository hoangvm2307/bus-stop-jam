using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Watermelon
{
    /// <summary>
    /// Developer UI to test and switch between store systems
    /// </summary>
    public class UIStoreSwitcher : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button switchStoreButton;
        [SerializeField] private Button showStoreButton;
        [SerializeField] private Button hideStoreButton;
        [SerializeField] private Button showBothButton;
        [SerializeField] private Button hideBothButton;
        [SerializeField] private TextMeshProUGUI currentStoreText;
        [SerializeField] private TextMeshProUGUI storeStatusText;

        [Header("Settings")]
        [SerializeField] private bool showOnlyInDevelopment = true;

        private void Start()
        {
            // Hide in build if specified
            if (showOnlyInDevelopment && !Debug.isDebugBuild)
            {
                gameObject.SetActive(false);
                return;
            }

            InitializeButtons();
            UpdateUI();
        }

        private void InitializeButtons()
        {
            if (switchStoreButton != null)
                switchStoreButton.onClick.AddListener(SwitchStore);
            
            if (showStoreButton != null)
                showStoreButton.onClick.AddListener(ShowStore);
            
            if (hideStoreButton != null)
                hideStoreButton.onClick.AddListener(HideStore);
            
            if (showBothButton != null)
                showBothButton.onClick.AddListener(ShowBothStores);
            
            if (hideBothButton != null)
                hideBothButton.onClick.AddListener(HideBothStores);
        }

        private void Update()
        {
            // Update status text every frame to show real-time status
            if (storeStatusText != null)
            {
                bool isDisplayed = UIStoreManager.IsStoreDisplayed();
                storeStatusText.text = $"Store Status: {(isDisplayed ? "OPEN" : "CLOSED")}";
                storeStatusText.color = isDisplayed ? Color.green : Color.red;
            }
        }

        private void SwitchStore()
        {
            // Close current store before switching
            UIStoreManager.HideStore();

            // Switch to the other store type
            var currentType = UIStoreManager.GetCurrentStoreType();
            var newType = currentType == UIStoreManager.StoreType.LegacyStore 
                ? UIStoreManager.StoreType.LunaUIStore 
                : UIStoreManager.StoreType.LegacyStore;
            
            UIStoreManager.SetStoreType(newType);
            UpdateUI();

            // Play sound for feedback
            AudioController.PlaySound(AudioController.AudioClips.buttonSound);
        }

        private void ShowStore()
        {
            UIStoreManager.ShowStore();
            AudioController.PlaySound(AudioController.AudioClips.buttonSound);
        }

        private void HideStore()
        {
            UIStoreManager.HideStore();
            AudioController.PlaySound(AudioController.AudioClips.buttonSound);
        }

        private void ShowBothStores()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            UIStoreManager.ShowBothStores();
#pragma warning restore CS0618 // Type or member is obsolete
            AudioController.PlaySound(AudioController.AudioClips.buttonSound);
        }

        private void HideBothStores()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            UIStoreManager.HideBothStores();
#pragma warning restore CS0618 // Type or member is obsolete
            AudioController.PlaySound(AudioController.AudioClips.buttonSound);
        }

        private void UpdateUI()
        {
            if (currentStoreText != null)
            {
                var currentType = UIStoreManager.GetCurrentStoreType();
                currentStoreText.text = $"Current Store: {currentType}";
                
                // Color coding for visual feedback
                currentStoreText.color = currentType == UIStoreManager.StoreType.LunaUIStore 
                    ? Color.blue 
                    : Color.white;
            }
        }

        #region Public Methods for Testing

        /// <summary>
        /// Switch to Legacy Store (for external scripts)
        /// </summary>
        public void SwitchToLegacyStore()
        {
            UIStoreManager.HideStore();
            UIStoreManager.SetStoreType(UIStoreManager.StoreType.LegacyStore);
            UpdateUI();
        }

        /// <summary>
        /// Switch to Luna UI Store (for external scripts)
        /// </summary>
        public void SwitchToLunaUIStore()
        {
            UIStoreManager.HideStore();
            UIStoreManager.SetStoreType(UIStoreManager.StoreType.LunaUIStore);
            UpdateUI();
        }

        #endregion

        #region Editor Methods

#if UNITY_EDITOR
        [ContextMenu("Test Switch Store")]
        private void TestSwitchStore()
        {
            SwitchStore();
        }

        [ContextMenu("Test Show Store")]
        private void TestShowStore()
        {
            ShowStore();
        }

        [ContextMenu("Force Luna UI Store")]
        private void ForceLunaUIStore()
        {
            SwitchToLunaUIStore();
        }

        [ContextMenu("Force Legacy Store")]
        private void ForceLegacyStore()
        {
            SwitchToLegacyStore();
        }
#endif

        #endregion
    }
} 