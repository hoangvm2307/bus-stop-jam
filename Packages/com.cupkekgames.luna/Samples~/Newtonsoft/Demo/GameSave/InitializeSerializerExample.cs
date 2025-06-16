using UnityEngine;
using CupkekGames.Newtonsoft;
using System;
using CupkekGames.Systems;
using Newtonsoft.Json;
using CupkekGames.InventorySystem;

namespace CupkekGames.Luna.Demo.Newtonsoft
{
    public class InitializeSerializerExample : MonoBehaviour
    {
        [SerializeField] private SerializationManager _serializationManager;
        private void Awake()
        {
            _serializationManager.Initialize(
                // Specify allowed types for serialization to prevent security vulnerabilities
                // This is security when using TypeNameHandling.Auto as it could allow arbitrary code execution
                // You can set this to null if you want to serialize everything, but it is not recommended
                new Type[] {
                    typeof(GameSaveMetadata),
                    typeof(GameSaveMetadataExample),
                    typeof(GameSaveDataExample),
                    typeof(Inventory),
                    typeof(InventoryItem),
                    typeof(Potion),
                    typeof(Equipment),
                },
                // Add all the converters that you want to use
                new JsonConverter[] {
                    new Vector2IntConverter(),
                    new GenericDictionaryConverter()
                },
                new PrivateSetterContractResolver()
            );
        }
    }
}
