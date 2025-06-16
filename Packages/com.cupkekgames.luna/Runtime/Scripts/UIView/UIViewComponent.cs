using System.Collections;
using CupkekGames.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna
{
    public class UIViewComponent : MonoBehaviour
    {
        // UI Panel Base
        public UIView UIView;
        public LunaUIManager LunaUIManager => UIView.LunaUIManager;
        public VisualElement ParentElement => UIView.ParentElement;
        public FadeUIElement Fade => UIView.Fade;
        // Properties
        [MultiLineHeader("UIDocument\nUses this gameobject's UIDocument if not set.")]
        [SerializeField] private UIDocument _uiDocument;
        public UIDocument UIDocument => _uiDocument;
        [Header("Leave empty for root element")]
        [SerializeField] protected string _parentName = "";
        [Header("Element to focus when this UIView becomes visible")]
        [SerializeField] protected string _focusName = "";
        // Settings
        [SerializeField] protected float _fadeDuration = 0.5f;
        [SerializeField] protected UIStartVisibility _startVisibility = UIStartVisibility.FadeIn;
        [SerializeField] protected EasingMode _easingMode = EasingMode.EaseOutCirc;
        [MultiLineHeader("Adds UIViewActionEscape that calls FadeOutThenDestroy.")]
        [SerializeField] private bool _addDefaultEscapeAction = false;
        [MultiLineHeader("Disable the interactable elements on other views whenever this view becomes visible.\n" +
                         "This is particularly useful for UI navigation with keys when opening a modal or screen on top of another view.\n" +
                         "This will prevent the auto navigation from interacting with the elements behind this view.")]
        [SerializeField] private bool _disableOtherViewsOnFadeIn = true;
        [SerializeField] private bool _debug = false;

        protected virtual void Awake()
        {
            if (_uiDocument == null)
            {
                _uiDocument = GetComponent<UIDocument>();
            }

            if (UIView == null)
            {
                VisualElement parent = string.IsNullOrEmpty(_parentName) ? _uiDocument.rootVisualElement : _uiDocument.rootVisualElement.Q<VisualElement>(_parentName);

                VisualElement focus = string.IsNullOrEmpty(_focusName) ? _uiDocument.rootVisualElement : _uiDocument.rootVisualElement.Q<VisualElement>(_focusName);

                UIView = new UIView(gameObject, parent, _startVisibility, focus, _fadeDuration, _easingMode, _disableOtherViewsOnFadeIn, _debug);
            }

            if (_addDefaultEscapeAction)
            {
                UIView.AddAction(new UIViewActionEscape(FadeOutThenDestroy));
            }
        }

        public void FadeOutThenDestroy()
        {
            Fade.OnFadeOut += ThenDestroy;

            Fade.FadeOut();
        }

        private void ThenDestroy()
        {
            Fade.OnFadeOut -= ThenDestroy;

            Destroy(gameObject);
        }
    }
}