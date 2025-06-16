using System.Collections.Generic;
using CupkekGames.Core;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.ObjectModel;


#if UNITY_INPUT
using UnityEngine.InputSystem;
#endif

namespace CupkekGames.Luna
{
  [RequireComponent(typeof(UIInteractableAudioHandler))]
  public class LunaUIManager : Singleton<LunaUIManager>
  {
    #region Settings

#if UNITY_INPUT
    [SerializeField] private PlayerInput _playerInput;
    public PlayerInput PlayerInput => _playerInput;
    [SerializeField] private List<string> _escapeActionNames = new List<string>() { "UI/Cancel" };
    public ReadOnlyCollection<string> EscapeActionNames => _escapeActionNames.AsReadOnly();
#else
    [SerializeField] private InputIconControlScheme _startingControlScheme;
#endif

    [SerializeField] private InputIconDatabaseSO _iconDatabase;
    public InputIconDatabaseSO IconDatabase => _iconDatabase;

    [SerializeField]
    List<string> _blacklist = new()
    {
        "unity-content-container"
    };

    #endregion
    #region References

    private UIInteractableAudioHandler _audioHandler;
    public UIInteractableAudioHandler AudioHandler => _audioHandler;

    #endregion
    #region Properties

    // UI Element Manager
    private Dictionary<VisualElement, HashSet<VisualElement>> _pages = new Dictionary<VisualElement, HashSet<VisualElement>>();
    private HashSet<VisualElement> _blockers = new HashSet<VisualElement>();
    private HashSet<VisualElement> _blockedElements = null;
    // Special Actions
#if UNITY_INPUT
    private List<InputAction> _escapeActions = new();
#endif

    #endregion

    #region Initialization
    protected override void Awake()
    {
      base.Awake();

      _audioHandler = GetComponent<UIInteractableAudioHandler>();


#if UNITY_INPUT
      if (_playerInput != null)
      {
        foreach (string name in _escapeActionNames)
        {
          _escapeActions.Add(_playerInput.actions[name]);
        }
      }
#endif
    }

    private void OnEnable()
    {
#if UNITY_INPUT
      if (!InputDeviceManager.OnEnable(_playerInput))
      {
        Debug.LogWarning("LunaUIManager: PlayerInput is null.");
      }

      foreach (InputAction action in _escapeActions)
      {
        action.performed += OnEscape;
      }
#else
InputDeviceManager.UpdateControlScheme(_startingControlScheme, true);
#endif
    }

#if UNITY_INPUT
    private void OnDisable()
    {
      InputDeviceManager.OnDisable();

      foreach (InputAction action in _escapeActions)
      {
        action.performed -= OnEscape;
      }
    }
#endif
    #endregion

    #region Input Actions

#if UNITY_INPUT
    private void OnEscape(InputAction.CallbackContext context)
    {
      InputEscapeManager.InputEscapeEvent?.Invoke();
    }

    public InputAction GetInputAction(string key)
    {
      if (_playerInput == null)
      {
        return null;
      }

      return _playerInput.actions[key];
    }

    // public void DebugControls(InputAction action)
    // {
    //     foreach (var binding in action.bindings)
    //     {
    //         string displayString = binding.ToDisplayString(out string deviceLayoutName, out string controlPath);
    //         Debug.Log(deviceLayoutName + " - " + displayString + " - " + controlPath);
    //     }
    // }

    // public static void DumpInputPaths()
    // {
    //     // <Layout>{Usage}#(DisplayName)Name
    //     var inputPaths = new JObject();
    //     foreach (var layout in InputSystem.ListLayouts())
    //     {
    //         inputPaths[layout] = new JArray();

    //         InputControlLayout controlLayout = InputSystem.LoadLayout(layout);

    //         try
    //         {
    //             var device = InputSystem.AddDevice(layout);

    //             // Iterate through all controls on the device
    //             foreach (var control in device.allControls)
    //             {
    //                 JArray arr = (JArray)inputPaths[device.layout];
    //                 arr.Add(control.path);
    //                 arr.Add(control.displayName);
    //             }
    //         }
    //         catch (System.Exception e)
    //         {
    //             Debug.LogError(e);
    //         }
    //     }

    //     File.WriteAllText("inputPaths.json", inputPaths.ToString(), Encoding.UTF8);
    // }
#endif

    #endregion
    #region UI Element Manager

    public void RegisterPage(VisualElement parent)
    {
      List<VisualElement> list = parent.Query<VisualElement>().ToList();

      HashSet<VisualElement> pageElements = GetPage(parent);
      foreach (VisualElement item in list)
      {
        if (item.focusable && !_blacklist.Contains(item.name))
        {
          pageElements.Add(item);
          if (item.enabledSelf)
          {
            _audioHandler.AddManipulator(item);
          }
        }
      }

      _pages[parent] = pageElements;
    }

    public void UnregisterPage(VisualElement parent)
    {
      if (parent != null && _pages.ContainsKey(parent))
      {
        foreach (VisualElement item in _pages[parent])
        {
          _audioHandler.RemoveManipulator(item);
        }

        _pages.Remove(parent);
      }
    }

    public HashSet<VisualElement> GetPage(VisualElement parent)
    {
      if (_pages.TryGetValue(parent, out var value))
      {
        return value;
      }

      return new HashSet<VisualElement>();
    }

    public void RegisterElement(VisualElement parent, VisualElement item)
    {
      HashSet<VisualElement> list = GetPage(parent) ?? new HashSet<VisualElement>();

      list.Add(item);

      _pages[parent] = list;

      if (item.enabledSelf)
      {
        _audioHandler.AddManipulator(item);
      }
    }

    public void UnregisterElement(VisualElement parent, VisualElement item)
    {
      HashSet<VisualElement> pageElements = GetPage(parent);

      if (pageElements != null)
      {
        pageElements.Remove(item);
        _audioHandler.RemoveManipulator(item);

        if (pageElements.Count != 0)
        {
          _pages[parent] = pageElements;
        }
        else
        {
          _pages.Remove(parent);
        }
      }
    }

    public void SetEnabledElements(bool enabled, HashSet<VisualElement> elements)
    {
      foreach (VisualElement item in elements)
      {
        if (item.enabledSelf != enabled)
        {
          item.SetEnabled(enabled);
        }
      }
    }

    public HashSet<VisualElement> SetEnabledAllPages(bool enabled, params VisualElement[] exceptPages)
    {
      return SetEnabledAllPages(enabled, new HashSet<VisualElement>(exceptPages));
    }

    public HashSet<VisualElement> SetEnabledAllPages(bool enabled, HashSet<VisualElement> exceptPages)
    {
      HashSet<VisualElement> changedElements = new();

      HashSet<VisualElement> exceptElements = new HashSet<VisualElement>();

      foreach (var page in exceptPages)
      {
        if (_pages.ContainsKey(page))
        {
          exceptElements.UnionWith(_pages[page]);
        }
      }

      foreach (VisualElement page in _pages.Keys)
      {
        if (exceptPages != null)
        {
          if (exceptPages.Contains(page))
          {
            continue;
          }
        }

        changedElements.UnionWith(SetEnabledPage(page, enabled, exceptElements));
      }

      return changedElements;
    }

    public HashSet<VisualElement> SetEnabledPage(VisualElement page, bool enabled, HashSet<VisualElement> exceptElements)
    {
      HashSet<VisualElement> changedElements = new();

      if (!_pages.ContainsKey(page))
      {
        return changedElements;
      }

      foreach (VisualElement item in _pages[page])
      {
        if (exceptElements.Contains(item))
        {
          continue;
        }

        if (item.enabledSelf != enabled)
        {
          item.SetEnabled(enabled);
          changedElements.Add(item);
        }
      }

      return changedElements;
    }

    public void SetBlocker(VisualElement blocker)
    {
      if (_blockedElements != null)
      {
        RemoveBlocker();
      }

      _blockedElements = SetEnabledAllPages(false);

      blocker.SetEnabled(true);
      blocker.RegisterCallback<ClickEvent>(OnBlocker);

      _blockers.Add(blocker);
    }

    public void SetBlocker(string elementName)
    {
      VisualElement blockerElement = null;

      foreach (HashSet<VisualElement> value in _pages.Values)
      {
        foreach (VisualElement item in value)
        {
          if (item.name == elementName)
          {
            blockerElement = item;
            break;
          }
        }
      }

      if (blockerElement != null)
      {
        SetBlocker(blockerElement);
      }
    }

    public void RemoveBlocker()
    {
      if (_blockedElements != null)
      {
        foreach (VisualElement item in _blockedElements)
        {
          item.SetEnabled(true);
        }
      }

      foreach (VisualElement blockerButton in _blockers)
      {
        blockerButton.UnregisterCallback<ClickEvent>(OnBlocker);
      }

      _blockers.Clear();
    }

    private void OnBlocker(ClickEvent evt)
    {
      RemoveBlocker();
    }

    #endregion
  }
}
