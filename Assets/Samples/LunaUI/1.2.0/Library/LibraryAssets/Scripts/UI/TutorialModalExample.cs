using System;
using System.Collections;
using System.Collections.Generic;
using CupkekGames.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna.Library
{
    public class TutorialModalExample : MonoBehaviour
    {
        private TutorialModalController _controller;
        [SerializeField] private List<string> _dialogue = new();
        [SerializeField] private string _title = "Tutorial Title";
        [SerializeField] private List<string> _subtitles;
        [SerializeField] private List<Sprite> _images;
        private int _nextIndex = 1;

        private void Awake()
        {
            _controller = GetComponent<TutorialModalController>();
        }

        private void OnEnable()
        {
            _controller.OnNext += Next;
            _controller.OnPrevious += Previous;

            _controller.OnTextStart += TextStart;
        }

        private void OnDisable()
        {
            _controller.OnNext -= Next;
            _controller.OnPrevious -= Previous;

            _controller.OnTextStart -= TextStart;
        }

        private void Start()
        {
            // Start is called after Awake and OnEnable of the _controller
            Restart();
        }

        public void Next()
        {
            if (_nextIndex >= _dialogue.Count)
            {
                _controller.EndCurrent();
                return;
            }

            if (_controller.Continue(_dialogue[_nextIndex], _title, _subtitles[_nextIndex], _images[_nextIndex], false))
            {
                _nextIndex++;
            }
        }

        public void Previous()
        {
            if (_nextIndex == 1)
            {
                return;
            }

            _nextIndex--;
            _nextIndex--;
            if (_controller.Continue(_dialogue[_nextIndex], _title, _subtitles[_nextIndex], _images[_nextIndex], true))
            {
                _nextIndex++;
            }
        }

        public void Restart()
        {
            _nextIndex = 0;
            if (_controller.Continue(_dialogue[_nextIndex], _title, _subtitles[_nextIndex], _images[_nextIndex], true))
            {
                _nextIndex++;
            }
        }

        private void TextStart()
        {
            if (HasNext())
            {
                _controller.SetEnabledNext(true);
            }
            else
            {
                _controller.SetEnabledNext(false);
            }

            if (HasPrevious())
            {
                _controller.SetEnabledPrevious(true);
            }
            else
            {
                _controller.SetEnabledPrevious(false);
            }
        }

        public bool HasNext()
        {
            return _nextIndex < _dialogue.Count - 1;
        }
        public bool HasPrevious()
        {
            return _nextIndex >= 1;
        }
    }
}