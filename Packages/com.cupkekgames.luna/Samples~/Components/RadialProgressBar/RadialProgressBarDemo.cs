using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna.Demo.Components
{
  public class RadialProgressBarDemo : MonoBehaviour
  {
    private UIDocument _uiDocument;
    private RadialProgressBar _radialProgressBar;
    private Slider _slider;
    private Button _buttonDamage;
    private Button _buttonHeal;

    private void Awake()
    {
      _uiDocument = GetComponent<UIDocument>();

      _radialProgressBar = _uiDocument.rootVisualElement.Q<RadialProgressBar>();
      _slider = _uiDocument.rootVisualElement.Q<Slider>();
      _buttonDamage = _uiDocument.rootVisualElement.Q<Button>("Damage");
      _buttonHeal = _uiDocument.rootVisualElement.Q<Button>("Heal");

      _radialProgressBar.Instant = true;
      _radialProgressBar.TargetValue = 1;
      _radialProgressBar.TargetIndicator = 1;
      _radialProgressBar.Instant = false;
    }

    private void OnEnable()
    {
      _slider.RegisterValueChangedCallback(OnValueChanged);

      _buttonDamage.clicked += Damage;
      _buttonHeal.clicked += Heal;
    }

    private void OnDisable()
    {
      _slider.UnregisterValueChangedCallback(OnValueChanged);

      _buttonDamage.clicked -= Damage;
      _buttonHeal.clicked -= Heal;
    }

    private void Damage()
    {
      _radialProgressBar.TargetValue -= 0.1f;
      _radialProgressBar.TargetIndicator -= 0.1f;
    }

    private void Heal()
    {
      _radialProgressBar.TargetValue += 0.1f;
      _radialProgressBar.TargetIndicator += 0.1f;
    }

    private void OnValueChanged(ChangeEvent<float> evt)
    {
      _radialProgressBar.TargetValue = evt.newValue;
      _radialProgressBar.TargetIndicator = evt.newValue;
    }
  }
}
