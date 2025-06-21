using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Watermelon
{
    /// <summary>
    /// Demo script to showcase the integration between Legacy Store and Luna UI Store
    /// </summary>
    public class IAPStoreIntegrationDemo : MonoBehaviour
    {
        [Header("Demo UI")]
        [SerializeField] private Button openLegacyStoreButton;
        [SerializeField] private Button openLunaStoreButton;
        [SerializeField] private Button openCurrentStoreButton;
        [SerializeField] private TextMeshProUGUI infoText;
        [SerializeField] private TextMeshProUGUI currencyDisplayText;

        [Header("Settings")]
        [SerializeField] private bool autoUpdateCurrency = true;
        [SerializeField] private float updateInterval = 1f;

        private float lastUpdateTime;

        private void Start()
        {
            InitializeDemo();
            UpdateInfoText();
            
            if (autoUpdateCurrency)
            {
                UpdateCurrencyDisplay();
            }
        }

        private void Update()
        {
            if (autoUpdateCurrency && Time.time - lastUpdateTime > updateInterval)
            {
                UpdateCurrencyDisplay();
                lastUpdateTime = Time.time;
            }
        }

        private void InitializeDemo()
        {
            // Setup button listeners
            if (openLegacyStoreButton != null)
            {
                openLegacyStoreButton.onClick.AddListener(() => OpenSpecificStore(UIStoreManager.StoreType.LegacyStore));
            }

            if (openLunaStoreButton != null)
            {
                openLunaStoreButton.onClick.AddListener(() => OpenSpecificStore(UIStoreManager.StoreType.LunaUIStore));
            }

            if (openCurrentStoreButton != null)
            {
                openCurrentStoreButton.onClick.AddListener(OpenCurrentStore);
            }

            // Subscribe to IAP events for demo purposes
            IAPManager.PurchaseCompleted += OnDemoPurchaseCompleted;
            IAPManager.PurchaseFailed += OnDemoPurchaseFailed;
        }

        private void OnDestroy()
        {
            // Cleanup subscriptions
            IAPManager.PurchaseCompleted -= OnDemoPurchaseCompleted;
            IAPManager.PurchaseFailed -= OnDemoPurchaseFailed;
        }

        private void OpenSpecificStore(UIStoreManager.StoreType storeType)
        {
            Debug.Log($"[IAPStoreIntegrationDemo] Opening {storeType}");
            
            // Switch to specific store type
            UIStoreManager.SetStoreType(storeType);
            
            // Show the store
            UIStoreManager.ShowStore();
            
            // Update info
            UpdateInfoText();
            
            // Play feedback sound
            AudioController.PlaySound(AudioController.AudioClips.buttonSound);
        }

        private void OpenCurrentStore()
        {
            Debug.Log($"[IAPStoreIntegrationDemo] Opening current store: {UIStoreManager.GetCurrentStoreType()}");
            
            UIStoreManager.ShowStore();
            AudioController.PlaySound(AudioController.AudioClips.buttonSound);
        }

        private void UpdateInfoText()
        {
            if (infoText == null) return;

            var currentStoreType = UIStoreManager.GetCurrentStoreType();
            var isStoreOpen = UIStoreManager.IsStoreDisplayed();
            var iapInitialized = IAPManager.IsInitialized;

            string info = $"<b>Store Integration Demo</b>\n\n" +
                         $"<color=yellow>Current Store:</color> {currentStoreType}\n" +
                         $"<color=yellow>Store Status:</color> {(isStoreOpen ? "<color=green>OPEN</color>" : "<color=red>CLOSED</color>")}\n" +
                         $"<color=yellow>IAP Manager:</color> {(iapInitialized ? "<color=green>READY</color>" : "<color=red>NOT READY</color>")}\n\n" +
                         GetStoreComparisonInfo();

            infoText.text = info;
        }

        private string GetStoreComparisonInfo()
        {
            return "<b>Store Comparison:</b>\n" +
                   "<color=cyan>Legacy Store (UIStore):</color>\n" +
                   "• Unity UI (uGUI) based\n" +
                   "• Traditional Canvas system\n" +
                   "• Existing animation system\n\n" +
                   "<color=blue>Luna UI Store:</color>\n" +
                   "• UI Toolkit (UIElements) based\n" +
                   "• Modern web-like styling\n" +
                   "• Advanced fade animations\n" +
                   "• Real-time price updates\n" +
                   "• Integrated with IAPManager";
        }

        private void UpdateCurrencyDisplay()
        {
            if (currencyDisplayText == null) return;

            int coins = CurrencyController.Get(CurrencyType.Coins);
            currencyDisplayText.text = $"Coins: {coins:N0}";
        }

        private void OnDemoPurchaseCompleted(ProductKeyType productKeyType)
        {
            Debug.Log($"[IAPStoreIntegrationDemo] Purchase completed: {productKeyType}");
            
            // Update currency display
            UpdateCurrencyDisplay();
            
            // Show a success message
            ShowTemporaryMessage($"Purchase Successful!\n{productKeyType}", Color.green);
        }

        private void OnDemoPurchaseFailed(ProductKeyType productKeyType, PurchaseFailureReason failureReason)
        {
            Debug.LogWarning($"[IAPStoreIntegrationDemo] Purchase failed: {productKeyType} - {failureReason}");
            
            // Show a failure message
            ShowTemporaryMessage($"Purchase Failed!\n{productKeyType}\n{failureReason}", Color.red);
        }

        private void ShowTemporaryMessage(string message, Color color)
        {
            // You could implement a temporary message display here
            // For now, just log it
            Debug.Log($"[IAPStoreIntegrationDemo] {message}");
        }

        #region Public Methods for Inspector/External Use

        [ContextMenu("Test Legacy Store")]
        public void TestLegacyStore()
        {
            OpenSpecificStore(UIStoreManager.StoreType.LegacyStore);
        }

        [ContextMenu("Test Luna UI Store")]
        public void TestLunaStore()
        {
            OpenSpecificStore(UIStoreManager.StoreType.LunaUIStore);
        }

        [ContextMenu("Add Test Coins")]
        public void AddTestCoins()
        {
            CurrencyController.Add(CurrencyType.Coins, 1000);
            UpdateCurrencyDisplay();
            Debug.Log("[IAPStoreIntegrationDemo] Added 1000 test coins");
        }

        [ContextMenu("Reset Coins")]
        public void ResetCoins()
        {
            CurrencyController.Set(CurrencyType.Coins, 0);
            UpdateCurrencyDisplay();
            Debug.Log("[IAPStoreIntegrationDemo] Reset coins to 0");
        }

        [ContextMenu("Refresh Info")]
        public void RefreshInfo()
        {
            UpdateInfoText();
            UpdateCurrencyDisplay();
        }

        #endregion
    }
} 