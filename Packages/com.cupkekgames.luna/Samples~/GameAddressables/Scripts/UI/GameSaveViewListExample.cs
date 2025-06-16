#if UNITY_ADDRESSABLES
using UnityEngine.UIElements;
using System.Collections.Generic;
using CupkekGames.Luna.Library;
using UnityEngine;
using CupkekGames.Systems;
using CupkekGames.Systems.UI;
using System;

namespace CupkekGames.Luna.Demo.Game.Addressables
{
  public class GameSaveViewListExample : GameSaveViewList<GameSaveDataExample, GameSaveMetadataExample>
  {
    [SerializeField] GameSaveManagerExample _saveManager;
    protected override GameSaveManager<GameSaveDataExample, GameSaveMetadataExample> GetSaveManager()
    {
      return _saveManager;
    }

    protected override bool IsInGame()
    {
      // If MainMenu Scene is not loaded, we are not in game so return false to disable saving
      return !SceneLoaderAddressable.Instance.IsLoaded(SceneDatabase.Instance.GetValue("MainMenu"));
    }

    protected override VisualElement SlotOne(int index, GameSaveMetadataWithSlot<GameSaveMetadataExample> metadata)
    {
      VisualElement container = new();
      container.AddToClassList("flex-row");

      VisualElement containerLeft = new();
      container.Add(containerLeft);
      VisualElement containerRight = new();
      container.Add(containerRight);

      containerLeft.Add(new Label("Index: " + index));
      containerLeft.Add(new Label("File: " + metadata.SaveSlot));

      containerRight.Add(new Label(metadata.Metadata.SaveDate.ToString()));
      string autosave = metadata.Metadata.IsAutosave ? "Autosave" : "Manual Save";
      containerRight.Add(new Label(autosave));

      VisualElement containerEnd = new();
      container.Add(containerEnd);

      containerEnd.Add(new Label("Save Version: " + metadata.Metadata.SaveVersion));

      return container;
    }

    protected override VisualElement SlotTwo(int index, GameSaveMetadataWithSlot<GameSaveMetadataExample> metadata)
    {
      return new Label("Gold: " + metadata.Metadata.Gold);
    }

    protected override void OnLoadButtonClicked()
    {
      GameSaveMetadataWithSlot<GameSaveMetadataExample>? metadata = GetSelectedMetadata();
      if (!metadata.HasValue)
      {
        return;
      }

      try {
        GameSaveManager.CurrentSave.Data = GameSaveManager.GetSave(metadata.Value.SaveSlot);
      }
      catch (Exception e)
      {
        Debug.LogException(e, this);
        return;
      }

      // Set the player base as the active scene
      SceneSO sceneBase = SceneDatabase.Instance.GetValue("Base");
      SceneSO sceneGameManager = SceneDatabase.Instance.GetValue("GameManager");
      SceneLoaderAddressable.Instance.SetActiveScene(sceneBase);

#if UNITY_INPUT
      SceneLoaderAddressable.Instance.UnloadAllCurrent(SceneTransitionDatabase.Instance.Transitions.GetValue("FadeWithInput"));
#else
      SceneLoaderAddressable.Instance.UnloadAllCurrent(SceneTransitionDatabase.Instance.Transitions.GetValue("Fade"));
#endif

      List<SceneSO> scenesToLoad = new List<SceneSO>() { sceneGameManager, sceneBase };

#if UNITY_INPUT
      SceneLoaderAddressable.Instance.LoadScene(scenesToLoad, SceneTransitionDatabase.Instance.Transitions.GetValue("FadeWithInput"));
#else
      SceneLoaderAddressable.Instance.LoadScene(scenesToLoad, SceneTransitionDatabase.Instance.Transitions.GetValue("Fade"));
#endif

      GameSaveView.ReturnClicked(); // unloads prefab
    }

    protected override TooltipController GetTooltipController()
    {
      return TooltipDatabaseExample.Instance.TooltipController;
    }
  }
}
#endif