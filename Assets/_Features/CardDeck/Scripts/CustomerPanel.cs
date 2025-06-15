using System;
using System.Collections.Generic;
using DG.Tweening;
using Quackery.ChatApp;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery
{
    public class CustomerPanel : MonoBehaviour
    {
        [SerializeField] private Image _portrait;
        [SerializeField] private TMPro.TextMeshProUGUI _loginName;
        [SerializeField] private Ease _easeType;
        private ChatWindow _chatWindow;

        public bool IsPanelMoving = false;

        public static Dictionary<CustomerPanelState, float> PanelPositions = new()
        {
            { CustomerPanelState.ReadyToEnter, -Screen.width*2},
            {CustomerPanelState.Active, 0},
            {CustomerPanelState.Exited, Screen.width*2}
        };
        private CustomerPanelState _currentState;

        public CustomerPanelState CurrentState => _currentState;

        public CustomerPanelState State { get; private set; } = CustomerPanelState.ReadyToEnter;

        private RectTransform _rectTransform => transform as RectTransform;
        void Awake()
        {
            _chatWindow = GetComponentInChildren<ChatWindow>(true);
            _chatWindow.enabled = false;
        }

        public void SetCustomerInfo(Sprite portrait, string loginName)
        {
            _portrait.sprite = portrait;
            _loginName.text = loginName;
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
            _rectTransform.DOAnchorPosX(PanelPositions[state], 0.5f)
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
