using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Quackery
{
    public class Animatable : MonoBehaviour
    {
        [SerializeField] private Ease _easeType;

        private RectTransform _rectTransform => transform as RectTransform;

        private event Action Callback = delegate { };

        public void TeleportOut()
        {

            _rectTransform.anchoredPosition =
                new Vector2(Screen.width * 2,
                _rectTransform.anchoredPosition.y);
        }

        public void SlideIn()
        {

            _rectTransform.DOAnchorPosX(0, 0.5f)
                .SetEase(_easeType);
        }

        public Animatable SlideOut()
        {
            _rectTransform.DOAnchorPosX(Screen.width * 2, 0.5f)
                .SetEase(_easeType)
                .OnComplete(() => Callback());
            return this;
        }

        public Animatable OnComplete(Action callback)
        {
            Callback += callback;
            return this;
        }

    }
}
