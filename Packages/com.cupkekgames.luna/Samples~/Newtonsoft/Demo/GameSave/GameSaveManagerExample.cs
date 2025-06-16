using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CupkekGames.Core;
using CupkekGames.Systems;
using CupkekGames.Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace CupkekGames.Luna.Demo.Newtonsoft
{
  [CreateAssetMenu(fileName = "NewtonsoftGameSaveManagerExample", menuName = "CupkekGames/Samples/Newtonsoft/GameSaveManagerExample")]
  public class GameSaveManagerExample : GameSaveManager<GameSaveDataExample, GameSaveMetadataExample>
  {
    public const string SUBFOLDER = "saves";
    public const string FILE_EXTENSION = "json";
    public const int SAVE_VERSION = 1;
    private string SaveDirectory => System.IO.Path.Combine(Application.persistentDataPath, SUBFOLDER);

    private void EnsureSaveDirectoryExists()
    {
        if (!Directory.Exists(SaveDirectory))
        {
            Directory.CreateDirectory(SaveDirectory);
        }
    }

    protected override List<string> GetAllFileNames()
    {
        try
        {
            EnsureSaveDirectoryExists();
            var result = Directory.GetFiles(SaveDirectory, $"*.{FILE_EXTENSION}")
                                .Select(path => System.IO.Path.GetFileName(path))
                                .ToList();

            Debug.Log($"Found {result.Count} save files");
            return result;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error getting save files: {ex.Message}");
            return new List<string>();
        }
    }

    protected override GameSaveDataExample GetNewSave(string saveVersion)
    {
        var newSave = new GameSaveDataExample();
        newSave.Metadata = newSave.CreateMetadata(saveVersion, false);
        return newSave;
    }

    protected override void OnDeleteRequest(int saveSlot, string fileName)
    {
        try
        {
            string fullPath = System.IO.Path.Combine(SaveDirectory, fileName);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                Debug.Log($"Deleted save file: {fileName}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error deleting save file {fileName}: {ex.Message}");
        }
    }

    protected override void OnSaveRequest(int saveSlot, string fileName, GameSaveDataExample data, bool autosave)
    {
        try
        {
            if (autosave)
            {
                GameSaveEvents.AutosaveStart?.Invoke();
            }

            EnsureSaveDirectoryExists();

            data.Metadata = data.CreateMetadata(GetSaveVersion(), autosave);
            var metadata = data.Metadata as GameSaveMetadataExample;
            UpdateMetadata(metadata, data);

            string fullPath = System.IO.Path.Combine(SaveDirectory, fileName);
            string json = SerializationManager.Instance.Serialize(data);
            File.WriteAllText(fullPath, json);

            Debug.Log($"Successfully saved to: {fileName}");

            if (autosave)
            {
                GameSaveEvents.AutosaveComplete?.Invoke();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error saving to file {fileName}: {ex.Message}");
            throw;
        }
    }

    private void UpdateMetadata(GameSaveMetadataExample metadata, GameSaveDataExample playerData)
    {
        metadata.Gold = playerData.Gold;
        // Add more metadata here if needed
    }

    protected override string GetFileExtenstion() => FILE_EXTENSION;

    protected override GameSaveDataExample LoadFromFile(string fileName)
    {
        try
        {
            string fullPath = System.IO.Path.Combine(SaveDirectory, fileName);
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"Save file not found: {fileName}");
            }

            string json = File.ReadAllText(fullPath);
            return SerializationManager.Instance.Deserialize<GameSaveDataExample>(json);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error loading save file {fileName}: {ex.Message}");
            throw;
        }
    }

    protected override GameSaveMetadataExample LoadMetadataFromFile(string fileName)
    {
        try
        {
            string fullPath = System.IO.Path.Combine(SaveDirectory, fileName);
            if (!File.Exists(fullPath))
            {
                // throw new FileNotFoundException($"Save file not found: {fileName}");
                return null;
            }

            using var streamReader = File.OpenText(fullPath);
            using var jsonReader = new JsonTextReader(streamReader);

            // Read to Metadata property
            jsonReader.Read(); // StartObject
            jsonReader.Read(); // PropertyName

            if (jsonReader.TokenType == JsonToken.PropertyName &&
                jsonReader.Value?.ToString() == "Metadata")
            {
                jsonReader.Read();
                var fastPathMetadataJson = JObject.Load(jsonReader);
                return SerializationManager.Instance.Deserialize<GameSaveMetadataExample>(fastPathMetadataJson.ToString());
            }

            // Fallback: parse entire file
            string json = File.ReadAllText(fullPath);
            var jsonObject = JObject.Parse(json);
            var metadataJson = jsonObject["Metadata"];

            if (metadataJson == null)
            {
                // throw new Exception($"Metadata not found in save file: {fileName}");
                return null;
            }

            return SerializationManager.Instance.Deserialize<GameSaveMetadataExample>(metadataJson.ToString());
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error loading metadata from {fileName}: {ex.Message}");
            throw;
        }
    }

    protected override string GetSaveVersion() => SAVE_VERSION.ToString();
  }
}