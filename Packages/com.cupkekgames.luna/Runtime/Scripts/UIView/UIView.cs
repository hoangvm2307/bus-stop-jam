using System.Collections;
using System.Collections.Generic;
using CupkekGames.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna
{
    public class UIView
    {
        private GameObject _parent;
        public GameObject Parent => _parent;
        private LunaUIManager _lunaUIManager;
        public LunaUIManager LunaUIManager => _lunaUIManager;
        private VisualElement _parentElement;
        public VisualElement ParentElement => _parentElement;
        private VisualElement _focusElement;
        private FadeUIElement _fade;
        public FadeUIElement Fade => _fade;
        private List<UIView> _children = new();
        public bool DisableOtherViewsOnFadeIn = true;
        private bool _debug;
        // State
        private List<IUIViewAction> _actionList = new();
        private UIViewReference _reference;
        private bool _isVisible;
        public bool IsVisible => _isVisible;
        private HashSet<VisualElement> _disabledElements = null;

        public UIView(GameObject parent, VisualElement parentElement,
            UIStartVisibility startVisibility = UIStartVisibility.Visible, VisualElement focusElement = null, float fadeDuration = 0.5f,
            EasingMode easingMode = EasingMode.EaseOutCirc, bool disableOtherViewsOnFadeIn = false, bool debug = false)
        {
            _parent = parent;
            DisableOtherViewsOnFadeIn = disableOtherViewsOnFadeIn;
            _debug = debug;
            _lunaUIManager = LunaUIManager.Instance;

            if (!parent.TryGetComponent<UIViewReference>(out _reference))
            {
                _reference = parent.AddComponent<UIViewReference>();
            }
            _reference.List.Add(this);

            _parentElement = parentElement;
            _focusElement = focusElement;

            _fade = new FadeUIElement(_reference, _parentElement, easingMode, _debug);

            _fade.SetDuration(0);

            if (startVisibility == UIStartVisibility.Visible)
            {
                _isVisible = false;
                OnFadeInStart();
                _fade.FadeIn();
            }
            else if (startVisibility == UIStartVisibility.Invisible)
            {
                _isVisible = true;
                OnFadeOut();
                _fade.FadeOut();
            }
            else if (startVisibility == UIStartVisibility.FadeIn)
            {
                _fade.FadeOut();
            }
            else if (startVisibility == UIStartVisibility.FadeOut)
            {
                _fade.FadeIn();
            }

            _fade.SetDuration(fadeDuration);
            Fade.OnFadeInStart += OnFadeInStart;
            Fade.OnFadeIn += OnFadeIn;
            Fade.OnFadeOut += OnFadeOut;

            if (startVisibility == UIStartVisibility.FadeIn)
            {
                _isVisible = false;
                // _parentElement.schedule.Execute(() => _fade.FadeIn()).StartingIn(0);
                _reference.StartCoroutine(DelayedFadeIn());
            }
            else if (startVisibility == UIStartVisibility.FadeOut)
            {
                _isVisible = true;
                // _parentElement.schedule.Execute(() => _fade.FadeOut()).StartingIn(0);
                _reference.StartCoroutine(DelayedFadeOut());
            }
        }

        private IEnumerator DelayedFadeIn()
        {
            yield return null;

            _fade.FadeIn();
        }

        private IEnumerator DelayedFadeOut()
        {
            yield return null;

            _fade.FadeOut();
        }

        protected virtual void OnFadeInStart()
        {
            if (_debug)
            {
                Debug.Log("UIView Fade In " + _parentElement.name);
            }
            OnEnable();
        }

        protected virtual void OnFadeIn()
        {
            _focusElement?.Focus();

            foreach (UIView child in _children)
            {
                if (child.IsVisible)
                {
                    child.OnFadeIn();
                }
            }
        }

        protected virtual void OnFadeOut()
        {
            if (_debug)
            {
                Debug.Log("UIView Fade Out " + _parentElement.name);
            }
            OnDisable();
        }

        /// <summary>
        /// Adds a new action to the view’s action list. 
        /// If the view is currently faded in, the action’s OnFadeIn method will be triggered immediately.
        /// The action will be automatically enabled or disabled in sync with the view's fade-in or fade-out state.
        /// </summary>
        /// <param name="action">Action to add</param>    
        public void AddAction(IUIViewAction action, bool activate = true)
        {
            _actionList.Add(action);

            if (activate && _isVisible)
            {
                action.OnFadeInStart();
            }
        }

        internal void OnEnable(bool skipCheck = false)
        {
            if (!skipCheck && _isVisible)
            {
                return;
            }

            if (_debug)
            {
                Debug.Log("OnEnable: " + _parentElement.name);
            }

            _lunaUIManager?.RegisterPage(ParentElement);

            foreach (IUIViewAction action in _actionList)
            {
                action.OnFadeInStart();
            }

            if (DisableOtherViewsOnFadeIn)
            {
                _disabledElements = _lunaUIManager?.SetEnabledAllPages(false, ParentElement);
            }

            _isVisible = true;

            HandleChildrenOnFadeIn();
        }

        internal void OnDisable(bool skipCheck = false)
        {
            if (!skipCheck && !_isVisible)
            {
                return;
            }

            if (_debug)
            {
                Debug.Log("OnDisable: " + _parentElement.name);
            }

            _lunaUIManager?.UnregisterPage(ParentElement);

            foreach (IUIViewAction action in _actionList)
            {
                action.OnFadeOut();
            }

            if (_disabledElements != null)
            {
                _lunaUIManager?.SetEnabledElements(true, _disabledElements);
                _disabledElements = null;
            }

            _isVisible = false;

            HandleChildrenOnFadeOut();
        }

        public void AddChild(UIView child)
        {
            _children.Add(child);
        }

        private void HandleChildrenOnFadeOut()
        {
            foreach (UIView child in _children)
            {
                if (!child.IsVisible)
                {
                    child.OnDisable(true);
                }
            }
        }
        private void HandleChildrenOnFadeIn()
        {
            foreach (UIView child in _children)
            {
                if (child.IsVisible)
                {
                    child.OnEnable(true);
                }
            }
        }
    }
}