using UnityEngine;
using UnityEngine.UIElements;
using CupkekGames.Luna;
using Watermelon;
using CupkekGames.Systems;
using CupkekGames.Systems.UI;

public class GameplayUIView : UIViewComponent
{
    [Header("Settings")]
    [SerializeField] private bool autoUpdateCurrency = true;
    [SerializeField] private float updateInterval = 0.5f;

    // UI Elements
    private Label _heartsText;
    private Label _levelText;
    private Label _coinsText;

    // Buttons
    private Button _storeButton;
    private Button _equipmentButton;
    private Button _noAdsButton;

    // Update tracking
    private float _lastUpdateTime;

    protected override void Awake()
    {
        base.Awake();

        // Get UI elements
        _heartsText = UIDocument.rootVisualElement.Q<Label>("hearts-text");
        _levelText = UIDocument.rootVisualElement.Q<Label>("level-text");
        _coinsText = UIDocument.rootVisualElement.Q<Label>("coins-text");

        // Get buttons
        _storeButton = UIDocument.rootVisualElement.Q<Button>("store-button");
        _equipmentButton = UIDocument.rootVisualElement.Q<Button>("equipment-button");
        _noAdsButton = UIDocument.rootVisualElement.Q<Button>("no-ads-button");
    }

    void OnEnable()
    {
        // Register button events
        if (_storeButton != null)
            _storeButton.RegisterCallback<ClickEvent>(OnStoreButtonClicked);
        if (_equipmentButton != null)
            _equipmentButton.RegisterCallback<ClickEvent>(OnEquipmentButtonClicked);
        if (_noAdsButton != null)
            _noAdsButton.RegisterCallback<ClickEvent>(OnNoAdsButtonClicked);

        // Subscribe to currency changes
        CurrencyController.SubscribeGlobalCallback(OnCurrencyChanged);

        // Initial updates
        UpdateAllDisplays();
    }

    void OnDisable()
    {
        // Unregister button events
        if (_storeButton != null)
            _storeButton.UnregisterCallback<ClickEvent>(OnStoreButtonClicked);
        if (_equipmentButton != null)
            _equipmentButton.UnregisterCallback<ClickEvent>(OnEquipmentButtonClicked);
        if (_noAdsButton != null)
            _noAdsButton.UnregisterCallback<ClickEvent>(OnNoAdsButtonClicked);

        // Unsubscribe from currency changes
        CurrencyController.UnsubscribeGlobalCallback(OnCurrencyChanged);
    }

    void Update()
    {
        // Auto update displays periodically
        if (autoUpdateCurrency && Time.time - _lastUpdateTime > updateInterval)
        {
            UpdateCoinsDisplay();
            UpdateLivesDisplay();
            _lastUpdateTime = Time.time;
        }
    }

    #region Display Updates

    private void UpdateAllDisplays()
    {
        UpdateCoinsDisplay();
        UpdateLevelDisplay();
        UpdateLivesDisplay();
    }

    private void UpdateCoinsDisplay()
    {
        if (_coinsText != null)
        {
            int coins = CurrencyController.Get(CurrencyType.Coins);
            _coinsText.text = FormatNumber(coins);
        }
    }

    private void UpdateLevelDisplay()
    {
        if (_levelText != null)
        {
            // Get current level from game system
            // Assuming there's a level system - adjust this to your actual level system
            int currentLevel = GetCurrentLevel();
            _levelText.text = $"LEVEL {currentLevel}";
        }
    }

    private void UpdateLivesDisplay()
    {
        if (_heartsText != null)
        {
            // Check if lives system exists and has lives
            bool hasFullLives = CheckIfLivesAreFull();

            if (hasFullLives)
            {
                _heartsText.text = "FULL";
                // Add pulse animation for full lives
                var heartsContainer = UIDocument.rootVisualElement.Q<VisualElement>("hearts-container");
                if (heartsContainer != null)
                {
                    heartsContainer.AddToClassList("pulse-animation");
                }
            }
            else
            {
                int livesCount = GetCurrentLivesCount();
                _heartsText.text = livesCount.ToString();

                // Remove pulse animation if not full
                var heartsContainer = UIDocument.rootVisualElement.Q<VisualElement>("hearts-container");
                if (heartsContainer != null)
                {
                    heartsContainer.RemoveFromClassList("pulse-animation");
                }
            }
        }
    }

    #endregion

    #region Button Event Handlers

    private void OnStoreButtonClicked(ClickEvent e)
    {
        Debug.Log("Store button clicked - Opening IAP Store");
        PlayClickFeedback();
 
        try
        {

            var instances = UIPrefabLoaderString.Instance.GetInstances("IAPStore");
            if (instances != null && instances.Count > 0)
            {
                // Reuse existing instance
                var existingStore = instances[0].GetComponent<IAPStoreView>();
                if (existingStore != null)
                {
                    existingStore.Fade.FadeIn();
                    return;
                }
            }
        }
        catch (System.Exception)
        {
            Debug.Log("Error opening store");
        }

        UIPrefabLoaderString.Instance.Instantiate("IAPStore");
    }
    private void OnEquipmentButtonClicked(ClickEvent e)
    {
        Debug.Log("Equipment button clicked - Opening Skin Store");
        PlayClickFeedback();

        // Open skin/equipment store
        // Adjust this to your actual skin store system
        OpenEquipmentStore();
    }

    private void OnNoAdsButtonClicked(ClickEvent e)
    {
        Debug.Log("No Ads button clicked - Opening No Ads Purchase");
        PlayClickFeedback();

        // Direct purchase of No Ads
        if (IAPManager.IsInitialized)
        {
            IAPManager.BuyProduct(ProductKeyType.NoAds);
        }
        else
        {
            SystemMessage.ShowMessage("Store not available. Please try again later.");
        }
    }

    #endregion

    #region Event Callbacks

    private void OnCurrencyChanged(Currency currency, int difference)
    {
        // Update coins display when currency changes
        if (currency.CurrencyType == CurrencyType.Coins)
        {
            UpdateCoinsDisplay();

            // Show coin gain animation if positive
            if (difference > 0)
            {
                ShowCoinGainEffect(difference);
            }
        }
    }

    #endregion

    #region Helper Methods

    private void PlayClickFeedback()
    {
#if MODULE_HAPTIC
        Haptic.Play(Haptic.HAPTIC_LIGHT);
#endif
        AudioController.PlaySound(AudioController.AudioClips.buttonSound);
    }

    private string FormatNumber(int number)
    {
        // Format large numbers (e.g., 1000 -> 1K, 1000000 -> 1M)
        if (number >= 1000000)
            return (number / 1000000.0f).ToString("F1") + "M";
        else if (number >= 1000)
            return (number / 1000.0f).ToString("F1") + "K";
        else
            return number.ToString();
    }

    private int GetCurrentLevel()
    {
        // Get current level from your level system
        // This is a placeholder - replace with your actual level system
        return PlayerPrefs.GetInt("CurrentLevel", 1);
    }

    private bool CheckIfLivesAreFull()
    {
        // Check if lives system has full lives
        // This is a placeholder - replace with your actual lives system
#if MODULE_LIVES
        return LivesSystem.Status.LivesCount >= LivesSystem.Settings.MaxLives;
#else
        return true; // Default to full if no lives system
#endif
    }

    private int GetCurrentLivesCount()
    {
        // Get current lives count
        // This is a placeholder - replace with your actual lives system
#if MODULE_LIVES
        return LivesSystem.Status.LivesCount;
#else
        return 5; // Default lives count
#endif
    }

    private void OpenEquipmentStore()
    {
        // Open the Skin Store using Watermelon's UI system
        UIController.ShowPage<Watermelon.SkinStore.UISkinStore>();

        Debug.Log("Opening Skin Store");
    }

    private void ShowCoinGainEffect(int amount)
    {
        // Create a floating text effect for coin gains
        Debug.Log($"Gained {amount} coins!");

        // You could implement a floating text animation here
        // For example, spawn a temporary UI element that animates upward and fades out
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Force update all UI displays
    /// </summary>
    public void RefreshUI()
    {
        UpdateAllDisplays();
    }

    /// <summary>
    /// Set the level display manually
    /// </summary>
    public void SetLevel(int level)
    {
        if (_levelText != null)
        {
            _levelText.text = $"LEVEL {level}";
        }
    }

    /// <summary>
    /// Set the lives display manually
    /// </summary>
    public void SetLives(int lives, int maxLives)
    {
        if (_heartsText != null)
        {
            if (lives >= maxLives)
            {
                _heartsText.text = "FULL";
            }
            else
            {
                _heartsText.text = lives.ToString();
            }
        }
    }

    /// <summary>
    /// Show/hide the entire gameplay UI
    /// </summary>
    public void SetUIVisible(bool visible)
    {
        var rootElement = UIDocument.rootVisualElement.Q<VisualElement>("gameplay-ui");
        if (rootElement != null)
        {
            rootElement.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }

    #endregion
}