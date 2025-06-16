using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using CupkekGames.Core;
using System.Collections;

namespace CupkekGames.Luna.Demo.Components
{
    public class ScrollViewDemo : MonoBehaviour
    {
        [SerializeField] private bool _squareEntryStyle = false;
        [SerializeField] private int _squareRowEntryAmount = 2;
        [SerializeField] private List<string> _source;
        [SerializeField] private string _scrollViewName;
        private UIDocument _uiDocument;
        public UIDocument UIDocument => _uiDocument;
        private ScrollView _scrollView;
        private ScrollView ScrollView => _scrollView;

        public IList GetSourceList()
        {
            return _source;
        }

        public void AddItemsSquare()
        {
            VisualElement container = new VisualElement();
            container.AddToClassList("scroll_entry_container");

            for (int i = 0; i < _source.Count; i++)
            {
                VisualElement entry = new VisualElement();
                entry.AddToClassList("square_item");
                Label label = new Label(_source[i]);
                entry.Add(label);

                //Adds the container to scrollview and creates a new container if _squareRowEntryAmount items have already been added to existing container
                if (i != 0 && i % _squareRowEntryAmount == 0)
                {
                    _scrollView.Add(container);
                    container = new VisualElement();
                    container.AddToClassList("scroll_entry_container");
                }
                container.Add(entry);

                //Adds the container to scrollview at last iteration
                if (i == (_source.Count - 1))
                {
                    _scrollView.Add(container);
                }
            }
        }

        public void AddItemsLine()
        {
            for (int i = 0; i < _source.Count; i++)
            {
                VisualElement container = new VisualElement();
                container.AddToClassList("line_item_container");
                Label label = new Label(_source[i]);
                label.AddToClassList("line_item");
                container.Add(label);
                _scrollView.Add(container);
            }
        }

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();

            _scrollView = _uiDocument.rootVisualElement.Q<ScrollView>(_scrollViewName);

            if (_squareEntryStyle)
            {
                AddItemsSquare();
            }
            else
            {
                AddItemsLine();
            }
        }

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
        }

    }
}
