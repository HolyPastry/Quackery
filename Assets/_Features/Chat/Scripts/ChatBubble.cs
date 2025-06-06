using System;
using System.Collections;
using KBCore.Refs;
using UnityEngine;

namespace Quackery.ChatApp
{
    internal class ChatBubble : ValidatedMonoBehaviour
    {
        [SerializeField, Child] private TMPro.TextMeshProUGUI _text;
        [SerializeField] private RectTransform _panel;

        private AudioSource _typingAudioSource;
        public RectTransform Panel => _panel;
        private float _duration;

        public object PanelTransform { get; internal set; }

        void Awake()
        {
            _typingAudioSource = GetComponent<AudioSource>();
        }

        public void SetText(string text, float duration)
        {
            _text.text = "";
            StartCoroutine(TypeTextRoutine(text, duration));
        }

        private IEnumerator TypeTextRoutine(string text, float duration)
        {
            _duration = duration;
            if (duration <= 0f)
            {
                _text.text += text;
                yield break;
            }
            PlayTypingSound();
            var charArray = text.ToCharArray();
            for (int i = 0; i < charArray.Length; i++)
            {
                _text.text += charArray[i];
                yield return new WaitForSeconds(_duration / charArray.Length);
            }
            StopPlayingTypingSound();
        }

        private void StopPlayingTypingSound()
        {
            if (_typingAudioSource != null && _typingAudioSource.isPlaying)
            {
                _typingAudioSource.Stop();
            }
        }

        private void PlayTypingSound()
        {
            if (_typingAudioSource != null && !_typingAudioSource.isPlaying)
            {
                _typingAudioSource.Play();
            }
        }

        public void AddText(string text, float duration = 0f)
        {
            if (_text.text.Length > 0)
                _text.text += "\n";
            StartCoroutine(TypeTextRoutine(text, duration));
        }

        internal void FinishTyping()
        {
            _duration = 0f;
        }
    }

}
