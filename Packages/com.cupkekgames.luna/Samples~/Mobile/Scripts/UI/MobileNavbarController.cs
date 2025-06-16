using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using CupkekGames.InventorySystem;
using CupkekGames.Systems;
using CupkekGames.Core;

namespace CupkekGames.Luna.Mobile
{
    public class MobileNavbarController : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;
        [SerializeField] private List<UIViewComponent> _views;
        private List<VisualElement> _buttonParents;
        private List<Button> _buttons;
        [SerializeField] private int _currentIndex = 0;

        private void Start()
        {
            // Setup Views
            for (int i = 0; i < _views.Count; i++)
            {
                if (i == _currentIndex)
                {
                    _views[i].Fade.FadeIn();
                }
                else
                {
                    _views[i].Fade.FadeOut();
                }
            }

            VisualElement navbar = _uiDocument.rootVisualElement.Q<VisualElement>("Navbar");

            // Setup Buttons
            _buttonParents = new();
            _buttons = new();

            int y = 0;
            foreach (VisualElement buttonParent in navbar.Children())
            {
                Button button = buttonParent.Q<Button>();
                _buttonParents.Add(buttonParent);
                _buttons.Add(button);

                int index = y;
                button.clicked += () => OnNavbarButtonClicked(index);
                
                y++;
            }

            // Initial State
            ClearAllActiveClass();
            OnNavbarButtonClicked(_currentIndex);
        }

        private void OnNavbarButtonClicked(int index)
        {
            _views[_currentIndex].Fade.FadeOut();
            _buttonParents[_currentIndex].RemoveFromClassList("active");
            _currentIndex = index;
            _buttonParents[_currentIndex].AddToClassList("active");
            _views[_currentIndex].Fade.FadeIn();
        }

        private void ClearAllActiveClass()
        {
            foreach (var buttonParent in _buttonParents)
            {
                buttonParent.RemoveFromClassList("active");
            }
        }
    }
}