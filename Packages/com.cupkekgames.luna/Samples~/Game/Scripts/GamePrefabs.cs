using System;
using CupkekGames.Core;
using CupkekGames.Systems;
using UnityEngine;

namespace CupkekGames.Luna.Demo.Game.Standart
{
    public class GamePrefabs : PrefabLoader<string>
    {
        [MultiLineHeader("Game Prefabs\n\n" +
            "These prefabs are instantiated when entering the game from the Main Menu,\n" +
            "and are destroyed when returning to the Main Menu.\n\n" +
            "1 - PauseMenuEscapeAction.\n" +
            "This prefab registers the Action that opens PauseMenu to InputEscapeManager.\n" +
            "So escape input opens the PauseMenu in the game scene but not in the Main Menu.\n\n" +
            "2 - ItemDatabase.\n" +
            "This prefab is not required in the Main Menu but is a dependency in the game scene."
        )]
        [SerializeField] string _mainMenuSceneKey = "MainMenu";
        [SerializeField] int _mainMenuSceneIndex = 1;
        [SerializeField] string _gameSceneKey = "Base";
        [SerializeField] int _gameSceneIndex = 2;
        private bool _isGameSceneLoaded = false;

        private void Start()
        {
            SceneLoader.Instance.OnSceneLoad += OnSceneLoad;
        }

        private void OnSceneLoad(string name, int index)
        {
            if (name == _mainMenuSceneKey || index == _mainMenuSceneIndex)
            {
                OnGameDestroy();
            }
            else if (name == _gameSceneKey || index == _gameSceneIndex)
            {
                OnGameStart();
            }
        }

        private void OnGameStart()
        {
            if (_isGameSceneLoaded)
            {
                DestroyAll();
            }

            _isGameSceneLoaded = true;
            foreach (var key in Keys)
            {
                GameObject go = Instantiate(key);

                DontDestroyOnLoad(go);
            }
        }

        private void OnGameDestroy()
        {
            _isGameSceneLoaded = false;
            DestroyAll();
        }
    }
}