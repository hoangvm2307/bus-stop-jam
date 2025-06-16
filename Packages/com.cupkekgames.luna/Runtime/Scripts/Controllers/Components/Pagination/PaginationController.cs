using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna
{
    public class PaginationController<T> : Pagination<T>
    {
        private VisualElement _parent;
        private VisualElement _buttonContainer;
        private Label _pageLabel;
        private Button _buttonNext;
        private Button _buttonPrev;
        private Dictionary<Button, Action> _buttons = new();
        private UIColorName _uiColorNormal;
        private UIColorName _uiColorActive;
        private int _maxButtonAmount;

        public PaginationController(List<T> data, int itemsPerPage) : base(data, itemsPerPage)
        {

        }

        public PaginationController(List<T> data, int itemsPerPage, VisualElement parent, UIColorName normal, UIColorName active,
        int maxButtonAmount) : base(data, itemsPerPage)
        {
            SetUI(parent, normal, active, maxButtonAmount);
        }

        public void SetUI(VisualElement parent, UIColorName normal, UIColorName active, int maxButtonAmount)
        {
            _parent = parent;
            _uiColorNormal = normal;
            _uiColorActive = active;
            _maxButtonAmount = maxButtonAmount;

            if (_parent != null)
            {
                _buttonContainer = _parent.Q<VisualElement>("ButtonContainer");
                _pageLabel = _parent.Q<Label>("PageLabel");

                if (_buttonPrev != null)
                {
                    _buttonPrev.clicked -= PreviousPageVoid;
                }

                if (_buttonNext != null)
                {
                    _buttonNext.clicked -= NextPageVoid;
                }

                _buttonPrev = _parent.Q<Button>("Previous");
                _buttonNext = _parent.Q<Button>("Next");

                if (_buttonPrev != null)
                {
                    _buttonPrev.AddToClassList(_uiColorNormal.ToString().ToLowerInvariant());
                    _buttonPrev.clicked += PreviousPageVoid;
                }
                if (_buttonNext != null)
                {
                    _buttonNext.AddToClassList(_uiColorNormal.ToString().ToLowerInvariant());
                    _buttonNext.clicked += NextPageVoid;
                }

                UpdateUI();

                OnPageChange -= OnPageChangeUI;
                OnPageChange += OnPageChangeUI;
            }
        }

        private void OnPageChangeUI(int currentPage)
        {
            UpdatePaginationUI();
        }

        public void UpdateUI()
        {
            CreateButtons();
            UpdatePaginationUI();
        }


        private void PreviousPageVoid()
        {
            PreviousPage();
        }
        private void NextPageVoid()
        {
            NextPage();
        }

        private void CreateButtons()
        {
            if (_buttonContainer == null)
            {
                return;
            }

            _buttonContainer.Clear();

            int totalPages = TotalPages;
            int amount = Mathf.Min(totalPages, _maxButtonAmount);

            for (int i = 0; i < amount; i++)
            {
                Button button = new Button();
                button.AddToClassList("btn");
                button.AddToClassList(_uiColorNormal.ToString().ToLowerInvariant());

                button.text = (i + 1).ToString();

                int cache = i;
                Action action = () => GoToPage(cache);
                button.clicked += action;
                _buttons.Add(button, action);

                _buttonContainer.Add(button);
            }
        }

        private void UpdatePaginationUI()
        {
            if (_pageLabel != null)
            {
                _pageLabel.text = CurrentPage.ToString();
            }

            int totalPages = TotalPages;
            int currentPage = CurrentPage;

            if (_buttonPrev != null)
            {
                if (currentPage == 0)
                {
                    _buttonPrev.SetEnabled(false);
                }
                else
                {
                    _buttonPrev.SetEnabled(true);
                }
            }

            if (_buttonNext != null)
            {
                if (currentPage == totalPages - 1)
                {
                    _buttonNext.SetEnabled(false);
                }
                else
                {
                    _buttonNext.SetEnabled(true);
                }
            }

            if (_buttonContainer == null)
            {
                return;
            }

            // Calculate start and end of the button window
            int halfWindow = _maxButtonAmount / 2;
            int startPage = Mathf.Max(0, currentPage - halfWindow);
            int endPage = Mathf.Min(totalPages, startPage + _maxButtonAmount);

            // If near the end, shift the window to the left to show the last few pages
            if (endPage - startPage < _maxButtonAmount && startPage > 0)
            {
                startPage = Mathf.Max(0, endPage - _maxButtonAmount);
            }

            bool showFirst = startPage > 0;
            bool showLast = endPage < totalPages;

            // Update button appearance based on the current page
            int buttonIndex = 0;
            int totalButtons = _buttonContainer.childCount;
            foreach (VisualElement ve in _buttonContainer.Children())
            {
                int pageIndex = startPage + buttonIndex;

                if (pageIndex >= endPage)
                {
                    ve.style.display = DisplayStyle.None;
                }
                else
                {
                    ve.style.display = DisplayStyle.Flex;

                    if (pageIndex == currentPage)
                    {
                        ve.AddToClassList(_uiColorActive.ToString().ToLowerInvariant());
                    }
                    else
                    {
                        ve.RemoveFromClassList(_uiColorActive.ToString().ToLowerInvariant());
                    }
                }


                Button button = (Button)ve;
                button.clicked -= _buttons[button];

                button.SetEnabled(true);
                bool isEllipsisButton = false;

                if (showFirst && buttonIndex <= 1)
                {
                    pageIndex = 0;
                    if (buttonIndex == 1)
                    {
                        button.text = "...";
                        button.SetEnabled(false);
                        isEllipsisButton = true;
                    }
                }
                else if (showLast && buttonIndex >= totalButtons - 2)
                {
                    pageIndex = totalPages - 1;
                    if (buttonIndex == totalButtons - 2)
                    {
                        button.text = "...";
                        button.SetEnabled(false);
                        isEllipsisButton = true;
                    }
                }

                if (!isEllipsisButton)
                {
                    button.text = (pageIndex + 1).ToString();
                }

                Action action = () => GoToPage(pageIndex);
                button.clicked += action;
                _buttons[button] = action;

                buttonIndex++;
            }
        }

        public void Show()
        {
            if (_parent != null)
            {
                _parent.style.display = DisplayStyle.Flex;
            }
        }

        public void Hide()
        {
            if (_parent != null)
            {
                _parent.style.display = DisplayStyle.None;
            }
        }
    }
}
