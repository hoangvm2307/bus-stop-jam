using UnityEngine;

namespace CupkekGames.Luna.Library
{
  public class DontDestroyOnLoad : MonoBehaviour
  {
    private void Awake()
    {
      DontDestroyOnLoad(gameObject);
    }
  }
}
