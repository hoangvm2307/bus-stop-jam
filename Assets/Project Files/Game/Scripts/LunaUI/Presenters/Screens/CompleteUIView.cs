using CupkekGames.Luna;
using UnityEngine;
using UnityEngine.UIElements;
using Watermelon;
using System.Collections;
using Watermelon.BusStop;
using CupkekGames.Systems.UI;

public class CompleteUIView : UIViewComponent
{
    [Header("Reward Settings")]
    [SerializeField] private int rewardMultiplier = 3;
    [SerializeField] private float rewardAnimationDuration = 0.3f;
    [SerializeField] private float currencySpawnCount = 10;

    // UI Elements
    private Label _currencyAmount;
    private Label _levelText1;
    private Label _levelText2;
    private Label _rewardAmount;
    private Button _watchVideoBtn;
    private Button _noThanksBtn;
    private Label _noThanksText;
    
    // Containers for animations
    private VisualElement _rewardContainer;
    private VisualElement _currencyContainer;

    // State tracking
    private int _currentReward;
    private bool _hasWatchedAd = false;

    // Constants
    private const string NO_THANKS_TEXT = "NO, THANKS";
    private const string CONTINUE_TEXT = "CONTINUE";

    protected override void Awake()
    {
        base.Awake();

        // Get UI elements
        _currencyAmount = UIDocument.rootVisualElement.Q<Label>("currency-amount");
        _levelText1 = UIDocument.rootVisualElement.Q<Label>("level-text-1");
        _levelText2 = UIDocument.rootVisualElement.Q<Label>("level-text-2");
        _rewardAmount = UIDocument.rootVisualElement.Q<Label>("reward-amount");
        _watchVideoBtn = UIDocument.rootVisualElement.Q<Button>("watch-video-btn");
        _noThanksBtn = UIDocument.rootVisualElement.Q<Button>("no-thanks-btn");
        _noThanksText = UIDocument.rootVisualElement.Q<Label>("no-thanks-text");

        // Get containers
        _rewardContainer = UIDocument.rootVisualElement.Q<VisualElement>("reward-container");
        _currencyContainer = UIDocument.rootVisualElement.Q<VisualElement>("currency-container");
    }

    void OnEnable()
    {
        // Register button events
        if (_watchVideoBtn != null)
            _watchVideoBtn.RegisterCallback<ClickEvent>(OnWatchVideoClicked);
        if (_noThanksBtn != null)
            _noThanksBtn.RegisterCallback<ClickEvent>(OnNoThanksClicked);

        // Initialize display
        InitializeCompleteScreen();
    }

    void OnDisable()
    {
        // Unregister button events
        if (_watchVideoBtn != null)
            _watchVideoBtn.UnregisterCallback<ClickEvent>(OnWatchVideoClicked);
        if (_noThanksBtn != null)
            _noThanksBtn.UnregisterCallback<ClickEvent>(OnNoThanksClicked);
    }

    #region Initialization

    private void InitializeCompleteScreen()
    {
        // Update currency display
        UpdateCurrencyDisplay();

        // Get current level reward
        _currentReward = GetCurrentLevelReward();

        // Reset state
        _hasWatchedAd = false;
        _noThanksText.text = NO_THANKS_TEXT;

        // Display initial reward
        DisplayReward(_currentReward);

        // Add pulse effect to reward
        StartCoroutine(AddPulseEffectDelayed());
    }

    private int GetCurrentLevelReward()
    {
        // Get reward from LevelController or default value
        if (LevelController.CurrentReward > 0)
        {
            return LevelController.CurrentReward;
        }
        
        // Fallback calculation based on current level
        int currentLevel = GetCurrentLevel();
        return Mathf.Max(50, currentLevel * 10); // Minimum 50 coins, increasing by 10 per level
    }

    private int GetCurrentLevel()
    {
        // Try to get from LevelController or PlayerPrefs
        return PlayerPrefs.GetInt("CurrentLevel", 1);
    }

    #endregion

    #region Display Updates

    private void UpdateCurrencyDisplay()
    {
        if (_currencyAmount != null)
        {
            int currentCoins = CurrencyController.Get(CurrencyType.Coins);
            _currencyAmount.text = FormatCurrency(currentCoins);
        }
    }

    private void DisplayReward(int amount)
    {
        if (_rewardAmount != null)
        {
            _rewardAmount.text = amount.ToString();
        }
    }

    private string FormatCurrency(int amount)
    {
        // Format large numbers (e.g., 1000 -> 1K, 1000000 -> 1M)
        if (amount >= 1000000)
            return (amount / 1000000.0f).ToString("F1") + "M";
        else if (amount >= 1000)
            return (amount / 1000.0f).ToString("F1") + "K";
        else
            return amount.ToString();
    }

    #endregion

    #region Button Event Handlers

    private void OnWatchVideoClicked(ClickEvent e)
    {
        Debug.Log("Watch Video button clicked");
        PlayClickFeedback();

        // Disable buttons during ad
        SetButtonsInteractable(false);

        // Show rewarded video ad
        AdsManager.ShowRewardBasedVideo(OnAdCompleted);
    }

    private void OnNoThanksClicked(ClickEvent e)
    {
        Debug.Log("No Thanks button clicked");
        PlayClickFeedback();

        if (_hasWatchedAd)
        {
            // If already watched ad, this is "CONTINUE" button
            ContinueToNextLevel();
        }
        else
        {
            // Give base reward and continue
            GiveReward(_currentReward, false);
        }
    }

    #endregion

    #region Ad Handling

    private void OnAdCompleted(bool success)
    {
        if (success)
        {
            Debug.Log("Ad watched successfully - giving multiplied reward");
            
            // Mark as watched
            _hasWatchedAd = true;
            
            // Give multiplied reward
            int multipliedReward = _currentReward * rewardMultiplier;
            GiveReward(multipliedReward, true);
            
            // Change button text to continue
            if (_noThanksText != null)
            {
                _noThanksText.text = CONTINUE_TEXT;
            }
            
            // Hide watch video button
            if (_watchVideoBtn != null)
            {
                _watchVideoBtn.style.display = DisplayStyle.None;
            }
        }
        else
        {
            Debug.Log("Ad failed or was skipped");
            
            // Re-enable buttons and continue normally
            SetButtonsInteractable(true);
            
            // Show message
            SystemMessage.ShowMessage("Ad not available. Please try again later.");
        }
    }

    #endregion

    #region Reward Handling

    private void GiveReward(int amount, bool isMultiplied)
    {
        Debug.Log($"Giving reward: {amount} coins (multiplied: {isMultiplied})");

        // Update reward display with animation
        StartCoroutine(AnimateRewardGiven(amount, isMultiplied));
    }

    private IEnumerator AnimateRewardGiven(int amount, bool isMultiplied)
    {
        // Update reward display
        DisplayReward(amount);
        
        // Add currency to player
        CurrencyController.Add(CurrencyType.Coins, amount);
        
        // Update currency display
        UpdateCurrencyDisplay();
        
        // Wait for animation
        yield return new WaitForSeconds(0.5f);
        
        // Re-enable no thanks button
        SetButtonsInteractable(true);
        
        // If not multiplied, allow continuing immediately
        if (!isMultiplied)
        {
            yield return new WaitForSeconds(1f);
            ContinueToNextLevel();
        }
    }

    #endregion

    #region Navigation

    private void ContinueToNextLevel()
    {
        Debug.Log("Continuing to next level");
        
        // Unlock life (from original UIComplete logic)
        LivesSystem.UnlockLife(false);
        
        // Hide this screen with fade out
        Fade.FadeOut();
        
        // Optional: Destroy the CompleteUI instance after fade out to free memory
        // This ensures fresh state for next level completion
        StartCoroutine(DestroyAfterFadeOut());
        
        // Load next level
        GameController.LoadNextLevel();
    }
    
    /// <summary>
    /// Destroy the CompleteUI instance after fade out animation completes
    /// </summary>
    private IEnumerator DestroyAfterFadeOut()
    {
        // Wait for fade out animation to complete
        yield return new WaitForSeconds(0.5f);
        
        // Destroy this CompleteUI instance to ensure fresh state next time
        if (UIPrefabLoaderString.Instance != null)
        {
            UIPrefabLoaderString.Instance.DestroyAllOf("CompleteUI");
            Debug.Log("CompleteUI instance destroyed after level completion");
        }
    }

    #endregion

    #region Helper Methods

    private void SetButtonsInteractable(bool interactable)
    {
        if (_watchVideoBtn != null)
            _watchVideoBtn.SetEnabled(interactable);
        if (_noThanksBtn != null)
            _noThanksBtn.SetEnabled(interactable);
    }

    private void PlayClickFeedback()
    {
#if MODULE_HAPTIC
        Haptic.Play(Haptic.HAPTIC_LIGHT);
#endif
        AudioController.PlaySound(AudioController.AudioClips.buttonSound);
    }

    private IEnumerator AddPulseEffectDelayed()
    {
        yield return new WaitForSeconds(1f);
        
        if (_rewardContainer != null)
        {
            _rewardContainer.AddToClassList("pulse-effect");
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Set the reward amount manually
    /// </summary>
    public void SetReward(int amount)
    {
        _currentReward = amount;
        DisplayReward(amount);
    }

    /// <summary>
    /// Force refresh the currency display
    /// </summary>
    public void RefreshCurrencyDisplay()
    {
        UpdateCurrencyDisplay();
    }

    /// <summary>
    /// Show the complete screen with custom reward
    /// </summary>
    public static void ShowCompleteScreen(int reward = -1)
    {
        // Find the CompleteUIView instance
        CompleteUIView completeView = FindObjectOfType<CompleteUIView>();
        if (completeView != null)
        {
            if (reward > 0)
            {
                completeView.SetReward(reward);
            }
            completeView.Fade.FadeIn();
        }
        else
        {
            Debug.LogWarning("CompleteUIView not found in scene!");
        }
    }

    /// <summary>
    /// Hide the complete screen
    /// </summary>
    public static void HideCompleteScreen()
    {
        CompleteUIView completeView = FindObjectOfType<CompleteUIView>();
        if (completeView != null)
        {
            completeView.Fade.FadeOut();
        }
    }

    #endregion
}
