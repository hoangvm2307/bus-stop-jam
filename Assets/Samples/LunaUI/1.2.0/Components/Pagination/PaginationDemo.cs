using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna.Demo.Components
{
    public class PaginationDemo : UIViewComponent
    {
        // Settings
        [Header("Pagination Settings")]
        [SerializeField] private int _itemPerPage = 3;
        [SerializeField] private int _maxButtonAmount = 5;
        [SerializeField] private UIColorName _baseButtonColor = UIColorName.BASE;
        [SerializeField] private UIColorName _activeButtonColor = UIColorName.PRIMARY;
        [Header("Pagination Data")]
        [SerializeField] private int _dataAmount = 200;
        // Pagination
        private PaginationController<string> _pagination;

        // Source List
        private List<string> _sourceList = new();
        // UI Controller for page elements
        private List<LabelWithBinding> _uiController = new();

        // Container
        private VisualElement _container;

        protected override void Awake()
        {
            base.Awake();

            // Mock up data
            for (int i = 0; i < _dataAmount; i++)
            {
                _sourceList.Add("Element-" + i);
            }

            // Create page element controllers  
            // In this example, our controller class is LabelWithBinding.  
            // Using controllers allows us to reuse VisualElements instead of destroying  
            // and recreating them whenever the page changes.  
            // This approach improves performance and reduces unnecessary UI rebuilds.  
            _container = ParentElement.Q<VisualElement>("PaginationElementsContainer");
            for (int i = 0; i < _itemPerPage; i++)
            {
                Label label = new Label();
                _container.Add(label);
                LabelWithBinding uiController = new LabelWithBinding(label);
                _uiController.Add(uiController);
            }

            // Create pagination
            VisualElement paginationElement = ParentElement.Q<VisualElement>("Pagination");
            _pagination = new(
                _sourceList,
                _itemPerPage,
                paginationElement,
                _baseButtonColor,
                _activeButtonColor,
                _maxButtonAmount
            );
        }

        private void OnEnable()
        {
            _pagination.OnPageChange += OnPageChange;
        }

        private void OnDisable()
        {
            _pagination.OnPageChange -= OnPageChange;
        }

        protected virtual void Start()
        {
            // Render first page
            _pagination.GoToPage(0);
        }

        private void OnPageChange(int page)
        {
            List<string> currentPage = _pagination.GetCurrentPageElements(); // get data for current page

            // int start = _pagination.GetStartIndex(page); // get start index of current page if needed

            // Why binding?
            // Instead of deleting and recreating elements at runtime, 
            // we are reusing existing elements and updating their content dynamically.
            // This approach improves performance by reducing garbage collection overhead 
            // and ensures smoother UI updates.

            for (int i = 0; i < _pagination.ItemsPerPage; i++)
            {
                LabelWithBinding controller = _uiController[i];
                controller.Unbind();

                if (i >= currentPage.Count)
                {
                    continue;
                }

                // Alternative way to get data
                // int index = start + i;
                // string data = _sourceList[index];

                // Easier way to get data
                string data = currentPage[i];

                controller.Bind(data);
            }
        }
    }
}