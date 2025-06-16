using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna
{
    [RequireComponent(typeof(AudioSource))]
    public class UIInteractableAudioHandler : MonoBehaviour
    {
        [Header("Audio Clips")]
        [SerializeField] private AudioClip _click;
        public AudioClip ButtonClick => _click;
        [SerializeField] private AudioClip _hover;
        public AudioClip ButtonHover => _hover;

        [Header("Audio Cooldown (seconds)")]
        [SerializeField] private float hoverCooldown = 0.05f;
        [SerializeField] private float clickCooldown = 0.05f;

        // Cooldown trackers
        private float _lastHoverTime;
        private float _lastClickTime;

        // Properties
        private Dictionary<VisualElement, Manipulator> _manipulators = new Dictionary<VisualElement, Manipulator>();
        // References
        private AudioSource _audioSource;
        // State
        public bool DisableAudio = false;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void AddManipulator(params VisualElement[] elements)
        {
            foreach (VisualElement element in elements)
            {
                AddManipulator(element);
            }
        }

        private void AddManipulator(VisualElement element)
        {
            if (_manipulators.ContainsKey(element))
            {
                return;
            }

            element.AddManipulator(new AudioOnHoverAndClickManipulator(this));
        }

        public void RemoveManipulator(params VisualElement[] elements)
        {
            foreach (VisualElement element in elements)
            {
                if (_manipulators.ContainsKey(element))
                {
                    element.RemoveManipulator(_manipulators[element]);
                    _manipulators.Remove(element);
                }
            }
        }

        public void PlayHover()
        {
            if (DisableAudio || _audioSource == null)
            {
                return;
            }

            if (Time.time >= _lastHoverTime + hoverCooldown)
            {
                _audioSource.PlayOneShot(_hover);
                _lastHoverTime = Time.time;
            }
        }

        public void PlayClick()
        {
            if (DisableAudio || _audioSource == null)
            {
                return;
            }

            if (Time.time >= _lastClickTime + clickCooldown)
            {
                _audioSource.PlayOneShot(_click);
                _lastClickTime = Time.time;
            }
        }
    }
}
