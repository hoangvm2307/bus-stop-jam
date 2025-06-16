using System;
using CupkekGames.Core.Pool;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna
{
    public class TooltipItemPool : ObjectPoolBase<TooltipItem>
    {
        private MonoBehaviour _coroutineRunner;
        private VisualElement _container;
        private string _ussClassNameItem;
        private string _elementNameItem;
        private float _fadeDuration = 0.5f;
        private EasingMode _easingMode = EasingMode.EaseOutCirc;
        public TooltipItemPool(
          MonoBehaviour coroutineRunner,
          VisualElement container,
          string ussClassNameItem,
          string elementNameItem,
          float fadeDuration = 0.5f,
          EasingMode easingMode = EasingMode.EaseOutCirc,
          int defaultCapacity = 5, 
          int maxCapacity = 20, 
          bool prewarm = true, 
          bool collectionCheck = true
        ) : base(
          defaultCapacity, 
          maxCapacity, 
          collectionCheck
        )
        {
          _coroutineRunner = coroutineRunner;
          _container = container;
          _ussClassNameItem = ussClassNameItem;
          _elementNameItem = elementNameItem;
          _fadeDuration = fadeDuration;
          _easingMode = easingMode;

          if (prewarm)
          {
            Prewarm();

            // Debug.Log("Created pool for " + typeof(TooltipItem) + " with " + Pool.CountAll + " objects");
          }
        }

        public override TooltipItem CreatePooledObject()
        {
          // Debug.Log("Pool: Creating tooltip item");
          var tooltipItem = new TooltipItem(_coroutineRunner, this, _container, _ussClassNameItem, _elementNameItem, _fadeDuration, _easingMode);
          return tooltipItem;
        }

        public override void OnDestroyObject(TooltipItem Instance)
        {
            // Debug.Log("Pool: Destroying tooltip item");
            Instance.OnPoolDestroy();
        }

        public override void OnReturnToPool(TooltipItem Instance)
        {
            // Debug.Log("Pool: Returning tooltip item to pool");
        }

        public override void OnTakeFromPool(TooltipItem Instance)
        {
            // Debug.Log("Pool: Taking tooltip item from pool");
            Instance.SetActive(true);
        }
    }
}
