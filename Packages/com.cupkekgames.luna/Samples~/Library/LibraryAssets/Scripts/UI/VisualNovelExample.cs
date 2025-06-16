using System;
using System.Collections;
using System.Collections.Generic;
using CupkekGames.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna.Library
{
    public class VisualNovelExample : MonoBehaviour
    {
        private VisualNovelController _controller;
        [SerializeField] private List<string> _dialogue = new();
        [SerializeField] private float _skipInterval = .1f;
        [SerializeField] private string[] _charNames;
        [SerializeField] private Sprite[] _avatars;

        private int _nextIndex = 0;
        private Coroutine _skipCoroutine;

        private void Awake()
        {
            _controller = GetComponent<VisualNovelController>();
        }

        private void OnEnable()
        {
            _controller.OnContinue += Continue;
            _controller.OnRestart += Restart;
            _controller.OnSkip += ToggleSkip;

            _controller.OnTextStart += TextStart;
            _controller.OnTextComplete += TextComplete;
        }

        private void OnDisable()
        {
            _controller.OnContinue -= Continue;
            _controller.OnRestart -= Restart;
            _controller.OnSkip -= ToggleSkip;

            _controller.OnTextStart -= TextStart;
            _controller.OnTextComplete -= TextComplete;
        }

        private void Start()
        {
            // Start is called after Awake and OnEnable of the _controller
            Restart();
        }

        public void Continue()
        {
            if (_nextIndex >= _dialogue.Count)
            {
                _controller.EndCurrent();
                return;
            }

            int charIndex = _nextIndex % 2;
            Sprite avatarLeft = charIndex == 0 ? _avatars[0] : null;
            Sprite avatarRight = charIndex == 1 ? _avatars[1] : null;

            if (_controller.Continue(_dialogue[_nextIndex], _charNames[charIndex], avatarLeft, avatarRight, false))
            {
                _nextIndex++;
            }
        }

        private void ToggleSkip()
        {
            if (_skipCoroutine != null)
            {
                StopCoroutine(_skipCoroutine);
                _skipCoroutine = null;
            }
            else
            {
                _skipCoroutine = StartCoroutine(SkipDialogueCoroutine());
            }
        }

        private IEnumerator SkipDialogueCoroutine()
        {
            while (_nextIndex < _dialogue.Count)
            {
                Continue();

                yield return new WaitForSeconds(_skipInterval);
            }
        }

        public void Restart()
        {
            _nextIndex = 0;
            if (_controller.Continue(_dialogue[0], _charNames[0], _avatars[0], null, true))
            {
                _nextIndex++;
            }
        }

        private void TextComplete()
        {
            if (HasNext())
            {
                _controller.ShowNext();
            }
            else
            {
                // _controller.HideNext();
                _controller.ShowNext(); // Next input will close the modal so we prefer to show this for this case

                _nextIndex++;

                if (_nextIndex == _dialogue.Count + 2)
                {
                    Destroy(gameObject);
                }
            }
        }

        private void TextStart()
        {
            _controller.HideNext();
        }

        public bool HasNext()
        {
            return _nextIndex < _dialogue.Count;
        }
    }
}