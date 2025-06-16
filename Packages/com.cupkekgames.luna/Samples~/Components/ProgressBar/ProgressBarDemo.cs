using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna.Demo.Components
{
  public class ProgressBarDemo : MonoBehaviour
  {
    private UIDocument _uiDocument;
    private ProgressBar _progressBar;
    private Button _buttonDamage;
    private Button _buttonHeal;

    private void Awake()
    {
      _uiDocument = GetComponent<UIDocument>();

      _progressBar = _uiDocument.rootVisualElement.Q<ProgressBar>();

      _buttonDamage = _uiDocument.rootVisualElement.Q<Button>("Damage");
      _buttonHeal = _uiDocument.rootVisualElement.Q<Button>("Heal");
    }

    private void Start()
    {
      // Set instant to true to avoid animation at start
      _progressBar.InstantPositive = true;
      _progressBar.InstantNegative = true;

      _progressBar.Indicator.TargetValue = 1;
      _progressBar.PlayIndicator(); // You must call PlayIndicator to visually apply the value

      _progressBar.Progress[0].TargetValue = 1f;
      _progressBar.RebuildProgressSegments(); // Must be called after modifying the size of the Progress array. Not needed here, included for documentation purposes.
      _progressBar.PlayProgress();  // You must call PlayIndicator to visually apply the value

      // Set instant back to false to play animation
      _progressBar.InstantPositive = false;
      _progressBar.InstantNegative = false;
    }

    private void OnEnable()
    {
      _buttonDamage.clicked += Damage;
      _buttonHeal.clicked += Heal;
    }

    private void OnDisable()
    {
      _buttonDamage.clicked -= Damage;
      _buttonHeal.clicked -= Heal;
    }

    private void Damage()
    {
      _progressBar.Progress[0].TargetValue -= 0.1f;
      _progressBar.Indicator.TargetValue = _progressBar.Progress[0].TargetValue;

      UpdateProgressBar();
    }

    private void Heal()
    {
      _progressBar.Progress[0].TargetValue += 0.1f;
      _progressBar.Indicator.TargetValue = _progressBar.Progress[0].TargetValue;

      UpdateProgressBar();
    }

    private void UpdateProgressBar()
    {
      _progressBar.UpdateProgressSegments(); // After updating, do NOT call RebuildProgressSegments. Instead, call UpdateProgressSegments.
      _progressBar.PlayProgress(); // Visually apply the value of progress bars

      _progressBar.PlayIndicator(); // Visually apply the value of the indicator bar
    }
  }
}
