using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

#if UNITY_INPUT
using UnityEngine.InputSystem;
#endif

namespace CupkekGames.Luna
{
  public class LoadingViewController : UIViewComponent
  {
    [Header("Different Easing for fade in and out")]
    [SerializeField] private EasingMode _easeModeIn;
    [SerializeField] private EasingMode _easeModeOut;
    [Header("Tips Settings")]
    [SerializeField] private List<string> _tips;
    [SerializeField] private long _tipIntervalMs;
#if UNITY_INPUT
    [Header("Dependency for Next Tip Input")]
    [SerializeField] private PlayerInput _playerInput;
#endif

    private List<VisualElement> _loadingPage;
    private VisualElement _tipsContainer;
    private Label _speech;
    private InputPrompt _inputPromptNextTip;
    private InputPrompt _inputPromptContinue;
    public InputPrompt InputPromptContinue => _inputPromptContinue;

    private IVisualElementScheduledItem _tipSchedule;
    private int _tipIndex = -1;

#if UNITY_INPUT
    private InputAction _nextTipAction;
#endif

    protected override void Awake()
    {
      base.Awake();

      _loadingPage = ParentElement.Query<VisualElement>("LoadingPage").ToList();
      _tipsContainer = ParentElement.Q<VisualElement>("TipsContainer");
      _speech = ParentElement.Q<Label>("Speech");
      _inputPromptNextTip = ParentElement.Q<InputPrompt>("InputPromptNextTip");
      _inputPromptContinue = ParentElement.Q<InputPrompt>("InputPromptContinue");

#if UNITY_INPUT
      if (_inputPromptNextTip != null && _playerInput != null)
      {
        _nextTipAction = _playerInput.actions[_inputPromptNextTip.InputActionName];
      }
#endif
    }

    private void OnEnable()
    {
      SceneTransition.LoadingScreenToggleEvent += OnLoadingScreenToggle;
      SetInputContinueVisiblity(false, false);
    }

    private void OnDisable()
    {
      SceneTransition.LoadingScreenToggleEvent -= OnLoadingScreenToggle;
      StopTipSchedule();

#if UNITY_INPUT
      if (_nextTipAction != null)
      {
        _nextTipAction.performed -= OnNextTipInput;
      }
#endif

      if (_inputPromptNextTip != null)
      {
        _inputPromptNextTip.clicked -= OnNextTip;
      }
    }

    private void Start()
    {
      _inputPromptNextTip?.OnAttach();
      _inputPromptContinue?.OnAttach();
    }

    public virtual void OnLoadingScreenToggle(bool fadeIn, float transitionDuration)
    {
      if (fadeIn)
      {
        ShowTips(false);
        SetInputContinueVisiblity(false, false);
        StartTipSchedule();

#if UNITY_INPUT
        if (_nextTipAction != null)
        {
          _nextTipAction.performed += OnNextTipInput;
        }
#endif

        if (_inputPromptNextTip != null)
        {
          _inputPromptNextTip.clicked += OnNextTip;
        }

        Fade.SetDuration(transitionDuration);
        Fade.SetEasing(_easeModeIn);
        Fade.FadeIn();
      }
      else
      {
        StopTipSchedule();

#if UNITY_INPUT
        if (_nextTipAction != null)
        {
          _nextTipAction.performed -= OnNextTipInput;
        }
#endif

        if (_inputPromptNextTip != null)
        {
          _inputPromptNextTip.clicked -= OnNextTip;
        }

        Fade.SetDuration(transitionDuration);
        Fade.SetEasing(_easeModeOut);
        Fade.FadeOut();
      }
    }

    public void RandomTip()
    {
      if (_speech == null)
      {
        return;
      }

      if (_tips.Count == 0)
      {
        _speech.text = "";
        return;
      }

      if (_tips.Count == 1)
      {
        _tipIndex = 0;
      }
      else
      {
        int newIndex = UnityEngine.Random.Range(0, _tips.Count - 1);
        _tipIndex = newIndex < _tipIndex ? newIndex : newIndex + 1;
      }

      _speech.text = _tips[_tipIndex];
    }

    public void StartTipSchedule()
    {
      if (_speech == null)
      {
        return;
      }

      _tipSchedule = _speech.schedule.Execute(RandomTip).Every(_tipIntervalMs);
    }
    public void StopTipSchedule()
    {
      if (_tipSchedule != null)
      {
        _tipSchedule.Pause();
        _tipSchedule = null;
      }
    }
    public void SetInputContinueVisiblity(bool visible, bool withTransition)
    {
      if (withTransition)
      {
        foreach (var ve in _loadingPage)
        {
          ve.AddToClassList("with-transition");
        }
      }
      else
      {
        foreach (var ve in _loadingPage)
        {
          ve.RemoveFromClassList("with-transition");
        }
      }

      if (visible)
      {
        foreach (var ve in _loadingPage)
        {
          ve.AddToClassList("show-input");
        }
      }
      else
      {
        foreach (var ve in _loadingPage)
        {
          ve.RemoveFromClassList("show-input");
        }
      }
    }

#if UNITY_INPUT
    private void OnNextTipInput(InputAction.CallbackContext context)
    {
      OnNextTip();
    }
#endif

    private void OnNextTip()
    {
      StopTipSchedule();
      StartTipSchedule();
    }

    public void ShowTips(bool show)
    {
      if (show)
      {
        if (_tipsContainer != null)
        {
          _tipsContainer.style.visibility = Visibility.Visible;
        }
        if (_inputPromptNextTip != null)
        {
          _inputPromptNextTip.style.visibility = Visibility.Visible;
          _inputPromptNextTip.Hide = false;
          _inputPromptNextTip.OnAttach();
        }
      }
      else
      {
        if (_tipsContainer != null)
        {
          _tipsContainer.style.visibility = Visibility.Hidden;
        }
        if (_inputPromptNextTip != null)
        {
          _inputPromptNextTip.style.visibility = Visibility.Hidden;
          _inputPromptNextTip.Hide = true;
        }
      }
    }
  }
}