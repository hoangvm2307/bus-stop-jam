using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using CupkekGames.Luna;
using System.Linq;
using System.Collections;


#if UNITY_INPUT
using UnityEngine.InputSystem;
#endif

namespace CupkekGames.Systems.UI
{
  [RequireComponent(typeof(UIDocument))]
  [RequireComponent(typeof(GameSaveView))]
  public abstract class GameSaveViewList<TSaveData, TSaveMetadata> : MonoBehaviour where TSaveData : IGameSaveData where TSaveMetadata : GameSaveMetadata
  {
    // References
    protected GameSaveManager<TSaveData, TSaveMetadata> _gameSaveManager;
    public GameSaveManager<TSaveData, TSaveMetadata> GameSaveManager => _gameSaveManager;
    protected GameSaveView _gameSaveView;
    public GameSaveView GameSaveView => _gameSaveView;
    // UI
    protected VisualElement _root;
    [SerializeField] protected VisualTreeAsset _listEntryTemplate;
    protected ListView _listView;
    List<GameSaveMetadataWithSlot<TSaveMetadata>> _metadataCache;
    List<GameSaveMetadataWithSlot<TSaveMetadata>> _metadataFiltered;
    protected bool _isInGame;
    protected Button _newSaveButton;
    protected Toggle _showAutoToggle;
    protected Toggle _showManualToggle;
    protected InputPrompt _loadSaveButton;
    protected InputPrompt _overwriteSaveButton;
    protected InputPrompt _deleteSaveButton;
    protected ChoicePopupController _choicePopupController;
    protected string _selectedChoiceAction = "";

#if UNITY_INPUT
    protected InputAction _loadSaveAction;
    protected InputAction _overwriteSaveAction;
    protected InputAction _deleteSaveAction;
#endif

    protected virtual void Awake()
    {
      _gameSaveManager = GetSaveManager();

      _root = GetComponent<UIDocument>().rootVisualElement;

      _gameSaveView = GetComponent<GameSaveView>();

      _newSaveButton = _root.Q<Button>("NewSave");
      _showAutoToggle = _root.Q<Toggle>("ShowAuto");
      _showManualToggle = _root.Q<Toggle>("ShowManual");

      _loadSaveButton = _root.Q<InputPrompt>("Load");
      _overwriteSaveButton = _root.Q<InputPrompt>("Overwrite");
      _deleteSaveButton = _root.Q<InputPrompt>("Delete");

      _listView = _root.Q<ListView>("LoadList");

      _choicePopupController = GetComponent<ChoicePopupController>();
    }

    
    private void InitializeListView()
    {
      _listView.makeItem = () =>
      {
        VisualElement item = _listEntryTemplate.Instantiate();

        GameSaveViewEntry<TSaveMetadata> entry = new GameSaveViewEntry<TSaveMetadata>();
        entry.MakeItem(gameObject, item, GetTooltipController());

        item.userData = entry;

        return item;
      };

      _listView.bindItem = (item, index) =>
      {
        GameSaveViewEntry<TSaveMetadata> entry = (GameSaveViewEntry<TSaveMetadata>)item.userData;

        GameSaveMetadataWithSlot<TSaveMetadata> metadata = _metadataFiltered[index];

        entry.BindItem(
          index, 
          metadata.SaveSlot, 
          metadata.Metadata, 
          _isInGame, 
          SlotOne(index, metadata), 
          SlotTwo(index, metadata)
        );
      };

      _listView.unbindItem = (item, index) =>
      {
        GameSaveViewEntry<TSaveMetadata> entry = (GameSaveViewEntry<TSaveMetadata>)item.userData;
        entry.UnbindItem();
      };

      _listView.selectionChanged += OnSelectionChanged;
    }

    protected virtual void RegisterCallbacks()
    {
      _listView.RegisterCallback<FocusInEvent>(e =>
      {
        _listView.Focus();
        e.StopPropagation();
        _listView.focusController.IgnoreEvent(e);
      });

      _listView.RegisterCallback<NavigationMoveEvent>(e =>
      {
        switch (e.direction)
        {
          case NavigationMoveEvent.Direction.Up:
            if (_listView.selectedIndex == 0)
            {
              if (_newSaveButton.enabledSelf)
              {
                _newSaveButton.Focus();
              }
              else
              {
                _showAutoToggle.Focus();
              }
            }
            else
            {
              _listView.selectedIndex--;
            }
            break;
          case NavigationMoveEvent.Direction.Down:
            if (_listView.selectedIndex == _listView.itemsSource.Count - 1)
            {
              if (_newSaveButton.enabledSelf)
              {
                _newSaveButton.Focus();
              }
              else
              {
                _showAutoToggle.Focus();
              }
            }
            else
            {
              _listView.selectedIndex++;
            }
            break;
          case NavigationMoveEvent.Direction.Left:
            _showAutoToggle.Focus();
            break;
          case NavigationMoveEvent.Direction.Right:
            _showAutoToggle.Focus();
            break;
        }

        e.StopPropagation(); // Prevent further processing of the event
        _listView.focusController.IgnoreEvent(e); // Prevent default navigation
      });

      _newSaveButton.RegisterCallback<NavigationMoveEvent>(e =>
      {
        switch (e.direction)
        {
          case NavigationMoveEvent.Direction.Up:
            _listView.Focus();
            break;
          case NavigationMoveEvent.Direction.Down:
            _listView.Focus();
            break;
          case NavigationMoveEvent.Direction.Left:
            _showAutoToggle.Focus();
            break;
          case NavigationMoveEvent.Direction.Right:
            _showAutoToggle.Focus();
            break;
        }

        e.StopPropagation(); // Prevent further processing of the event
        _listView.focusController.IgnoreEvent(e); // Prevent default navigation
      });
    }

    public void ResetListSelection()
    {
      if (_metadataFiltered.Count > 0)
      {
        _listView.selectedIndex = 0;
        _listView.ScrollToItem(_listView.selectedIndex);
        _listView.Focus();
      } else {
        _listView.selectedIndex = -1;
      }
    }

    protected virtual void Start()
    {
      UpdateMetadataCache();

      InitializeListView();
      RegisterCallbacks();
      _listView.itemsSource = _metadataFiltered;
      ResetListSelection();

      _listView.Focus();

      _isInGame = IsInGame();

      if (_isInGame)
      {
        _newSaveButton.clicked += OnNewSaveButtonClicked;
        _overwriteSaveButton.clicked += OnOverwriteButtonClicked;
      }
      else
      {
        _newSaveButton.SetEnabled(false);
        _overwriteSaveButton.style.display = DisplayStyle.None;
      }

      _showAutoToggle.RegisterValueChangedCallback(OnShowAutoToggleChanged);
      _showManualToggle.RegisterValueChangedCallback(OnShowManualToggleChanged);
      _choicePopupController.OnButtonClick += OnChoiceButtonClick;

      _loadSaveButton.clicked += OnLoadButtonClicked;
      _deleteSaveButton.clicked += OnDeleteButtonClicked;

      #if UNITY_INPUT
        _overwriteSaveAction = _overwriteSaveButton.Action;
        _loadSaveAction = _loadSaveButton.Action;
        _deleteSaveAction = _deleteSaveButton.Action;

        _loadSaveAction.performed += OnLoadInputPerformed;
        _deleteSaveAction.performed += OnDeleteInputPerformed;
        
        if (_isInGame)
        {
          _overwriteSaveAction.performed += OnOverwriteInputPerformed;
        }
      #endif
    }
    protected virtual void OnDestroy()
    {
      _newSaveButton.clicked -= OnNewSaveButtonClicked;

      _showAutoToggle.UnregisterValueChangedCallback(OnShowAutoToggleChanged);
      _showManualToggle.UnregisterValueChangedCallback(OnShowManualToggleChanged);
      _choicePopupController.OnButtonClick -= OnChoiceButtonClick;

      _loadSaveButton.clicked -= OnLoadButtonClicked;
      _overwriteSaveButton.clicked -= OnOverwriteButtonClicked;
      _deleteSaveButton.clicked -= OnDeleteButtonClicked;

      #if UNITY_INPUT
        _loadSaveAction.performed -= OnLoadInputPerformed;
        _overwriteSaveAction.performed -= OnOverwriteInputPerformed;
        _deleteSaveAction.performed -= OnDeleteInputPerformed;
      #endif
    }

    private void OnShowAutoToggleChanged(ChangeEvent<bool> evt)
    {
      UpdateMetadataFiltered();
      UpdateListView();
    }

    private void OnShowManualToggleChanged(ChangeEvent<bool> evt)
    {
      UpdateMetadataFiltered();
      UpdateListView();
    }

    private void UpdateMetadataCache()
    {
      _metadataCache = _gameSaveManager.GetAllMetadata(true);
      UpdateMetadataFiltered();
    }

    private void UpdateMetadataFiltered()
    {
      _metadataFiltered = _metadataCache.Where(m => 
        (m.Metadata.IsAutosave && _showAutoToggle.value) || 
        (!m.Metadata.IsAutosave && _showManualToggle.value)
      ).ToList();
    }

    protected virtual void OnSelectionChanged(IEnumerable<object> selection)
    {
      OnSelectionChanged();
    }

    protected virtual void OnSelectionChanged()
    {
      GameSaveMetadataWithSlot<TSaveMetadata>? entry = GetSelectedMetadata();
      string saveSlotText = entry.HasValue ? entry.Value.SaveSlot.ToString() : "";
      _loadSaveButton.LabelText = "Load Save " + saveSlotText;
      _overwriteSaveButton.LabelText = "Overwrite Save " + saveSlotText;
      _deleteSaveButton.LabelText = "Delete Save" + saveSlotText;
    }

    private void UpdateListView()
    {
      _listView.itemsSource = _metadataFiltered;
      _listView.RefreshItems();
      if (_listView.selectedIndex >= _metadataFiltered.Count)
      {
        _listView.selectedIndex = _metadataFiltered.Count - 1;
      }

      if (_listView.selectedIndex > -1 && _listView.selectedIndex < _metadataFiltered.Count)
      {
        OnSelectionChanged();
      }
    }

    private void OnNewSaveButtonClicked()
    {
      int availableSlot = _gameSaveManager.GetFirstAvailableSlot();
      _gameSaveManager.SaveToFile(availableSlot, _gameSaveManager.CurrentSave.Data);

      UpdateMetadataCache();
      UpdateListView();
    }

    private void OnOverwriteButtonClicked()
    {
      GameSaveMetadataWithSlot<TSaveMetadata>? entry = GetSelectedMetadata();
      if (!entry.HasValue)
      {
        return;
      }
      string saveSlotText = entry.Value.SaveSlot.ToString();
      _selectedChoiceAction = "overwrite";
      _choicePopupController.TextHeader = "Overwrite Save " + saveSlotText;
      _choicePopupController.TextBody = "Are you sure you want to overwrite this save?";
      _choicePopupController.Fade.FadeIn();
    }

    private void OnDeleteButtonClicked()
    {
      GameSaveMetadataWithSlot<TSaveMetadata>? entry = GetSelectedMetadata();
      if (!entry.HasValue)
      {
        return;
      }
      string saveSlotText = entry.Value.SaveSlot.ToString();
      _selectedChoiceAction = "delete";
      _choicePopupController.TextHeader = "Delete Save " + saveSlotText;
      _choicePopupController.TextBody = "Are you sure you want to delete this save?";
      _choicePopupController.Fade.FadeIn();
    }

    public GameSaveMetadataWithSlot<TSaveMetadata>? GetSelectedMetadata()
    {
      if (_listView.selectedIndex < 0 || _listView.selectedIndex >= _metadataFiltered.Count)
      {
        return null;
      }

      return _metadataFiltered[_listView.selectedIndex];
    }

    protected abstract void OnLoadButtonClicked();
    protected abstract GameSaveManager<TSaveData, TSaveMetadata> GetSaveManager();
    protected abstract bool IsInGame();
    protected abstract VisualElement SlotOne(int index, GameSaveMetadataWithSlot<TSaveMetadata> metadata);
    protected abstract VisualElement SlotTwo(int index, GameSaveMetadataWithSlot<TSaveMetadata> metadata);
    protected abstract TooltipController GetTooltipController();

    private void OnChoiceButtonClick(int i)
    {
      if (i == 0)
      {
        GameSaveMetadataWithSlot<TSaveMetadata> metadata = _metadataFiltered[_listView.selectedIndex];
        if (_selectedChoiceAction == "overwrite")
        {
          _gameSaveManager.SaveToFile(metadata.SaveSlot, _gameSaveManager.CurrentSave.Data);
          ResetListSelection();
        }
        else if (_selectedChoiceAction == "delete")
        {
          _gameSaveManager.DeleteFile(metadata.SaveSlot);
        }
        UpdateMetadataCache();
        UpdateListView();
      }
    }

#if UNITY_INPUT
    private void OnLoadInputPerformed(InputAction.CallbackContext context)
    {
      OnLoadButtonClicked();
    }
    
    private void OnOverwriteInputPerformed(InputAction.CallbackContext context)
    {
      OnOverwriteButtonClicked();
    }

    private void OnDeleteInputPerformed(InputAction.CallbackContext context)
    {
      OnDeleteButtonClicked();
    }
#endif
  }
}
