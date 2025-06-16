using UnityEngine;

namespace CupkekGames.Core
{
  public abstract class KeyValueDatabaseMonoSO<TKey, TValue> : KeyValueDatabaseMono<TKey, TValue> where TValue : ScriptableObject
  {
    [SerializeField] private FolderReference _searchFolder;
  }
}