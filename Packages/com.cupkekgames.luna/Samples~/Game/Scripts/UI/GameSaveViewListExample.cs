using UnityEngine.UIElements;
using CupkekGames.Luna.Library;
using UnityEngine;
using CupkekGames.Systems.UI;
using CupkekGames.Systems;
using System;

namespace CupkekGames.Luna.Demo.Game.Standart
{
  public class GameSaveViewListExample : GameSaveViewList<GameSaveDataExample, GameSaveMetadataExample>
  {
    [SerializeField] GameSaveManagerExample _saveManager;
    protected override GameSaveManager<GameSaveDataExample, GameSaveMetadataExample> GetSaveManager()
    {
      // You can use alternative ways to get save manager
      // Here we are using serialized field to get save manager
      return _saveManager;
    }

    protected override bool IsInGame()
    {
      // If MainMenu Scene is loaded, we are not in game so return false to disable saving
      return !SceneLoader.Instance.IsLoaded(1);
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

#if UNITY_INPUT
      SceneLoader.Instance.LoadScene(2, SceneTransitionDatabase.Instance.Transitions.GetValue("FadeWithInput"));
#else
      SceneLoader.Instance.LoadScene(2, SceneTransitionDatabase.Instance.Transitions.GetValue("Fade"));
#endif

      GameSaveView.ReturnClicked(); // unloads prefab
    }

    protected override TooltipController GetTooltipController()
    {
      // You can use alternative ways to get tooltip controller if you don't like singleton solution
      return TooltipDatabaseExample.Instance.TooltipController;
    }
  }
}