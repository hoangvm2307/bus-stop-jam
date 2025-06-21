using UnityEngine;
using UnityEngine.UIElements;
using CupkekGames.Luna;
using Watermelon;

public class IAPStoreView : UIViewComponent
{
    private Button _noAdsPurchaseButton;
    private Button _starterPackPurchaseButton;
    private Button _powerUpPurchaseButton;
    private Button _closeButton;

    // Coin pack buttons
    private Button _coinsSmallButton;
    private Button _coinsMediumButton;
    private Button _coinsLargeButton;
    private Button _coinsFreeButton;
    private Button _coinsAdButton;

    // Price labels for dynamic updates
    private Label _noAdsPrice;
    private Label _starterPackPrice;
    private Label _powerUpPrice;
    
    // Coin pack price labels
    private Label _coinsSmallPrice;
    private Label _coinsMediumPrice;
    private Label _coinsLargePrice;

    // Currency display
    private Label _currencyLabel;

    // Free pack cooldown tracking
    private const string FREE_PACK_COOLDOWN_KEY = "free_pack_cooldown";
    private const float FREE_PACK_COOLDOWN_HOURS = 24f; // 24 hours cooldown

    protected override void Awake()
    {
        base.Awake();

        // Get existing buttons
        _noAdsPurchaseButton = UIDocument.rootVisualElement.Q<Button>("buy-no-ads");
        _starterPackPurchaseButton = UIDocument.rootVisualElement.Q<Button>("buy-starter-pack");
        _powerUpPurchaseButton = UIDocument.rootVisualElement.Q<Button>("buy-power-pack");
        _closeButton = UIDocument.rootVisualElement.Q<Button>("close-button");

        // Get coin pack buttons
        _coinsSmallButton = UIDocument.rootVisualElement.Q<Button>("buy-coins-small");
        _coinsMediumButton = UIDocument.rootVisualElement.Q<Button>("buy-coins-medium");
        _coinsLargeButton = UIDocument.rootVisualElement.Q<Button>("buy-coins-large");
        _coinsFreeButton = UIDocument.rootVisualElement.Q<Button>("buy-coins-free");
        _coinsAdButton = UIDocument.rootVisualElement.Q<Button>("buy-coins-ad");

        // Get existing price labels
        _noAdsPrice = _noAdsPurchaseButton?.Q<Label>();
        _starterPackPrice = _starterPackPurchaseButton?.Q<Label>();
        _powerUpPrice = _powerUpPurchaseButton?.Q<Label>();

        // Get coin pack price labels
        _coinsSmallPrice = _coinsSmallButton?.Q<Label>();
        _coinsMediumPrice = _coinsMediumButton?.Q<Label>();
        _coinsLargePrice = _coinsLargeButton?.Q<Label>();

        // Get currency display
        _currencyLabel = UIDocument.rootVisualElement.Q<Label>("currency-label");

        // Subscribe to IAP Manager events
        IAPManager.SubscribeOnPurchaseModuleInitted(OnIAPManagerInitialized);
    }

    void OnEnable()
    {
        // Register existing buttons
        if (_noAdsPurchaseButton != null)
            _noAdsPurchaseButton.RegisterCallback<ClickEvent>(OnNoAdsPurchaseClicked);
        if (_starterPackPurchaseButton != null)
            _starterPackPurchaseButton.RegisterCallback<ClickEvent>(OnStarterPackPurchaseClicked);
        if (_powerUpPurchaseButton != null)
            _powerUpPurchaseButton.RegisterCallback<ClickEvent>(OnPowerUpPurchaseClicked);
        if (_closeButton != null)
            _closeButton.RegisterCallback<ClickEvent>(OnCloseButtonClicked);

        // Register coin pack buttons
        if (_coinsSmallButton != null)
            _coinsSmallButton.RegisterCallback<ClickEvent>(OnCoinsSmallPurchaseClicked);
        if (_coinsMediumButton != null)
            _coinsMediumButton.RegisterCallback<ClickEvent>(OnCoinsMediumPurchaseClicked);
        if (_coinsLargeButton != null)
            _coinsLargeButton.RegisterCallback<ClickEvent>(OnCoinsLargePurchaseClicked);
        if (_coinsFreeButton != null)
            _coinsFreeButton.RegisterCallback<ClickEvent>(OnCoinsFreeClicked);
        if (_coinsAdButton != null)
            _coinsAdButton.RegisterCallback<ClickEvent>(OnCoinsAdClicked);

        // Subscribe to purchase events
        IAPManager.PurchaseCompleted += OnPurchaseCompleted;
        IAPManager.PurchaseFailed += OnPurchaseFailed;

        // Update displays
        UpdateCurrencyDisplay();
        UpdateProductPrices();
        UpdateFreePackButton();
    }

    void OnDisable()
    {
        // Unregister existing buttons
        if (_noAdsPurchaseButton != null)
            _noAdsPurchaseButton.UnregisterCallback<ClickEvent>(OnNoAdsPurchaseClicked);
        if (_starterPackPurchaseButton != null)
            _starterPackPurchaseButton.UnregisterCallback<ClickEvent>(OnStarterPackPurchaseClicked);
        if (_powerUpPurchaseButton != null)
            _powerUpPurchaseButton.UnregisterCallback<ClickEvent>(OnPowerUpPurchaseClicked);
        if (_closeButton != null)
            _closeButton.UnregisterCallback<ClickEvent>(OnCloseButtonClicked);

        // Unregister coin pack buttons
        if (_coinsSmallButton != null)
            _coinsSmallButton.UnregisterCallback<ClickEvent>(OnCoinsSmallPurchaseClicked);
        if (_coinsMediumButton != null)
            _coinsMediumButton.UnregisterCallback<ClickEvent>(OnCoinsMediumPurchaseClicked);
        if (_coinsLargeButton != null)
            _coinsLargeButton.UnregisterCallback<ClickEvent>(OnCoinsLargePurchaseClicked);
        if (_coinsFreeButton != null)
            _coinsFreeButton.UnregisterCallback<ClickEvent>(OnCoinsFreeClicked);
        if (_coinsAdButton != null)
            _coinsAdButton.UnregisterCallback<ClickEvent>(OnCoinsAdClicked);

        // Unsubscribe from purchase events
        IAPManager.PurchaseCompleted -= OnPurchaseCompleted;
        IAPManager.PurchaseFailed -= OnPurchaseFailed;
    }

    private void OnIAPManagerInitialized()
    {
        UpdateProductPrices();
    }

    private void UpdateProductPrices()
    {
        if (!IAPManager.IsInitialized) return;

        // Update existing product prices
        var noAdsProduct = IAPManager.GetProductData(ProductKeyType.NoAds);
        if (noAdsProduct != null && _noAdsPrice != null)
        {
            _noAdsPrice.text = noAdsProduct.GetLocalPrice();
        }

        var starterPackProduct = IAPManager.GetProductData(ProductKeyType.StarterPack);
        if (starterPackProduct != null && _starterPackPrice != null)
        {
            _starterPackPrice.text = starterPackProduct.GetLocalPrice();
        }

        var powerUpProduct = IAPManager.GetProductData(ProductKeyType.PUPack);
        if (powerUpProduct != null && _powerUpPrice != null)
        {
            _powerUpPrice.text = powerUpProduct.GetLocalPrice();
        }

        // Update coin pack prices
        var coinsSmallProduct = IAPManager.GetProductData(ProductKeyType.GoldSmall);
        if (coinsSmallProduct != null && _coinsSmallPrice != null)
        {
            _coinsSmallPrice.text = coinsSmallProduct.GetLocalPrice();
        }

        var coinsMediumProduct = IAPManager.GetProductData(ProductKeyType.GoldMedium);
        if (coinsMediumProduct != null && _coinsMediumPrice != null)
        {
            _coinsMediumPrice.text = coinsMediumProduct.GetLocalPrice();
        }

        var coinsLargeProduct = IAPManager.GetProductData(ProductKeyType.GoldBig);
        if (coinsLargeProduct != null && _coinsLargePrice != null)
        {
            _coinsLargePrice.text = coinsLargeProduct.GetLocalPrice();
        }
    }

    private void UpdateCurrencyDisplay()
    {
        if (_currencyLabel != null)
        {
            int coinsAmount = CurrencyController.Get(CurrencyType.Coins);
            _currencyLabel.text = coinsAmount.ToString();
        }
    }

    private void UpdateFreePackButton()
    {
        if (_coinsFreeButton == null) return;

        float lastClaimTime = PlayerPrefs.GetFloat(FREE_PACK_COOLDOWN_KEY, 0f);
        float currentTime = Time.time;
        float timeSinceLastClaim = currentTime - lastClaimTime;
        float cooldownDuration = FREE_PACK_COOLDOWN_HOURS * 3600f; // Convert hours to seconds

        if (timeSinceLastClaim >= cooldownDuration)
        {
            _coinsFreeButton.SetEnabled(true);
            var buttonText = _coinsFreeButton.Q<Label>();
            if (buttonText != null) buttonText.text = "FREE";
        }
        else
        {
            _coinsFreeButton.SetEnabled(false);
            float remainingTime = cooldownDuration - timeSinceLastClaim;
            int remainingHours = Mathf.CeilToInt(remainingTime / 3600f);
            var buttonText = _coinsFreeButton.Q<Label>();
            if (buttonText != null) buttonText.text = $"{remainingHours}h";
        }
    }

    #region Button Click Handlers

    private void OnNoAdsPurchaseClicked(ClickEvent e)
    {
        Debug.Log("No Ads purchase button clicked");
        PlayClickFeedback();
        IAPManager.BuyProduct(ProductKeyType.NoAds);
    }

    private void OnStarterPackPurchaseClicked(ClickEvent e)
    {
        Debug.Log("Starter Pack purchase button clicked");
        PlayClickFeedback();
        IAPManager.BuyProduct(ProductKeyType.StarterPack);
    }

    private void OnPowerUpPurchaseClicked(ClickEvent e)
    {
        Debug.Log("Power Up purchase button clicked");
        PlayClickFeedback();
        IAPManager.BuyProduct(ProductKeyType.PUPack);
    }

    private void OnCoinsSmallPurchaseClicked(ClickEvent e)
    {
        Debug.Log("Small coins pack purchase button clicked");
        PlayClickFeedback();
        IAPManager.BuyProduct(ProductKeyType.GoldSmall);
    }

    private void OnCoinsMediumPurchaseClicked(ClickEvent e)
    {
        Debug.Log("Medium coins pack purchase button clicked");
        PlayClickFeedback();
        IAPManager.BuyProduct(ProductKeyType.GoldMedium);
    }

    private void OnCoinsLargePurchaseClicked(ClickEvent e)
    {
        Debug.Log("Large coins pack purchase button clicked");
        PlayClickFeedback();
        IAPManager.BuyProduct(ProductKeyType.GoldBig);
    }

    private void OnCoinsFreeClicked(ClickEvent e)
    {
        Debug.Log("Free coins button clicked");
        PlayClickFeedback();

        // Check cooldown
        float lastClaimTime = PlayerPrefs.GetFloat(FREE_PACK_COOLDOWN_KEY, 0f);
        float currentTime = Time.time;
        float timeSinceLastClaim = currentTime - lastClaimTime;
        float cooldownDuration = FREE_PACK_COOLDOWN_HOURS * 3600f;

        if (timeSinceLastClaim >= cooldownDuration)
        {
            // Grant free coins
            CurrencyController.Add(CurrencyType.Coins, 100);
            
            // Set cooldown
            PlayerPrefs.SetFloat(FREE_PACK_COOLDOWN_KEY, currentTime);
            PlayerPrefs.Save();
            
            // Update displays
            UpdateCurrencyDisplay();
            UpdateFreePackButton();
            
            Debug.Log("Free coins granted: 100");
        }
        else
        {
            float remainingTime = cooldownDuration - timeSinceLastClaim;
            int remainingHours = Mathf.CeilToInt(remainingTime / 3600f);
            SystemMessage.ShowMessage($"Free pack available in {remainingHours} hours");
        }
    }

    private void OnCoinsAdClicked(ClickEvent e)
    {
        Debug.Log("Watch AD for coins button clicked");
        PlayClickFeedback();

        // Show rewarded video ad
        AdsManager.ShowRewardBasedVideo(success =>
        {
            if (success)
            {
                // Grant AD reward coins
                CurrencyController.Add(CurrencyType.Coins, 100);
                UpdateCurrencyDisplay();
                Debug.Log("AD reward coins granted: 100");
            }
            else
            {
                SystemMessage.ShowMessage("Ad not available. Please try again later.");
            }
        });
    }

    private void OnCloseButtonClicked(ClickEvent e)
    {
        Debug.Log("Store close button clicked");
        PlayClickFeedback();
        Fade.FadeOut();
    }

    #endregion

    private void PlayClickFeedback()
    {
#if MODULE_HAPTIC
        Haptic.Play(Haptic.HAPTIC_LIGHT);
#endif
        AudioController.PlaySound(AudioController.AudioClips.buttonSound);
    }

    private void OnPurchaseCompleted(ProductKeyType productKeyType)
    {
        Debug.Log($"Purchase completed: {productKeyType}");
        
        // Update currency display after purchase
        UpdateCurrencyDisplay();
        
        // Handle specific purchase rewards
        switch (productKeyType)
        {
            case ProductKeyType.NoAds:
                // Handle no ads purchase
                break;
            case ProductKeyType.StarterPack:
                // Handle starter pack purchase - give rewards
                CurrencyController.Add(CurrencyType.Coins, 400);
                break;
            case ProductKeyType.PUPack:
                // Handle power up pack purchase
                break;
            case ProductKeyType.GoldSmall:
                // Give 150 coins for small pack
                CurrencyController.Add(CurrencyType.Coins, 150);
                break;
            case ProductKeyType.GoldMedium:
                // Give 450 coins for medium pack
                CurrencyController.Add(CurrencyType.Coins, 450);
                break;
            case ProductKeyType.GoldBig:
                // Give 1000 coins for large pack
                CurrencyController.Add(CurrencyType.Coins, 1000);
                break;
        }

        // Update currency display again after rewards
        UpdateCurrencyDisplay();
    }

    private void OnPurchaseFailed(ProductKeyType productKeyType, PurchaseFailureReason failureReason)
    {
        Debug.LogWarning($"Purchase failed: {productKeyType}, Reason: {failureReason}");
        SystemMessage.ShowMessage($"Purchase failed: {failureReason}");
    }

    // Static method to show the Luna store
    public static void ShowLunaStore()
    {
        IAPStoreView storeView = FindObjectOfType<IAPStoreView>();
        if (storeView != null)
        {
            storeView.Fade.FadeIn();
        }
        else
        {
            Debug.LogWarning("IAPStoreView not found in scene!");
        }
    }

    // Static method to hide the Luna store
    public static void HideLunaStore()
    {
        IAPStoreView storeView = FindObjectOfType<IAPStoreView>();
        if (storeView != null)
        {
            storeView.Fade.FadeOut();
        }
    }
}
