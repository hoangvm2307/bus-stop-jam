using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using CupkekGames.Core;

namespace CupkekGames.Luna
{
    [RequireComponent(typeof(DialogueAudio))]
    public class DialogueController : UIViewComponent
    {
        // UI Elements
        private Label _speechLabel;
        // Settings
        [SerializeField] private char[] _skipChars = new char[] { ' ' };
        [SerializeField] private char[] _stopChars = new char[] { '.', ',', '!', '?' };
        [SerializeField] private DialogueAnimationMode TextAnimationMode = DialogueAnimationMode.Visibility;
        // [SerializeField] private int _soundEveryChar = 1; // TODO
        // Audio
        private DialogueAudio _audio;
        // State
        private Coroutine _currentCoroutine;
        public bool IsPlaying => _currentCoroutine != null;
        private string _currentText;
        // Events
        public event Action OnTextStart;
        public event Action OnTextComplete;

        protected override void Awake()
        {
            base.Awake();

            _audio = GetComponent<DialogueAudio>();

            _speechLabel = ParentElement.Q<Label>("Speech");
            _speechLabel.text = "";
        }

        private IEnumerator PlayCoroutine(string text)
        {
            OnTextStart?.Invoke();

            _currentText = text;

            if (TextAnimationMode == DialogueAnimationMode.Default || TextAnimationMode == DialogueAnimationMode.Visibility)
            {
                for (int charIndex = 0; charIndex < text.Length; charIndex++)
                {
                    char lastVisibleChar = text[charIndex];

                    while (System.Array.IndexOf(_skipChars, lastVisibleChar) != -1)
                    {
                        charIndex++;
                        if (charIndex == text.Length - 1)
                        {
                            break;
                        }
                        lastVisibleChar = text[charIndex];
                    }

                    if (TextAnimationMode == DialogueAnimationMode.Default)
                    {
                        _speechLabel.text = text[..(charIndex + 1)];
                    }
                    else
                    {
                        _speechLabel.text = BuildPartiallyRevealedString(text, charIndex);
                    }

                    if (_audio != null)
                    {
                        if (System.Array.Exists(_stopChars, e => e == lastVisibleChar))
                        {
                            // _isStopCharacter = true;
                            _audio.PlayPause();
                            yield return new WaitForSeconds(0.1f);
                        }
                        else
                        {
                            _audio.PlayLetter();
                        }
                    }

                    yield return new WaitForSeconds(0.05f);
                }
            }
            else if (TextAnimationMode == DialogueAnimationMode.Instant)
            {
                _speechLabel.text = text;
            }

            _currentCoroutine = null;
            OnTextComplete?.Invoke();
        }

        private string BuildPartiallyRevealedString(string original, int charIndex)
        {
            string revealed = original[..(charIndex + 1)];
            string unrevealed = original[(charIndex + 1)..];

            StringBuilder sb = new StringBuilder();
            sb.Append(revealed);
            sb.Append(RichTextColor.TRANSPARENT);
            sb.Append(unrevealed);
            sb.Append(RichTextColor.CLOSING_TAG);
            return sb.ToString();
        }

        /// <summary>
        // Finish current text animation and show the whole text
        /// </summary>
        public void EndCurrent()
        {
            if (IsPlaying)
            {
                StopCoroutine(_currentCoroutine);
                _currentCoroutine = null;
            }

            _speechLabel.text = _currentText;
            OnTextComplete?.Invoke();
        }

        /// <summary>
        /// Continue takes a text input and a boolean flag to determine whether to skip the current text.
        /// </summary>
        /// <param name="text">Text to play.</param>
        /// <param name="skipCurrent">Only effective if there is a text playing. 
        /// If false it will end the current text and show it fully.
        /// If true it will skip current text and start playing new text immediately.</param>
        /// <returns>true if it started playing the new text</returns>
        public bool Continue(string text, bool skipCurrent)
        {
            if (IsPlaying)
            {
                EndCurrent();
                if (!skipCurrent)
                {
                    return false;
                }
            }

            if (text != null)
            {
                _currentCoroutine = StartCoroutine(PlayCoroutine(text));
            }

            return true;
        }
    }
}
