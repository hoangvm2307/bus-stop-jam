using System;
using System.Collections;
using System.Collections.Generic;
using CupkekGames.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna.Library
{
    public class SpeechBubbleExample : MonoBehaviour
    {
        private SpeechBubbleController _controller;
        [SerializeField] private List<string> _dialogue = new();
        [SerializeField] private Sprite[] _avatars;
        [SerializeField] private Vector2 _position;

        private int _nextIndex = 0;

        private void Awake()
        {
            _controller = GetComponent<SpeechBubbleController>();

            _controller.SetPostion(_position);
        }

        private void OnEnable()
        {
            _controller.OnContinue += Continue;

            _controller.OnTextStart += TextStart;
            _controller.OnTextComplete += TextComplete;
        }

        private void OnDisable()
        {
            _controller.OnContinue -= Continue;

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

            if (_controller.Continue(_dialogue[_nextIndex], avatarLeft, avatarRight, false))
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
        public void Restart()
        {
            _nextIndex = 0;
            if (_controller.Continue(_dialogue[0], _avatars[0], null, true))
            {
                _nextIndex++;
            }
        }
    }
}