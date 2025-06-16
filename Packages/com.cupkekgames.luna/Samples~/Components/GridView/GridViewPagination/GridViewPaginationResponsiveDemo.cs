using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna.Demo.Components
{
    public class GridViewPaginationResponsiveDemo : UIViewComponent
    {
        // Settings
        [Header("GridView Settings")]
        [SerializeField] private int _lineCount = 3;
        [SerializeField] private int _itemWidth = 100;
        [SerializeField] private int _maxButtonAmount = 5;
        [SerializeField] private UIColorName _baseButtonColor = UIColorName.BASE;
        [SerializeField] private UIColorName _activeButtonColor = UIColorName.PRIMARY;
        [SerializeField] private VisualTreeAsset _slotTemplate;
        [SerializeField] private bool _hideEmpty = true;
        [Header("GridView Data")]
        [SerializeField] private int _dataAmount = 200;
        // Pagination
        private GridViewPaginationInt _gridView;

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
            VisualElement gridViewSlotContainer = ParentElement.Q<VisualElement>("GridViewSlotContainer");

            _gridView = new GridViewPaginationInt(
                _sourceList,
                _slotTemplate,
                gameObject,
                gridViewParent,
                gridViewSlotContainer,
                null
            );

            VisualElement paginationElement = ParentElement.Q<VisualElement>("Pagination");
            
            _gridView.SetHideEmpty(_hideEmpty);
            _gridView.SetPaginationUI(paginationElement, _baseButtonColor, _activeButtonColor, _maxButtonAmount);

            _gridView.SetDynamicItemPerLine(_lineCount, _itemWidth);
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