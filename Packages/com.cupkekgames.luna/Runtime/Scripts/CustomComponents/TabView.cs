using System;
using System.Collections.Generic;
using System.Linq;
using CupkekGames.Core;
using Unity.Properties;
using UnityEngine.UIElements;

#if UNITY_INPUT
using UnityEngine.InputSystem;
#endif

namespace CupkekGames.Luna
{
    [UxmlElement]
    public partial class TabView : VisualElement
    {
        private bool _horizontal = true;
        [UxmlAttribute]
        private bool Horizontal
        {
            get
            {
                return _horizontal;
            }
            set
            {
                _horizontal = value;

                SetHorizontal();
            }
        }
        private UIColorName _buttonColor = UIColorName.PRIMARY;
        [UxmlAttribute]
        private UIColorName ButtonColor
        {
            get
            {
                return _buttonColor;
            }
            set
            {
                _buttonColor = value;
            }
        }
        private UIColorName _buttonColorSelected = UIColorName.SECONDARY;
        [UxmlAttribute]
        private UIColorName ButtonColorSelected
        {
            get
            {
                return _buttonColorSelected;
            }
            set
            {
                _buttonColorSelected = value;
            }
        }
        private int _activeTab = 0;
        [UxmlAttribute]
        public int ActiveTab
        {
            get
            {
                return _activeTab;
            }
            set
            {
                _activeTab = value;

                OnTabSelected?.Invoke(ActiveTab);

                HandleOnTabSelected();
            }
        }
        private bool _showInputPrompts = true;
        [UxmlAttribute]
        private bool ShowInputPrompts
        {
            get
            {
                return _showInputPrompts;
            }
            set
            {
                _showInputPrompts = value;


                if (_prev != null)
                {
                    _prev.style.display = _showInputPrompts ? DisplayStyle.Flex : DisplayStyle.None;
                }
                if (_next != null)
                {
                    _next.style.display = _showInputPrompts ? DisplayStyle.Flex : DisplayStyle.None;
                }
            }
        }
        private string _inputActionPrev = "UI/Previous";
        [UxmlAttribute]
        private string InputActionPrev
        {
            get
            {
                return _inputActionPrev;
            }
            set
            {
                _inputActionPrev = value;

                _prev.InputActionName = _inputActionPrev;
            }
        }
        private string _inputActionNext = "UI/Next";
        [UxmlAttribute]
        private string InputActionNext
        {
            get
            {
                return _inputActionNext;
            }
            set
            {
                _inputActionNext = value;

                _next.InputActionName = _inputActionNext;
            }
        }
        private List<InputIconControlScheme> _hideInputPromptsIf = new List<InputIconControlScheme>()
        {
           InputIconControlScheme.KeyboardMouse
        };
        [UxmlAttribute]
        public List<InputIconControlScheme> HideInputPromptsIf
        {
            get
            {
                return _hideInputPromptsIf;
            }
            set
            {
                _hideInputPromptsIf = value;
            }
        }
        private const string _ussClassName = "TabView";
        private const string _ussHeaderContainer = _ussClassName + "__header-container";
        private const string _ussHeaderTabContainer = _ussClassName + "__header-tab-container";
        private const string _ussHeaderTabContainerButton = _ussClassName + "__header-tab";
        private const string _ussHeaderTabContainerButtonSelected = _ussClassName + "__header-tab-selected";
        private const string _ussHeaderTabInputPrompt = _ussClassName + "__header-tab-input-prompt";
        private VisualElement _headerContainer;
        public VisualElement HeaderContainer => _headerContainer;
        private VisualElement _headerTabContainer;
        public VisualElement HeaderTabContainer => _headerTabContainer;
        private InputPrompt _prev;
        private InputPrompt _next;
        // State
#if UNITY_INPUT
        private InputAction _prevAction;
        private InputAction _nextAction;
#endif
        // Events
        public event Action<int> OnTabSelected;

        public TabView()
        {
            AddToClassList(_ussClassName);

            _headerContainer = new VisualElement();
            _headerContainer.AddToClassList(_ussHeaderContainer);
            Add(_headerContainer);

            _prev = new InputPrompt
            {
                InputActionName = _inputActionPrev,
                HideIf = HideInputPromptsIf
            };
            _prev.AddToClassList(_ussHeaderTabInputPrompt);
            _headerContainer.Add(_prev);
            if (!_showInputPrompts)
            {
                _prev.style.display = DisplayStyle.None;
            }

            _headerTabContainer = new VisualElement();
            _headerTabContainer.AddToClassList(_ussHeaderTabContainer);
            _headerContainer.Add(_headerTabContainer);

            _next = new InputPrompt
            {
                InputActionName = _inputActionNext,
                HideIf = HideInputPromptsIf
            };
            _next.AddToClassList(_ussHeaderTabInputPrompt);
            _headerContainer.Add(_next);
            if (!_showInputPrompts)
            {
                _next.style.display = DisplayStyle.None;
            }

            SetHorizontal();

            this.RegisterCallback<AttachToPanelEvent>(e => OnAttach());
            this.RegisterCallback<DetachFromPanelEvent>(e => OnDetach());
        }

        private void OnAttach()
        {
            Setup();

#if UNITY_INPUT
            if (_showInputPrompts)
            {
                PlayerInput playerInput = InputDeviceManager.PlayerInput;

                if (playerInput != null)
                {
                    // Register input actions
                    _prevAction = playerInput.actions[_inputActionPrev];
                    if (_prevAction != null)
                    {
                        _prevAction.performed += PreviousTabInput;
                    }

                    _nextAction = playerInput.actions[_inputActionNext];
                    if (_nextAction != null)
                    {
                        _nextAction.performed += NextTabInput;
                    }
                }
            }
#endif
        }

        private void OnDetach()
        {
#if UNITY_INPUT
            if (_prevAction != null)
            {
                _prevAction.performed -= PreviousTabInput;
            }

            if (_nextAction != null)
            {
                _nextAction.performed -= NextTabInput;
            }
#endif
        }

        private void Setup()
        {
            _headerTabContainer.Clear();

            if (ActiveTab == -1)
            {
                ActiveTab = 0;
            }

            foreach (VisualElement ve in Children())
            {
                if (ve is UnityEngine.UIElements.Tab tab)
                {
                    SetupTab(tab, null);
                }
                else if (ve is CupkekGames.Luna.Tab lunaTab)
                {
                    SetupTab(null, lunaTab);
                }
            }

            HandleOnTabSelected();
        }
        private void SetupTab(UnityEngine.UIElements.Tab tab1, CupkekGames.Luna.Tab tab2)
        {
            Button button = new Button();

            button.AddToClassList(_ussHeaderTabContainerButton);
            button.AddToClassList("btn");
            button.AddToClassList("btn-lg");
            button.AddToClassList(ButtonColor.ToString().ToLowerInvariant());

            int tabIndex = _headerTabContainer.childCount;

            button.clicked += () => ActiveTab = tabIndex;

            bool iconImageCheck = (tab1 != null && tab1.iconImage != null) || (tab2 != null && tab2.IconImage != null);

            if (iconImageCheck)
            {
                VisualElement image = new VisualElement();
                image.AddToClassList("size-48");
                image.AddToClassList("size-min-48");

                button.Add(image);

                Label labelElement = null;
                if (tab1 != null)
                {
                    image.style.backgroundImage = tab1.iconImage;
                    labelElement = new Label(tab1.label);

                    labelElement.SetBinding(nameof(Label.text), new DataBinding
                    {
                        dataSource = tab1,
                        dataSourcePath = PropertyPath.FromName(nameof(UnityEngine.UIElements.Tab.label)),
                        bindingMode = BindingMode.ToTarget,
                    });
                }
                else if (tab2 != null)
                {
                    image.style.backgroundImage = new StyleBackground(tab2.IconImage);
                    labelElement = new Label(tab2.Label);

                    labelElement.SetBinding(nameof(Label.text), new DataBinding
                    {
                        dataSource = tab2,
                        dataSourcePath = PropertyPath.FromName(nameof(CupkekGames.Luna.Tab.Label)),
                        bindingMode = BindingMode.ToTarget,
                    });
                }

                button.Add(labelElement);
            }
            else
            {
                if (tab1 != null)
                {
                    button.text = tab1.label;

                    button.SetBinding(nameof(Button.text), new DataBinding
                    {
                        dataSource = tab1,
                        dataSourcePath = PropertyPath.FromName(nameof(UnityEngine.UIElements.Tab.label)),
                        bindingMode = BindingMode.ToTarget,
                    });
                }
                else if (tab2 != null)
                {
                    button.text = tab2.Label;

                    button.SetBinding(nameof(Button.text), new DataBinding
                    {
                        dataSource = tab2,
                        dataSourcePath = PropertyPath.FromName(nameof(CupkekGames.Luna.Tab.Label)),
                        bindingMode = BindingMode.ToTarget,
                    });
                }
            }

            _headerTabContainer.Add(button);

            if (tab2 != null && tab2.TabButtonContent != null)
            {
                VisualElement ve = tab2.TabButtonContent.CloneTree();
                ve.AddToClassList("absolute");

                if (tab2.ContentSizePercent.x > 0)
                {
                    ve.style.width = Length.Percent(tab2.ContentSizePercent.x);
                }
                if (tab2.ContentSizePercent.y > 0)
                {
                    ve.style.height = Length.Percent(tab2.ContentSizePercent.y);
                }

                if (tab2.HeaderMinSize.x > 0)
                {
                    button.style.minWidth = Length.Percent(tab2.HeaderMinSize.x);
                }
                if (tab2.HeaderMinSize.y > 0)
                {
                    button.style.minHeight = Length.Percent(tab2.HeaderMinSize.y);
                }

                if (tab2.Overflow)
                {
                    button.style.overflow = Overflow.Visible;
                }

                button.Add(ve);
            }
        }

        private void HandleOnTabSelected()
        {
            List<VisualElement> headers = _headerTabContainer.Children().ToList();

            int i = 0;
            foreach (VisualElement ve in Children())
            {
                if (i >= headers.Count)
                {
                    break;
                }

                if (ve == _headerContainer)
                {
                    continue;
                }

                if (i == ActiveTab)
                {
                    ve.style.display = DisplayStyle.Flex;
                    headers[i].AddToClassList(ButtonColorSelected.ToString().ToLowerInvariant());
                    headers[i].RemoveFromClassList(ButtonColor.ToString().ToLowerInvariant());
                    headers[i].AddToClassList(_ussHeaderTabContainerButtonSelected);
                }
                else
                {
                    ve.style.display = DisplayStyle.None;
                    headers[i].RemoveFromClassList(ButtonColorSelected.ToString().ToLowerInvariant());
                    headers[i].AddToClassList(ButtonColor.ToString().ToLowerInvariant());
                    headers[i].RemoveFromClassList(_ussHeaderTabContainerButtonSelected);
                }

                i++;
            }
        }

        private void SetHorizontal()
        {
            if (Horizontal)
            {
                AddToClassList("horizontal");
                RemoveFromClassList("vertical");
            }
            else
            {
                RemoveFromClassList("horizontal");
                AddToClassList("vertical");
            }
        }

#if UNITY_INPUT
        private void NextTabInput(InputAction.CallbackContext context)
        {
            NextTab();
        }
        private void PreviousTabInput(InputAction.CallbackContext context)
        {
            PreviousTab();
        }
#endif

        public void NextTab()
        {
            // Increment index and loop back to the first if at the end
            int count = _headerTabContainer.childCount;
            ActiveTab = (ActiveTab + 1) % count;
        }

        public void PreviousTab()
        {
            // Decrement index and loop back to the last if at the beginning
            int count = _headerTabContainer.childCount;
            ActiveTab = (ActiveTab - 1 + count) % count;
        }
        public Button GetTabHeader(int index)
        {
            return _headerTabContainer.Children().ElementAt(index) as Button;
        }
    }
}