using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna.Demo.Components
{
  public class ProgressBarOverwatch : MonoBehaviour
  {
    private UIDocument _uiDocument;
    private ProgressBar _progressBar;
    private Button _buttonDamage;
    private Button _buttonHeal;
    private Button _buttonShield;
    private Button _buttonArmor;
    private Label _labelHealth;
    private Label _labelMaxHealth;
    private Label _labelShield;
    private Label _labelArmor;
    // State
    private const int _maxHealth = 1000;
    private int _health = 1000;
    private int _shield = 0;
    private int _armor = 0;

    private void Awake()
    {
      _uiDocument = GetComponent<UIDocument>();

      _progressBar = _uiDocument.rootVisualElement.Q<ProgressBar>();

      _buttonDamage = _uiDocument.rootVisualElement.Q<Button>("Damage");
      _buttonHeal = _uiDocument.rootVisualElement.Q<Button>("Heal");
      _buttonShield = _uiDocument.rootVisualElement.Q<Button>("Shield");
      _buttonArmor = _uiDocument.rootVisualElement.Q<Button>("Armor");

      _labelHealth = _uiDocument.rootVisualElement.Q<Label>("Health");
      _labelHealth.text = _health.ToString();

      _labelMaxHealth = _uiDocument.rootVisualElement.Q<Label>("MaxHealth");
      _labelMaxHealth.text = _maxHealth.ToString();

      _labelShield = _uiDocument.rootVisualElement.Q<Label>("Shield");
      _labelShield.text = _shield.ToString();

      _labelArmor = _uiDocument.rootVisualElement.Q<Label>("Armor");
      _labelArmor.text = _armor.ToString();
    }

    private void Start()
    {
      // Set instant to true to avoid animation at start
      _progressBar.InstantPositive = true;
      _progressBar.InstantNegative = true;

      _progressBar.Indicator.TargetValue = 1;
      _progressBar.RebuildProgressSegments();
      UpdateProgressBar();

      // SetOverlayRepeatAmount() doesn't work in Start because width is not calculated on first frame.
      // Since we know width is 900px, we can split it into 10 segments by setting OverlayImageWidth to 90.
      // _progressBar.OverlayImageWidth = 90;
      // We already do it in inspector, so we don't need to do it here.

      _progressBar.InstantPositive = false; // We don't want animation when healing
      _progressBar.InstantNegative = false;
    }

    private void OnEnable()
    {
      _buttonDamage.clicked += Damage;
      _buttonHeal.clicked += Heal;
      _buttonShield.clicked += Shield;
      _buttonArmor.clicked += Armor;
    }

    private void OnDisable()
    {
      _buttonDamage.clicked -= Damage;
      _buttonHeal.clicked -= Heal;
      _buttonShield.clicked -= Shield;
      _buttonArmor.clicked -= Armor;
    }

    private void Damage()
    {
      if (_armor > 0)
      {
        _armor -= 100;
      }
      else if (_shield > 0)
      {
        _shield -= 100;
      }
      else
      {
        _health -= 100;
        if (_health < 0)
        {
          _health = 0;
        }
      }
      UpdateProgressBar();
    }

    private void Heal()
    {
      _health += 100;
      if (_health > _maxHealth)
      {
        _health = _maxHealth;
      }

      UpdateProgressBar();
    }
    private void Shield()
    {
      _shield += 100;

      UpdateProgressBar();
    }
    private void Armor()
    {
      _armor += 100;

      UpdateProgressBar();
    }

    private void UpdateProgressBar()
    {
      // Update labels
      _labelHealth.text = _health.ToString();
      _labelShield.text = _shield.ToString();
      _labelArmor.text = _armor.ToString();

      float total = _maxHealth + _shield + _armor;

      float healthPercent = _health / total;
      float shieldPercent = _shield / total;
      float armorPercent = _armor / total;

      _progressBar.Progress[0].TargetValue = healthPercent;
      _progressBar.Progress[1].TargetValue = shieldPercent;
      _progressBar.Progress[2].TargetValue = armorPercent;
      _progressBar.UpdateProgressSegments();
      _progressBar.PlayProgress();

      _progressBar.Indicator.TargetValue = healthPercent + shieldPercent + armorPercent;
      _progressBar.PlayIndicator();

      _progressBar.SetOverlayRepeatAmount((int)((total / 100) + 0.5f));
    }
  }
}
