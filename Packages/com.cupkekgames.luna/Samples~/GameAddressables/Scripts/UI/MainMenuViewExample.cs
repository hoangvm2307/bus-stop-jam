#if UNITY_ADDRESSABLES
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using CupkekGames.Luna.Library;
using CupkekGames.Systems;
using CupkekGames.Systems.UI;
using System;

namespace CupkekGames.Luna.Demo.Game.Addressables
{
    public class MainMenuViewExample : MainMenuView<GameSaveDataExample, GameSaveMetadataExample>
    {
        [Header("Prefab Keys")]
        [SerializeField] string _settingsMenuKey = "Settings";
        [SerializeField] string _loadMenuKey = "SaveLoad";
        [SerializeField] string _creditsMenuKey = "Credits";
        [Header("Save Manager")]
        [SerializeField] GameSaveManagerExample _saveManager;
        [Header("Game Scenes")]
        [Header("Loaded by order. Last element will become the active scene.")]
        [SerializeField] List<string> _gameScenes = new();

        // UI
        private VisualElement _background;
        private VisualElement _title;
        private VisualElement _mainButtonContainer;
        private VisualElement _bottomContainer;
        private TooltipController _tooltipController;
        // Animations
        private List<VisualElement> _toAnimate = new List<VisualElement>();
        private List<IVisualElementScheduledItem> _stateAnimationSchedules = new List<IVisualElementScheduledItem>();
        // Modal
        protected ChoicePopupController _choicePopupController;

        protected override void Awake()
        {
            base.Awake();

            _choicePopupController = GetComponent<ChoicePopupController>();

            _toAnimate = UIDocument.rootVisualElement.Query<VisualElement>("Char").ToList();

            _background = UIDocument.rootVisualElement.Q<VisualElement>("Background");
            _title = UIDocument.rootVisualElement.Q<VisualElement>("Title");
            _mainButtonContainer = UIDocument.rootVisualElement.Q<VisualElement>("MainButtonContainer");
            _bottomContainer = UIDocument.rootVisualElement.Q<VisualElement>("BottomContainer");

            // Add tooltip to quit button
            _tooltipController = TooltipDatabaseExample.Instance.TooltipController;

            List<TooltipContainerSetup> setups = new()
            {
                new TooltipContainerSetup(
                  null,
                  new Label("Quit Game"),
                  null,
                  null,
                  new UIColor(UIColorName.BASE, UIColorValue.V_700),
                  new UIColor(UIColorName.BASE, UIColorValue.V_800)
                )
            };
            Manipulator manipulator = new TooltipManipulator(
                  gameObject,
                  _tooltipController,
                  setups
                );

            _buttonQuit.AddManipulator(manipulator);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _choicePopupController.OnButtonClick += OnButtonClick;

            StartCoroutine(StartingAnimation());

            // Change what element is focused next when using directional navigation
            _buttonSettings.RegisterCallback<NavigationMoveEvent>(e =>
            {
                if (e.direction == NavigationMoveEvent.Direction.Down)
                {
                    _buttonQuit.Focus();
                    e.StopPropagation(); // Prevent further processing of the event
                    _buttonSettings.focusController.IgnoreEvent(e); // Prevent default navigation
                }
            });

            _buttonQuit.RegisterCallback<NavigationMoveEvent>(e =>
            {
                switch (e.direction)
                {
                    case NavigationMoveEvent.Direction.Up: _buttonSettings.Focus(); break;
                    case NavigationMoveEvent.Direction.Down: _buttonContinue.Focus(); break;
                    case NavigationMoveEvent.Direction.Left: _buttonSettings.Focus(); break;
                }

                e.StopPropagation(); // Prevent further processing of the event
                _buttonSettings.focusController.IgnoreEvent(e); // Prevent default navigation
            });

            _buttonContinue.RegisterCallback<NavigationMoveEvent>(e =>
            {
                if (e.direction == NavigationMoveEvent.Direction.Up)
                {
                    _buttonQuit.Focus();
                    e.StopPropagation(); // Prevent further processing of the event
                    _buttonSettings.focusController.IgnoreEvent(e); // Prevent default navigation
                }
            });
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _choicePopupController.OnButtonClick -= OnButtonClick;

            foreach (IVisualElementScheduledItem element in _stateAnimationSchedules)
            {
                element.Pause();
            }
            _stateAnimationSchedules.Clear();
        }
        public IEnumerator StartingAnimation()
        {
            WaitForSeconds delay = new WaitForSeconds(0.4f);
            yield return delay;

            _mainButtonContainer.RemoveFromClassList("animation");

            yield return delay;

            _title.RemoveFromClassList("animation");
            _bottomContainer.RemoveFromClassList("animation");

            yield return delay;

            for (int i = 0; i < _toAnimate.Count; i++)
            {
                // Schedule the first transition 100 milliseconds after the root.schedule.Execute method is called.
                long startDelay = (i % 2) * 0;
                VisualElement element = _toAnimate[i];
                _stateAnimationSchedules.Add(element.schedule.Execute(() => element.ToggleInClassList("animation")).StartingIn(startDelay).Every(2400));
            }
        }
        protected override GameSaveManager<GameSaveDataExample, GameSaveMetadataExample> GetSaveManager()
        {
            return _saveManager;
        }
        protected override void OnButtonContinueClicked()
        {
            try {
                GameSaveManager.CurrentSave.Data = GameSaveManager.GetSave(LastSaveSlot);
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
                return;
            }

            // Set the player base as the active scene
            SceneSO sceneBase = SceneDatabase.Instance.GetValue(_gameScenes[_gameScenes.Count - 1]);
            SceneLoaderAddressable.Instance.SetActiveScene(sceneBase);

            List<SceneSO> scenesToLoad = _gameScenes
              .Where(key => SceneDatabase.Instance.ContainsKey(key))  // Ensure the key exists in the dictionary
              .Select(key => SceneDatabase.Instance.GetValue(key))             // Select the values corresponding to the keys
              .ToList();

#if UNITY_INPUT
#if UNITY_SHADER_GRAPH
            SceneLoaderAddressable.Instance.LoadSceneAndUnLoadCurrent(scenesToLoad, SceneTransitionDatabase.Instance.Transitions.GetValue("CircleWithInput"));
#else
            SceneLoaderAddressable.Instance.LoadSceneAndUnLoadCurrent(scenesToLoad, SceneTransitionDatabase.Instance.Transitions.GetValue("FadeWithInput"));
#endif
#else
#if UNITY_SHADER_GRAPH
            SceneLoaderAddressable.Instance.LoadSceneAndUnLoadCurrent(scenesToLoad, SceneTransitionDatabase.Instance.Transitions.GetValue("Circle"));
#else
            SceneLoaderAddressable.Instance.LoadSceneAndUnLoadCurrent(scenesToLoad, SceneTransitionDatabase.Instance.Transitions.GetValue("Fade"));
#endif
#endif
        }
        protected override void OnButtonLoadClicked()
        {
            UIPrefabLoaderString.Instance.Instantiate(_loadMenuKey);
        }

        protected override void OnButtonNewGameClicked()
        {
            _buttonNewGame.SetEnabled(false);

            GameSaveManager.CurrentSave.Data = new GameSaveDataExample();

            // Set the player base as the active scene
            SceneSO sceneBase = SceneDatabase.Instance.GetValue(_gameScenes[_gameScenes.Count - 1]);
            SceneLoaderAddressable.Instance.SetActiveScene(sceneBase);

            List<SceneSO> scenesToLoad = _gameScenes
              .Where(key => SceneDatabase.Instance.ContainsKey(key))  // Ensure the key exists in the dictionary
              .Select(key => SceneDatabase.Instance.GetValue(key))             // Select the values corresponding to the keys
              .ToList();

#if UNITY_INPUT
            SceneLoaderAddressable.Instance.LoadSceneAndUnLoadCurrent(scenesToLoad, SceneTransitionDatabase.Instance.Transitions.GetValue("FadeWithInput"));
#else
            SceneLoaderAddressable.Instance.LoadSceneAndUnLoadCurrent(scenesToLoad, SceneTransitionDatabase.Instance.Transitions.GetValue("FadeWithInput"));
#endif
        }

        protected override void OnButtonCreditsClicked()
        {
            UIPrefabLoaderString.Instance.Instantiate(_creditsMenuKey);
        }

        protected override void OnButtonSettingsClicked()
        {
            UIPrefabLoaderString.Instance.Instantiate(_settingsMenuKey);
        }
        protected override void OnButtonQuitClicked()
        {
            _choicePopupController.Fade.FadeIn();
        }
        private void OnButtonClick(int i)
        {
            if (i == 1)
            {
                Application.Quit();
            }
        }
    }
}
#endif