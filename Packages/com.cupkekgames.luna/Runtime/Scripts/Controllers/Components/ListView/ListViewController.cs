using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

namespace CupkekGames.Luna
{
  [RequireComponent(typeof(UIDocument))]
  public abstract class ListViewController : MonoBehaviour
  {
    [SerializeField] private VisualTreeAsset _entry;
    [SerializeField] private string _listName;
    private UIDocument _uiDocument;
    public UIDocument UIDocument => _uiDocument;
    private ListView _listView;
    private ListView ListView => _listView;

    protected virtual void Awake()
    {
      _uiDocument = GetComponent<UIDocument>();

      _listView = _uiDocument.rootVisualElement.Q<ListView>(_listName);

      FillListView();
    }

    protected virtual void OnEnable()
    {
      _listView.selectionChanged += OnSelectionChanged;
    }

    protected virtual void OnDisable()
    {
      _listView.selectionChanged -= OnSelectionChanged;
    }

    protected virtual void FillListView()
    {
      _listView.makeItem = () =>
      {
        var newEntry = _entry.Instantiate();

        return newEntry;
      };

      _listView.bindItem = (item, index) =>
      {
        BindItem(item, index);
      };

      _listView.unbindItem = (item, index) =>
      {
        UnbindItem(item, index);
      };

      _listView.itemsSource = GetSourceList();
    }
    public abstract IList GetSourceList();
    public abstract void BindItem(VisualElement item, int index);
    public abstract void UnbindItem(VisualElement item, int index);
    public abstract void OnSelectionChanged(IEnumerable<object> selectedItems);
  }
}
