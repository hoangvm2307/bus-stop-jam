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

    // Price labels for dynamic updates
    private Label _noAdsPrice;
    private Label _starterPackPrice;
    private Label _powerUpPrice;

    // Currency display
    private Label _currencyLabel;

    protected override void Awake()
    {
        base.Awake();

        // Get buttons
        _noAdsPurchaseButton = UIDocument.rootVisualElement.Q<Button>("buy-no-ads");
        _starterPackPurchaseButton = UIDocument.rootVisualElement.Q<Button>("buy-starter-pack");
        _powerUpPurchaseButton = UIDocument.rootVisualElement.Q<Button>("buy-power-pack");
        _closeButton = UIDocument.rootVisualElement.Q<Button>("close-button");

        // Get price labels
        _noAdsPrice = _noAdsPurchaseButton?.Q<Label>();
        _starterPackPrice = _starterPackPurchaseButton?.Q<Label>();
        _powerUpPrice = _powerUpPurchaseButton?.Q<Label>();

        // Get currency display
        _currencyLabel = UIDocument.rootVisualElement.Q<Label>("currency-label");

        // Subscribe to IAP Manager events
        IAPManager.SubscribeOnPurchaseModuleInitted(OnIAPManagerInitialized);
    }

    void OnEnable()
    {
        if (_noAdsPurchaseButton != null)
            _noAdsPurchaseButton.RegisterCallback<ClickEvent>(OnNoAdsPurchaseClicked);
        if (_starterPackPurchaseButton != null)
            _starterPackPurchaseButton.RegisterCallback<ClickEvent>(OnStarterPackPurchaseClicked);
        if (_powerUpPurchaseButton != null)
            _powerUpPurchaseButton.RegisterCallback<ClickEvent>(OnPowerUpPurchaseClicked);
        if (_closeButton != null)
            _closeButton.RegisterCallback<ClickEvent>(OnCloseButtonClicked);

        // Subscribe to purchase events
        IAPManager.PurchaseCompleted += OnPurchaseCompleted;
        IAPManager.PurchaseFailed += OnPurchaseFailed;

        // Update currency display
        UpdateCurrencyDisplay();
        // Update product prices
        UpdateProductPrices();
    }

    void OnDisable()
    {
        if (_noAdsPurchaseButton != null)
            _noAdsPurchaseButton.UnregisterCallback<ClickEvent>(OnNoAdsPurchaseClicked);
        if (_starterPackPurchaseButton != null)
            _starterPackPurchaseButton.UnregisterCallback<ClickEvent>(OnStarterPackPurchaseClicked);
        if (_powerUpPurchaseButton != null)
            _powerUpPurchaseButton.UnregisterCallback<ClickEvent>(OnPowerUpPurchaseClicked);
        if (_closeButton != null)
            _closeButton.UnregisterCallback<ClickEvent>(OnCloseButtonClicked);

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

        // Update No Ads price
        var noAdsProduct = IAPManager.GetProductData(ProductKeyType.NoAds);
        if (noAdsProduct != null && _noAdsPrice != null)
        {
            _noAdsPrice.text = noAdsProduct.GetLocalPrice();
        }

        // Update Starter Pack price
        var starterPackProduct = IAPManager.GetProductData(ProductKeyType.StarterPack);
        if (starterPackProduct != null && _starterPackPrice != null)
        {
            _starterPackPrice.text = starterPackProduct.GetLocalPrice();
        }

        // Update Power Up Pack price
        var powerUpProduct = IAPManager.GetProductData(ProductKeyType.PUPack);
        if (powerUpProduct != null && _powerUpPrice != null)
        {
            _powerUpPrice.text = powerUpProduct.GetLocalPrice();
        }
    }

    private void UpdateCurrencyDisplay()
    {
        if (_currencyLabel != null)
        {
            // Get current coins amount from currency system
            int coinsAmount = CurrencyController.Get(CurrencyType.Coins);
            _currencyLabel.text = coinsAmount.ToString();
        }
    }

    private void OnNoAdsPurchaseClicked(ClickEvent e)
    {
        Debug.Log("No Ads purchase button clicked");
        
#if MODULE_HAPTIC
        Haptic.Play(Haptic.HAPTIC_LIGHT);
#endif
        AudioController.PlaySound(AudioController.AudioClips.buttonSound);
        
        IAPManager.BuyProduct(ProductKeyType.NoAds);
    }

    private void OnStarterPackPurchaseClicked(ClickEvent e)
    {
        Debug.Log("Starter Pack purchase button clicked");
        
#if MODULE_HAPTIC
        Haptic.Play(Haptic.HAPTIC_LIGHT);
#endif
        AudioController.PlaySound(AudioController.AudioClips.buttonSound);
        
        IAPManager.BuyProduct(ProductKeyType.StarterPack);
    }

    private void OnPowerUpPurchaseClicked(ClickEvent e)
    {
        Debug.Log("Power Up purchase button clicked");
        
#if MODULE_HAPTIC
        Haptic.Play(Haptic.HAPTIC_LIGHT);
#endif
        AudioController.PlaySound(AudioController.AudioClips.buttonSound);
        
        IAPManager.BuyProduct(ProductKeyType.PUPack);
    }

    private void OnCloseButtonClicked(ClickEvent e)
    {
        Debug.Log("Store close button clicked");
        
#if MODULE_HAPTIC
        Haptic.Play(Haptic.HAPTIC_LIGHT);
#endif
        AudioController.PlaySound(AudioController.AudioClips.buttonSound);
        
        // Hide the Luna UI store
        Fade.FadeOut();
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
        }

        // Update currency display again after rewards
        UpdateCurrencyDisplay();
    }

    private void OnPurchaseFailed(ProductKeyType productKeyType, PurchaseFailureReason failureReason)
    {
        Debug.LogWarning($"Purchase failed: {productKeyType}, Reason: {failureReason}");
        
        // Could show an error message to user here
        SystemMessage.ShowMessage($"Purchase failed: {failureReason}");
    }

    // Static method to show the Luna store
    public static void ShowLunaStore()
    {
        // Find the IAPStoreView component in the scene
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
