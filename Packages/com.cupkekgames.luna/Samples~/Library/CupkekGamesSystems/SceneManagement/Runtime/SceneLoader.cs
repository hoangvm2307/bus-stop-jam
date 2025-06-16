using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using CupkekGames.Core;
using CupkekGames.Luna;

namespace CupkekGames.Systems
{
    public class SceneLoader : Singleton<SceneLoader>
    {
        [SerializeField] string _startSceneName = "";
        [Header("Index is used if name is empty")]
        [SerializeField] int _startSceneIndex = 1;
        [SerializeField] string _transitionKey = "Fade";
        private SceneTransition _sceneTransition;

        public event Action<string, int> OnSceneLoad;

        private void Start()
        {
            _sceneTransition = SceneTransitionDatabase.Instance.Transitions.GetValue(_transitionKey);

            if (string.IsNullOrEmpty(_startSceneName))
            {
                LoadScene(_startSceneIndex, _sceneTransition);
            }
            else
            {
                LoadScene(_startSceneName, _sceneTransition);
            }
        }

        public void LoadScene(string sceneName, SceneTransition sceneLoadTransition)
        {
            StartCoroutine(LoadSceneAsync(sceneName, -1, sceneLoadTransition));
        }

        public void LoadScene(int sceneIndex, SceneTransition sceneLoadTransition)
        {
            StartCoroutine(LoadSceneAsync(null, sceneIndex, sceneLoadTransition));
        }

        // Coroutine for loading the scene asynchronously
        private IEnumerator LoadSceneAsync(string sceneName, int sceneIndex, SceneTransition sceneLoadTransition)
        {
            // Activate loading UI
            sceneLoadTransition.FadeIn();

            float delay = sceneLoadTransition.GetStartDelay();

            if (delay > 0)
            {
                yield return new WaitForSeconds(delay);
            }

            // Start loading the scene asynchronously
            AsyncOperation operation;
            if (sceneIndex >= 0)
            {
                operation = SceneManager.LoadSceneAsync(sceneIndex);
            }
            else
            {
                operation = SceneManager.LoadSceneAsync(sceneName);
            }

            // Prevent the scene from activating immediately when loading is complete
            operation.allowSceneActivation = false;

            // Update the UI based on loading progress
            while (!operation.isDone)
            {
                // // Mathf.Clamp01 to ensure the value stays between 0 and 1
                // float progress = Mathf.Clamp01(operation.progress / 0.9f);

                // // Update the slider and text UI elements
                // loadingBar.value = progress;
                // progressText.text = (progress * 100f).ToString("F0") + "%";

                // Check if the scene has finished loading (progress >= 0.9f)
                if (operation.progress >= 0.9f)
                {
                    // Optionally, wait for a button press or delay before activating the scene
                    // For simplicity, we just activate it here
                    operation.allowSceneActivation = true;
                }

                yield return null;  // Wait for the next frame
            }

            sceneLoadTransition.FadeOut();

            OnSceneLoad?.Invoke(sceneName, sceneIndex);
        }

        public bool IsLoaded(string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            return scene.IsValid() && scene.isLoaded;
        }
        public bool IsLoaded(int sceneIndex)
        {
            Scene scene = SceneManager.GetSceneByBuildIndex(sceneIndex);
            return scene.IsValid() && scene.isLoaded;
        }
    }
}