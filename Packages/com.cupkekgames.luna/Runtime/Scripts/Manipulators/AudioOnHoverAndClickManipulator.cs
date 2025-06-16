using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna
{
  public class AudioOnHoverAndClickManipulator : Manipulator
  {
    private UIInteractableAudioHandler _audioHandler;

    private bool _canPlayHover = true;

    public AudioOnHoverAndClickManipulator(UIInteractableAudioHandler audioHandler)
    {
      _audioHandler = audioHandler;
    }

    protected override void RegisterCallbacksOnTarget()
    {
      target.RegisterCallback<ClickEvent>(OnClick);
      target.RegisterCallback<NavigationSubmitEvent>(OnClick);

      target.RegisterCallback<MouseEnterEvent>(OnHover);
      target.RegisterCallback<FocusEvent>(OnHover);

      target.RegisterCallback<MouseLeaveEvent>(ResetHover);
      target.RegisterCallback<BlurEvent>(ResetHover);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
      target.UnregisterCallback<ClickEvent>(OnClick);
      target.UnregisterCallback<NavigationSubmitEvent>(OnClick);

      target.UnregisterCallback<MouseEnterEvent>(OnHover);
      target.UnregisterCallback<FocusEvent>(OnHover);

      target.UnregisterCallback<MouseLeaveEvent>(ResetHover);
      target.UnregisterCallback<BlurEvent>(ResetHover);
    }

    private void OnClick(EventBase e)
    {
      _audioHandler.PlayClick();
    }

    private void OnHover(EventBase e)
    {
      if (_canPlayHover)
      {
        _audioHandler.PlayHover();

        _canPlayHover = false;
      }
    }

    private void ResetHover(EventBase e)
    {
      _canPlayHover = true;
    }
  }
}
