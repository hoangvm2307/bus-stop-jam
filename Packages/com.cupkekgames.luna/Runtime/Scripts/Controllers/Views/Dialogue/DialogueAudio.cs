using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using CupkekGames.Core;

namespace CupkekGames.Luna
{
    [RequireComponent(typeof(AudioSource))]
    public class DialogueAudio : MonoBehaviour
    {
        // UI Elements
        // Audio
        private AudioSource _audioSource;
        [SerializeField] private AudioClip _letter;
        [SerializeField] private AudioClip _pause;
        protected virtual void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void PlayLetter()
        {
            _audioSource.PlayOneShot(_letter);
        }

        public void PlayPause()
        {
            _audioSource.PlayOneShot(_pause);
        }
    }
}