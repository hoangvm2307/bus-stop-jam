using System;
using System.Collections.Generic;
using CupkekGames.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna
{
    public class ReturnButtonController : MonoBehaviour
    {
        [SerializeField]
        private List<string> _names = new List<string>()
        {
            "ReturnButton"
        };

        // UI
        private UIDocument _uiDocument;
        private List<Button> _buttons = new();

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();

            foreach (string name in GetButtonNames())
            {
                _buttons.AddRange(_uiDocument.rootVisualElement.Query<Button>(name).ToList());
            }
        }

        private void OnEnable()
        {
            foreach (Button button in _buttons)
            {
                if (button == null)
                {
                    continue;
                }

                button.clicked += OnClicked;
            }
        }

        private void OnDisable()
        {
            foreach (Button button in _buttons)
            {
                if (button == null)
                {
                    continue;
                }

                button.clicked -= OnClicked;
            }
        }

        private void OnClicked()
        {
            InputEscapeManager.Pop();
        }

        protected virtual List<string> GetButtonNames()
        {
            return _names;
        }
    }
}