using System;
using System.Collections;
using System.Collections.Generic;
using Bakery.Dialogs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Quackery.Chats
{

    public class ChatWindow : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private ChatBubble _meChatBubblePrefab;
        [SerializeField] private ChatBubble _otherChatBubblePrefab;
        [SerializeField] private ChoiceButton _choiceButtonPrefab;
        [SerializeField] private TypingSign _typingSign;

        [SerializeField] CharacterData _meCharacterData;
        [SerializeField] private Transform _chatContent;

        [SerializeField] private AudioSource _notificationAudioSource;

        private VerticalLayoutGroup _chatContentLayoutGroup;

        private CharacterData _lastSpeaker;
        private ChatBubble _lastBubble;

        private readonly List<ChoiceButton> _choiceButtons = new();

        public float BottomMargin
        {
            set
            {
                var rectTransform = transform as RectTransform;
                rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, value);
            }
        }

        void Awake()
        {
            _chatContentLayoutGroup = _chatContent.GetComponent<VerticalLayoutGroup>();
            _typingSign.gameObject.SetActive(false);
        }

        void OnEnable()
        {
            DialogEvents.OnStoryNextLine += OnStoryNextLine;
            DialogEvents.OnChoiceAvailable += OnChoiceAvailable;

        }

        void OnDisable()
        {
            DialogEvents.OnStoryNextLine -= OnStoryNextLine;
            DialogEvents.OnChoiceAvailable -= OnChoiceAvailable;
        }

        private void OnChoiceAvailable(List<DialogChoice> choices)
        {
            if (_lastSpeaker != _meCharacterData)
            {
                _lastBubble = Instantiate(_meChatBubblePrefab, _chatContent);
                _lastBubble.SetText("", 0f);
                _lastSpeaker = _meCharacterData;
            }

            for (int i = 0; i < choices.Count; i++)
            {
                var choiceButton = Instantiate(_choiceButtonPrefab, _lastBubble.Panel);
                _choiceButtons.Add(choiceButton);
                choiceButton.SetChoice(choices[i], i);
                choiceButton.OnChoiceSelected += MakeChoice;
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(_lastBubble.transform as RectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_chatContent as RectTransform);
        }


        private void MakeChoice(int chosenIndex)
        {
            DialogServices.MakeChoice(chosenIndex);
            while (_choiceButtons.Count > 0)
            {
                var choiceButton = _choiceButtons[0];
                _choiceButtons.RemoveAt(0);
                choiceButton.OnChoiceSelected -= MakeChoice;
                Destroy(choiceButton.gameObject);
            }
        }

        private void OnStoryNextLine(CharacterData data, string line, List<string> tags, float duration)
        {
            StartCoroutine(OnStoryNextLineRoutine(data, line, tags, duration));

        }

        private IEnumerator OnStoryNextLineRoutine(CharacterData data, string line, List<string> tags, float duration)
        {

            var isMe = data == _meCharacterData;
            var bubblePrefab = isMe ? _meChatBubblePrefab : _otherChatBubblePrefab;
            float delay = 0;
            if (!isMe)
            {
                _typingSign.transform.SetAsLastSibling();
                _typingSign.gameObject.SetActive(true);
                LayoutRebuilder.ForceRebuildLayoutImmediate(_chatContent as RectTransform);
                delay = duration;
                yield return new WaitForSeconds(delay);
                _typingSign.gameObject.SetActive(false);
                PlayNotification();
            }
            duration -= delay;

            if (_lastSpeaker == data)
            {
                _lastBubble.AddText(line, duration);
            }
            else
            {
                _lastBubble = Instantiate(bubblePrefab, _chatContent);
                _lastSpeaker = data;
                _lastBubble.SetText(line, duration);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(_lastBubble.transform as RectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_chatContent as RectTransform);
        }

        private void PlayNotification()
        {
            if (_notificationAudioSource != null && !_notificationAudioSource.isPlaying)
            {
                _notificationAudioSource.Play();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_typingSign.gameObject.activeSelf) return;

            if (_lastBubble != null)
                _lastBubble.FinishTyping();

            if (DialogServices.IsDialogInProgress())
                DialogServices.SkipOneLine();

        }

        internal void ClearChat()
        {
            _lastSpeaker = null;
            _lastBubble = null;

            foreach (Transform child in _chatContent)
            {
                if (child.TryGetComponent<ChatBubble>(out var chatBubble))
                {
                    Destroy(chatBubble.gameObject);
                }
            }

            while (_choiceButtons.Count > 0)
            {
                var choiceButton = _choiceButtons[0];
                _choiceButtons.RemoveAt(0);
                choiceButton.OnChoiceSelected -= MakeChoice;
                Destroy(choiceButton.gameObject);
            }

            _typingSign.gameObject.SetActive(false);
        }

        internal void SetChatHistory(string chatHistory)
        {
            //noop
        }
    }
}
