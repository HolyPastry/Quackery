using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Quackery
{

    public class RotatingClientChatPanel : ClientChatPanel
    {

        [SerializeField] private Ease _easeType;
        [SerializeField] private float _transitionDuration = 1f;
        [SerializeField] private CustomerPanelState _currentState;

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
