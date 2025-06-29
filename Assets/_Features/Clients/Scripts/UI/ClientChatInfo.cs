using System;
using System.Collections.Generic;
using DG.Tweening;
using Quackery.Chats;
using Quackery.Clients;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery
{
    public class ClientChatInfo : MonoBehaviour
    {
        [SerializeField] private Image _portrait;
        [SerializeField] private TMPro.TextMeshProUGUI _loginName;
        [SerializeField] private Ease _easeType;
        [SerializeField] private float _transitionDuration = 1f;
        [SerializeField] private CustomerPanelState _currentState;
        [SerializeField] private ChatWindow _chatWindow;

        public bool IsPanelMoving = false;

        public static Dictionary<CustomerPanelState, float> PanelPositions = new()
        {
            { CustomerPanelState.ReadyToEnter, -Screen.width*2},
            {CustomerPanelState.Active, 0},
            {CustomerPanelState.Exited, Screen.width*2}
        };


        public CustomerPanelState CurrentState => _currentState;

        public CustomerPanelState State { get; private set; } = CustomerPanelState.ReadyToEnter;

        private RectTransform _rectTransform => transform as RectTransform;

        public float BottomMargin
        {
            set
            {
                // _chatWindow.BottomMargin = value;
            }
        }

        public Action OnBackButtonPressed = delegate { };

        void Awake()
        {

            _chatWindow.enabled = false;
        }

        public void SetClientInfo(Client client)
        {
            _portrait.sprite = client.Portrait;
            _loginName.text = client.LoginName;
            _chatWindow.SetChatHistory(client.ChatHistory);
        }
        public void EnableChat() => _chatWindow.enabled = true;
        public void DisableChat()
        {
            _chatWindow.ClearChat();
            _chatWindow.enabled = false;
        }

        public void TeleportToPosition(CustomerPanelState state)
        {
            _currentState = state;
            _rectTransform.anchoredPosition =
                new Vector2(PanelPositions[state],
                _rectTransform.anchoredPosition.y);
            if (state == CustomerPanelState.Active)
                EnableChat();
            else
                DisableChat();
        }

        public void SlideToPosition(CustomerPanelState state)
        {
            if (_currentState == state)
                return;
            IsPanelMoving = true;
            _rectTransform.DOAnchorPosX(PanelPositions[state], _transitionDuration)
                .SetEase(_easeType)
                .OnComplete(() =>
                {
                    _currentState = state;
                    if (state == CustomerPanelState.Active)
                        EnableChat();
                    else
                        DisableChat();
                    IsPanelMoving = false;
                });
        }
    }
}
