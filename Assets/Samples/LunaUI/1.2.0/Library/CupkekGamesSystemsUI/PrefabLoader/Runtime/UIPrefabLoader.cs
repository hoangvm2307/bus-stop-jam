using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using CupkekGames.Luna;

namespace CupkekGames.Systems.UI
{
  public abstract class UIPrefabLoader<TKey> : PrefabLoader<TKey>
  {
    public void FadeOutDestroy(TKey key, float duration = 0.5f)
    {
      List<GameObject> list = GetInstances(key);

      if (list == null)
      {
        return;
      }

      foreach (GameObject go in list)
      {
        UIDocument uiDocument = go.GetComponent<UIDocument>();

        // fade
        FadeUIElement fadeUIElement = new(this, uiDocument.rootVisualElement);
        fadeUIElement.SetDuration(duration);
        fadeUIElement.FadeOut();
      }

      StartCoroutine(DestroyAllOfWithDelay(key, duration));
    }
  }
}