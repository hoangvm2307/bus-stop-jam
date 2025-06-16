using System.Collections.Generic;
using System.Text;
using CupkekGames.InventorySystem;
using CupkekGames.Systems;
using CupkekGames.Luna.Library;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna.Demo.Newtonsoft
{
  public class NewtonsoftDemo : UIViewComponent
  {
    [SerializeField] GameSaveManagerExample _saveManager;
    [SerializeField] InventoryItemDatabaseExample _inventoryItemDatabase;
    private Button _save;
    private Button _load;
    private Label _current;
    private Label _saved;
    private int _nextSaveSlot = 0;
    // Data edit buttons
    private Button _addGold;
    private Button _addRandomItem;
    private Button _removeGold;
    private Button _removeRandomItem;

    protected override void Awake()
    {
      base.Awake();

      // Locate the button within the ParentElement
      _save = ParentElement.Q<Button>("Save");
      _save.text = "Save to slot " + _nextSaveSlot;
      _load = ParentElement.Q<Button>("Load");
      _load.text = "Load latest save file";

      _save.clicked += OnSave;
      _load.clicked += OnLoad;

      _current = ParentElement.Q<Label>("Current");
      _saved = ParentElement.Q<Label>("Saved");

      UpdateCurrentData();
      UpdateSavedData();

      _addGold = ParentElement.Q<Button>("AddGold");
      _addRandomItem = ParentElement.Q<Button>("AddRandomItem");
      _removeGold = ParentElement.Q<Button>("RemoveGold");
      _removeRandomItem = ParentElement.Q<Button>("RemoveRandomItem");

      _addGold.clicked += () => OnAddGold();
      _addRandomItem.clicked += () => OnAddRandomItem();
      _removeGold.clicked += () => OnRemoveGold();
      _removeRandomItem.clicked += () => OnRemoveRandomItem();
    }

    private string[] GetSaveInfo(GameSaveDataExample save)
    {
      List<string> info = new List<string>
      {
        "Player Name: " + save.PlayerName,
        "Gold: " + save.Gold.ToString(),
        "Diamond: " + save.Diamond.ToString(),
        "Exp: " + save.Exp.ToString(),
        "Lvl: " + save.Lvl.ToString(),
        "ExpReq: " + save.ExpReq.ToString()
      };

      info.Add("Inventory Items:");
      foreach (var item in save.Inventory.Items)
      {
        InventoryItemDefinition itemDefinition = _inventoryItemDatabase.GetItemDefinition(item.ItemType, item.Key);
        info.Add($"{item.GetType().Name} - {itemDefinition.Name} - Amount: {item.Amount}");
      }

      return info.ToArray();
    }

    private string BuildLabelText(string[] values)
    {
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < values.Length; i++)
      {
        sb.Append(values[i]);
        if (i < values.Length - 1)
          sb.AppendLine();
      }
      return sb.ToString();
    }

    private void OnSave()
    {
      GameSaveDataExample current = _saveManager.CurrentSave.Data;
      // Save logic here
      _saveManager.SaveToFile(_nextSaveSlot, current);
      _nextSaveSlot++;
      _save.text = "Save to slot " + _nextSaveSlot;

      UpdateSavedData();
    }

    private void OnLoad()
    {
      GameSaveMetadataWithSlot<GameSaveMetadataExample> metadata = _saveManager.GetLastMetadata();
      if (metadata.Metadata != null)
      {
        _saveManager.CurrentSave.Data = _saveManager.GetSave(metadata.SaveSlot);
        UpdateCurrentData();
      }
    }

    private void UpdateCurrentData()
    {
      _current.text = "Current Data: \n" + BuildLabelText(GetSaveInfo(_saveManager.CurrentSave.Data));
    }

    private void UpdateSavedData()
    {
      List<GameSaveMetadataWithSlot<GameSaveMetadataExample>> metadata = _saveManager.GetAllMetadata(true);
      List<string> saveInfo = new List<string>();
      
      saveInfo.Add("Save File Amount: " + metadata.Count);
      if (metadata.Count > 0)
      {
        saveInfo.Add("Last Save Date: " + metadata[0].Metadata.SaveDate.ToString());
        saveInfo.Add("Last Save Slot: " + metadata[0].SaveSlot);
        saveInfo.Add("Last Save Data: \n" + BuildLabelText(GetSaveInfo(_saveManager.GetSave(metadata[0].SaveSlot))));
      }
      
      _saved.text = BuildLabelText(saveInfo.ToArray());
      if (metadata.Count > 0)
      {
        _load.text = "Load last save file: " + metadata[0].SaveSlot;
      }
    }

    private void OnDestroy()
    {
      _save.clicked -= OnSave;
      _load.clicked -= OnLoad;
    }

    private void OnAddGold()
    {
      GameSaveDataExample current = _saveManager.CurrentSave.Data;
      current.Gold += 100;
      UpdateCurrentData();
    }

    private void OnAddRandomItem()
    {
      InventoryItem item;
      
      int randomItemType = UnityEngine.Random.Range(0, 2);
      if (randomItemType == 0)
      {
        item = new Potion("HealPotion");
        item.SetAmount(UnityEngine.Random.Range(4, 50));
      } else {
        item = new Equipment("ShortSword");
        item.SetAmount(UnityEngine.Random.Range(4, 50));
      }

      GameSaveDataExample current = _saveManager.CurrentSave.Data;
      current.Inventory.AddItem(item);
      UpdateCurrentData();
    }

    private void OnRemoveGold()
    {
      GameSaveDataExample current = _saveManager.CurrentSave.Data;
      current.Gold = Mathf.Max(0, current.Gold - 100);
      UpdateCurrentData();
    }

    private void OnRemoveRandomItem()
    {
      GameSaveDataExample current = _saveManager.CurrentSave.Data;
      if (current.Inventory.Items.Count > 0)
      {
        int randomIndex = UnityEngine.Random.Range(0, current.Inventory.Items.Count);
        current.Inventory.RemoveItem(current.Inventory.Items[randomIndex]);
        UpdateCurrentData();
      }
    }
  }
}
