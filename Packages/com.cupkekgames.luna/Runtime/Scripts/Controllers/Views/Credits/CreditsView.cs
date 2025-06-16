using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using CupkekGames.Core;
using CupkekGames.Luna;

namespace CupkekGames.Luna
{
    public class CreditsView : UIViewComponent
    {
        //Editable fields
        [SerializeField] private bool _autoCloseCredits = true;
        [SerializeField] private float _scrollSpeed = 1f;
        [SerializeField] public long _updateFrequency = 10;
        [SerializeField] public long _scrollStartDelay = 0;
        [SerializeField] private string _gameTitle;
        [SerializeField] private string _endMessage;
        [SerializeField] private float _afterEndMessageDelay = 100;
        [SerializeField] private KeyValueDatabase<string, List<string>> _sections = new();
        //Task for scheduling and executing scrolling
        private IVisualElementScheduledItem _taskValue;
        //UI Elements
        private Label _gameTitleElement;
        private Label _endMessageElement;
        private VisualElement _verticalScrollerContainer;
        private Scroller _verticalScroller;
        private ScrollView _scrollView;
        private Button _returnButton;
        // Start is called before the first frame update
        protected override void Awake()
        {
            base.Awake();

            _scrollView = UIDocument.rootVisualElement.Q<ScrollView>();
            _returnButton = UIDocument.rootVisualElement.Q<Button>("ReturnButton");

            //Add Game Title to scrollview
            _gameTitleElement = new Label(_gameTitle);
            _gameTitleElement.AddToClassList("game_title");
            _scrollView.Add(_gameTitleElement);

            //Adds each section in _sections to scrollView
            foreach (var key in _sections.Keys)
            {
                //Add section title to scrollview
                Label sectionTitle = new Label(key);
                sectionTitle.AddToClassList("section_title");
                _scrollView.Add(sectionTitle);
                //Add each credit under section to scrollview
                var value = _sections.GetValue(key);
                foreach (string str in value)
                {
                    VisualElement creditLineContainer = new VisualElement { pickingMode = PickingMode.Ignore };
                    creditLineContainer.AddToClassList("credit_line_container");

                    if (str.Contains(":"))
                    {
                        //gets the text before the character ":" and assigns to position(e.g. for voice actor credits, position would be their character)
                        string creditedPosition = str.Substring(0, str.IndexOf(":"));
                        Label creditedPositionElement = new Label(creditedPosition);
                        creditedPositionElement.AddToClassList("credited_position");

                        //gets the text after the character ":" and assigns to person(e.g. for voice actor credits, person would be the actor's name)
                        string creditedPerson = str.Substring((str.IndexOf(":") + 1), str.Length - (str.IndexOf(":") + 1));
                        Label creditedPersonElement = new Label(creditedPerson);
                        creditedPersonElement.AddToClassList("credited_person");

                        creditLineContainer.Add(creditedPositionElement);
                        creditLineContainer.Add(creditedPersonElement);
                    }
                    else
                    {
                        Label creditLine = new Label(str);
                        creditLine.AddToClassList("credit_line");
                        creditLineContainer.Add(creditLine);
                    }

                    _scrollView.Add(creditLineContainer);
                }
            }
            //Add End Message to scrollview
            _endMessageElement = new Label(_endMessage);
            _endMessageElement.AddToClassList("end_message");
            _scrollView.Add(_endMessageElement);
            //Get vertical scroller and make it invisible
            _verticalScrollerContainer = UIDocument.rootVisualElement.Q<VisualElement>("unity-content-and-vertical-scroll-container");
            _verticalScroller = _verticalScrollerContainer.Q<Scroller>();
            _verticalScroller.style.visibility = Visibility.Hidden;
        }

        protected virtual void OnEnable()
        {
            _returnButton.clicked += FadeOutThenDestroy;

            ScrollToEnd();
        }

        protected virtual void OnDisable()
        {
            _returnButton.clicked -= FadeOutThenDestroy;
            _taskValue.Pause();
        }

        private void ScrollToEnd()
        {
            float _check = _verticalScroller.value;
            _taskValue = _verticalScroller.schedule
                .Execute(() =>
                {
                    _verticalScroller.value += _scrollSpeed;
                    _check += _scrollSpeed;
                })
                .Every(_updateFrequency)
                .Until(() =>
                          {
                              // Check if the condition to stop the task is met
                              // _verticalScroller.value never goes above maximum value, while check will grow over it one more iteration before isCompleted is true
                              bool isCompleted = _check == _verticalScroller.value + _afterEndMessageDelay ? true : false;
                              if (isCompleted)
                              {
                                  if (_autoCloseCredits)
                                  {
                                      //Exit credits if _autoCloseCredits is set to true
                                      FadeOutThenDestroy();
                                  }
                              }

                              return isCompleted;
                          })
                .StartingIn(_scrollStartDelay);
        }
    }
}