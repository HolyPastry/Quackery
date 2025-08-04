using System;

using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Quackery
{
    public enum Direction
    {
        Left,
        Right,
        Top,
        Bottom
    }
    public class AnimatedRect : MonoBehaviour
    {

        [SerializeField] private RectTransform _rectTransform;

        public RectTransform RectTransform
        {
            get => _rectTransform;
            set => _rectTransform = value;
        }
        public float Height => _rectTransform.rect.height;

        [Header("Animation Settings")]
        [SerializeField] private float _animationDuration = 0.5f;
        //[SerializeField] private float _scaleFactor = 1.1f;
        [SerializeField] private float _scaleDuration = 0.2f;
        [SerializeField] private Ease _animationEaseIn = Ease.OutBack;
        [SerializeField] private Ease _animationEaseOut = Ease.InBack;
        [SerializeField] private bool _startHidden = true;

        [SerializeField] private Vector2 _overridePosition = Vector2.zero;
        [SerializeField] private bool _overridePositionEnabled = false;

        bool _isAnimating = false;
        private Action _callback = delegate { };
        private TweenerCore<Vector3, Vector3, VectorOptions> _inprogressZoomTween;

        void Awake()
        {
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();

            if (_startHidden)
                _rectTransform.gameObject.SetActive(false);
        }
        private Vector2 DirectionVector(Direction direction)
        {
            return direction switch
            {
                Direction.Left => new Vector2(-Screen.width * 2, 0),
                Direction.Right => new Vector2(Screen.width * 2, Screen.height / 2),
                Direction.Top => new Vector2(Screen.width / 2, -Screen.height - Height),
                Direction.Bottom => new Vector2(0, -Screen.height * 2),
                _ => Vector2.zero
            };
        }

        public AnimatedRect SlideIn(Direction from)
        {
            _isAnimating = false;

            // if (_overridePositionEnabled)
            //     Teleport(_overridePosition);
            // else
            TeleportOffscreen(from);
            // TeleportTo(DirectionVector(from));

            _rectTransform.localScale = Vector3.one;
            _rectTransform.gameObject.SetActive(true);

            //return this;
            return SlideToZero();
        }

        public AnimatedRect FloatUp(float distance, float duration, Action callback = null)
        {
            _isAnimating = true;
            _rectTransform.gameObject.SetActive(true);
            _rectTransform.DOLocalMoveY(_rectTransform.localPosition.y + distance, duration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    callback?.Invoke();
                    EndAnimation();
                    _rectTransform.gameObject.SetActive(false);
                    _rectTransform.localPosition = new Vector3(_rectTransform.localPosition.x, _rectTransform.localPosition.y - distance, _rectTransform.localPosition.z);
                });
            return this;
        }

        private void EndAnimation()
        {
            _callback();
            _callback = delegate { };
            _isAnimating = false;
        }

        public AnimatedRect DoComplete(Action callback)
        {
            _callback = callback;
            return this;
        }

        private AnimatedRect Teleport(Direction to)
        {
            TeleportTo(DirectionVector(to));
            return this;
        }

        public AnimatedRect SlideOut(Direction to, bool instant = false)
        {
            if (instant)
            {
                Teleport(to);
                _rectTransform.gameObject.SetActive(false);
                EndAnimation();
                return this;
            }
            _isAnimating = true;
            var targetPosition = _overridePositionEnabled ? _overridePosition : DirectionVector(to);
            _inprogressZoomTween.Kill();
            _rectTransform
               .DOAnchorPos(targetPosition, _animationDuration)
               .SetEase(_animationEaseOut)
               .OnComplete(() =>
               {
                   _rectTransform.gameObject.SetActive(false);
                   EndAnimation();
               });
            return this;
        }

        public AnimatedRect ZoomIn()
        {
            _isAnimating = true;
            _rectTransform.gameObject.SetActive(true);
            _inprogressZoomTween.Kill();
            _inprogressZoomTween = _rectTransform.transform.DOScale(Vector3.one, _animationDuration).SetEase(_animationEaseIn)
                .OnComplete(() =>
                {
                    EndAnimation();
                });
            return this;
        }
        public AnimatedRect ZoomOut(bool instant)
        {
            if (instant)
            {
                _rectTransform.transform.localScale = Vector3.zero;
                _rectTransform.gameObject.SetActive(false);
                EndAnimation();
                return this;
            }
            _isAnimating = true;
            _inprogressZoomTween.Kill();
            _inprogressZoomTween = _rectTransform.transform.DOScale(Vector3.zero, _animationDuration).SetEase(_animationEaseOut)
                 .OnComplete(() =>
                 {
                     _rectTransform.gameObject.SetActive(false);
                     EndAnimation();
                 });
            return this;
        }

        internal WaitUntil WaitForAnimation()
        {
            return new WaitUntil(() => !_isAnimating);
        }

        internal AnimatedRect SlideIn(Vector2 position)
        {
            _overridePosition = position;
            _overridePositionEnabled = true;
            return SlideIn(Direction.Right);
        }

        internal void Punch()
        {

            _isAnimating = true;
            _rectTransform.DOPunchScale(Vector3.one * 1.1f, _scaleDuration, 10, 0.1f).OnComplete(() =>
            {
                EndAnimation();
            });

        }

        internal void TeleportTo(Vector3 origin)
        {

            _rectTransform.localPosition = _rectTransform.InverseTransformPoint(origin);
            _isAnimating = false;
        }

        internal AnimatedRect SlideToZero()
        {
            _isAnimating = true;
            _rectTransform.DOLocalMove(Vector3.zero, _animationDuration)
                .SetEase(_animationEaseIn)
                .OnComplete(() =>
                {
                    EndAnimation();
                });
            return this;
        }

        internal void Show()
        {
            _rectTransform.gameObject.SetActive(true);
        }

        internal void Hide()
        {
            _rectTransform.gameObject.SetActive(false);
        }

        internal void SlideToTop(int yOffset)
        {

            _isAnimating = true;
            var offset = (Screen.height / 2) + yOffset;

            _rectTransform.DOAnchorPosY(offset, _animationDuration)
                .SetEase(_animationEaseIn)
                .OnComplete(() =>
                {
                    EndAnimation();
                });
        }

        internal void TeleportToMiddle(int yOffset)
        {
            // _rectTransform.anchorMin = new Vector2(0, 0.5f);
            // _rectTransform.anchorMax = new Vector2(1, 0.5f);
            _rectTransform.anchoredPosition = new Vector2(0, yOffset);
        }


        public void TeleportOffscreen(Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    _rectTransform.anchoredPosition = new Vector2(-Screen.width, 0);
                    break;
                case Direction.Right:
                    _rectTransform.anchoredPosition = new Vector2(Screen.width, 0);
                    break;
                case Direction.Top:
                    _rectTransform.anchoredPosition = new Vector2(0, Screen.height);
                    break;
                case Direction.Bottom:
                    _rectTransform.anchoredPosition = new Vector2(0, -Screen.height);
                    break;
            }
        }
    }
}
