using UnityEngine;

namespace CupkekGames.Systems
{
  public abstract class GameSaveDataSO<TSaveData> : ScriptableObject where TSaveData : IGameSaveData
  {
    [SerializeField] public TSaveData Data;
  }
}