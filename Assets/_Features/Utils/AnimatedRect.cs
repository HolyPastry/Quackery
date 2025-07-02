using System;

using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

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

        [Header("Animation Settings")]
        [SerializeField] private float _animationDuration = 0.5f;
        [SerializeField] private float _scaleFactor = 1.1f;
        [SerializeField] private float _scaleDuration = 0.2f;
        [SerializeField] private Ease _animationEaseIn = Ease.OutBack;
        [SerializeField] private Ease _animationEaseOut = Ease.InBack;
        [SerializeField] private bool _startHidden = true;

        [SerializeField] private Vector2 _overridePosition = Vector2.zero;
        [SerializeField] private bool _overridePositionEnabled = false;

        bool _isAnimating = false;
        private Action _callback = delegate { };

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
                Direction.Right => new Vector2(Screen.width * 2, 0),
                Direction.Top => new Vector2(0, Screen.height * 2),
                Direction.Bottom => new Vector2(0, -Screen.height * 2),
                _ => Vector2.zero
            };
        }

        public AnimatedRect SlideIn(Direction from)
        {
            _isAnimating = true;

            if (_overridePositionEnabled)
                Teleport(_overridePosition);
            else
                Teleport(from);
            _rectTransform.localScale = Vector3.one;
            _rectTransform.gameObject.SetActive(true);
            _rectTransform
                .DOAnchorPos(Vector2.zero, _animationDuration)
                .SetEase(_animationEaseIn)
                .OnComplete(() =>
                {
                    EndAnimation();
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

        private void Teleport(Vector2 position)
        {
            _rectTransform.position = position;
        }
        private AnimatedRect Teleport(Direction to)
        {
            Teleport(DirectionVector(to));
            return this;
        }

        public AnimatedRect SlideOut(Direction to)
        {
            _isAnimating = true;
            var targetPosition = _overridePositionEnabled ? _overridePosition : DirectionVector(to);
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
            _rectTransform.transform.DOScale(Vector3.one, _animationDuration).SetEase(_animationEaseIn)
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
            _rectTransform.transform.DOScale(Vector3.zero, _animationDuration).SetEase(_animationEaseOut)
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

        internal void ScaleUp()
        {
            _rectTransform.DOScale(_scaleFactor, _scaleDuration);
        }

        internal void ScaleDown()
        {
            _rectTransform.DOScale(Vector3.one, _scaleDuration);
        }

        internal void Punch()
        {

            _isAnimating = true;
            _rectTransform.DOPunchScale(Vector3.one * 1.1f, _scaleDuration, 10, 0.1f).OnComplete(() =>
            {
                EndAnimation();
            });

        }
    }
}
