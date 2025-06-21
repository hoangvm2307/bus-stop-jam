

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CupkekGames.Luna;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Systems.UI
{
  public class NotificationView : UIViewComponent
  {
    // Settings
    [Header("Settings")]
    [SerializeField] private int _maxNotifications = 5;
    [SerializeField] private string _elementNameContainer = "NotificationContainer";
    [SerializeField] private string _elementNameModalButton = "NotificationModalButton";
    [SerializeField] private string _prefabKeyModal = "NotificationModal";
    // References
    private NotificationHistory _notificationHistory;
    // UI References
    [Header("UI")]
    [SerializeField] private VisualTreeAsset _notificationTemplate;
    [SerializeField] private UIColorName _notificationColor;
    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _pushSound;
    [SerializeField] private AudioClip _dismissSound;
    private VisualElement _notificationContainer;
    private Button _buttonModal;
    // State
    private int _showCoroutinesCount = 0; // Variable to delay notifications if multiple are pushed at the same time
    private Dictionary<Guid, SingleNotification> _notificationToElement = new();
    private List<Guid> _notificationList = new();

    protected override void Awake()
    {
      base.Awake();

      _audioSource = GetComponent<AudioSource>();

      _buttonModal = ParentElement.Q<Button>(_elementNameModalButton);
      _buttonModal.AddToClassList(_notificationColor.ToString().ToLowerInvariant());

      _notificationContainer = ParentElement.Q<VisualElement>(_elementNameContainer);
    }

    public void RegisterHistory(NotificationHistory notificationHistory)
    {
      UnregisterHistory();

      _notificationHistory = notificationHistory;

      // GetLastNonDismissed returns the last non-dismissed notifications ordered by time
      // Which means newest notification is at the start of the list
      List<NotificationData> history = _notificationHistory.GetLastNonDismissed(_maxNotifications);

      // We need to push the notifications in reverse order to keep the order
      // So we push the newest notification last
      for (int i = history.Count - 1; i >= 0; i--)
      {
        PushNotification(history[i].Id, false);
      }

      _notificationHistory.OnPushNotification += OnPushNotification;
    }

    public void UnregisterHistory()
    {
      Fade.OnFadeIn -= OnFadeInRegister;
      Fade.OnFadeOut -= OnFadeOutUnregister;

      if (_notificationHistory == null)
      {
        return;
      }

      _notificationHistory.OnPushNotification -= OnPushNotification;
    }
    protected virtual void OnEnable()
    {
      _buttonModal.clicked += ButtonModal;

      Fade.OnFadeIn += OnFadeInRegister;
      Fade.OnFadeOut += OnFadeOutUnregister;
    }
    protected virtual void OnDisable()
    {
      _buttonModal.clicked -= ButtonModal;

      Fade.OnFadeIn -= OnFadeInRegister;
      Fade.OnFadeOut -= OnFadeOutUnregister;
    }

    protected virtual void OnDestroy()
    {
      UnregisterHistory();
    }

    private void OnFadeInRegister()
    {
      if (_notificationHistory != null)
      {
        _notificationHistory.OnPushNotification += OnPushNotification;
      }
    }

    private void OnFadeOutUnregister()
    {
      if (_notificationHistory != null)
      {
        _notificationHistory.OnPushNotification -= OnPushNotification;
      }
    }

    private void ButtonModal()
    {
      UIPrefabLoaderString.Instance.Instantiate(_prefabKeyModal);
    }

    private void OnPushNotification(Guid notificationId)
    {
      PushNotification(notificationId);
    }

    public void PushNotification(Guid notificationId, bool withSound = true)
    {
      if (_notificationToElement.Count + 1 > _maxNotifications)
      {
        SingleNotification notification = PopLastNotification();

        StartCoroutine(RemoveNotification(notification.Parent));
      }

      NotificationData notificationData = _notificationHistory.GetNotification(notificationId);

      VisualElement parent = _notificationTemplate.Instantiate();
      SingleNotification singleNotification = new SingleNotification(parent, notificationData, _notificationColor);

      _notificationContainer.Add(parent);
      _notificationToElement.Add(notificationId, singleNotification);
      _notificationList.Add(notificationId);

      StartCoroutine(ShowNotification(parent, _showCoroutinesCount));
      _showCoroutinesCount++;

      if (withSound)
      {
        PlayPush();
      }

      singleNotification.OnDissmiss += () =>
      {
        SingleNotification notification = PopNotification(notificationId);
        if (notification != null)
        {
          StartCoroutine(RemoveNotification(notification.Parent));
          notificationData.OnClick();
        }
      };

      if (LunaUIManager.Instance != null)
      {
        // Register button to LunaUIManager so it can be managed by mass disable/enable
        LunaUIManager.Instance.RegisterElement(ParentElement, singleNotification.DismissButton);
      }
    }

    private IEnumerator ShowNotification(VisualElement notification, int showCoroutinesCount)
    {
      WaitForSeconds delay = new WaitForSeconds(0.1f);

      yield return delay;

      for (int i = 0; i < showCoroutinesCount; i++)
      {
        yield return delay;
      }

      notification.AddToClassList("active");
      _showCoroutinesCount--;
    }

    private SingleNotification PopLastNotification()
    {
      Guid id = _notificationList[0];

      return PopNotification(id);
    }

    private SingleNotification PopNotification(Guid id, bool withSound = true)
    {
      if (!_notificationToElement.ContainsKey(id))
      {
        return null;
      }

      SingleNotification notification = _notificationToElement[id];
      _notificationToElement.Remove(id);
      _notificationList.Remove(id);

      if (notification.NotificationData != null)
      {
        notification.NotificationData.IsDismissed = true;
      }

      if (withSound)
      {
        PlayDismiss();
      }

      return notification;
    }

    private IEnumerator RemoveNotification(VisualElement notification)
    {
      notification.AddToClassList("removing");

      yield return new WaitForSeconds(0.4f);

      notification.AddToClassList("remove");

      yield return new WaitForSeconds(0.2f);

      notification.parent.Remove(notification);
    }

    public int GetLastIndex()
    {
      return _notificationToElement.Count;
    }

    public void PlayPush()
    {
      if (_audioSource == null || _pushSound == null)
      {
        return;
      }

      _audioSource.PlayOneShot(_pushSound);
    }

    public void PlayDismiss()
    {
      if (_audioSource == null || _dismissSound == null)
      {
        return;
      }

      _audioSource.PlayOneShot(_dismissSound);
    }
  }
}