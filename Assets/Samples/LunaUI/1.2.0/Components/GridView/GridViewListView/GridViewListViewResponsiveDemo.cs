using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna.Demo.Components
{
    public class GridViewListViewResponsiveDemo : UIViewComponent
    {
        // Settings
        [Header("GridView Settings")]
        [SerializeField] private int _itemWidth = 100;
        [SerializeField] private VisualTreeAsset _slotTemplate;
        [SerializeField] private bool _hideEmpty = true;
        [Header("GridView Data")]
        [SerializeField] private int _dataAmount = 200;
        // ListView
        private GridViewListViewInt _gridView;

        // Source List
        private List<int> _sourceList = new();

        protected override void Awake()
        {
            base.Awake();

            // Mock up data
            for (int i = 0; i < _dataAmount; i++)
            {
                _sourceList.Add(UnityEngine.Random.Range(0, 1000));
            }
 
            VisualElement gridViewParent = ParentElement.Q<VisualElement>("GridView");
            ListView gridViewSlotContainer = ParentElement.Q<ListView>("GridViewList");

            _gridView = new GridViewListViewInt(
                _sourceList,
                _slotTemplate,
                gameObject,
                gridViewParent,
                gridViewSlotContainer,
                null
            );

            _gridView.SetHideEmpty(_hideEmpty);
            _gridView.SetDynamicItemPerLine(_itemWidth);
        }

        private void OnEnable()
        {
            _gridView.RegisterDynamicItemPerLineUpdate();
        }

        private void OnDisable()
        {
            _gridView.UnregisterDynamicItemPerLineUpdate();
        }
    }
}