using System;
using System.Collections;
using System.Collections.Generic;
using Bakery.Dialogs;
using KBCore.Refs;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery
{
    public class SpeechBubble : ValidatedMonoBehaviour
    {

        public CharacterData SpeakerData;
        [SerializeField, Child] private TMPro.TextMeshProUGUI _text;
        [SerializeField, Self] private AnimatedRect _panel;
        [SerializeField, Self] private AudioSource _typingAudioSource;

        [SerializeField] private float _timeOnScreen = 4f; // Default time on screen if not specified in dialog

        private float _duration;


        void Start()
        {
            DialogEvents.OnStoryNextLine += OnStoryNextLine;
            gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            DialogEvents.OnStoryNextLine -= OnStoryNextLine;
        }

        public void SetText(string text)
        {
            _text.text = text;
            LayoutRebuilder.ForceRebuildLayoutImmediate(_panel.RectTransform);
            // _panel.gameObject.SetActive(true);
            // var layout = _panel.GetComponent<LayoutGroup>();
            // layout.enabled = false;
            // layout.enabled = true;
            _panel.ZoomIn();
        }
        public void Hide()
        {
            StopAllCoroutines();
            _panel.ZoomOut(false);
            _text.text = "";
        }

        private void OnStoryNextLine(CharacterData data, string text, List<string> list, float duration)
        {
            if (data != SpeakerData) return;
            StopAllCoroutines();
            _panel.ZoomIn();
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
            yield return new WaitForSeconds(_timeOnScreen);
            _panel.ZoomOut(false);
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
    }
}
