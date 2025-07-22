
using System;
using System.Collections;
using Quackery.Shops;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Quackery
{
    public class ShopScrollRect : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private float _dragThreshold = 10f;
        [SerializeField] private float _scrollSpeed = 1000f;

        [SerializeField] private float _snapValue = 100f;

        [SerializeField] private RectTransform _content;

        private float _stablePositionY = 0;
        private float _dragAmount;
        private bool _isScrolling;
        private bool _locked;
        private bool _scrollLocked;

        public float DragAmount
        {
            get => _dragAmount;
            private set
            {
                if (_locked) return;
                if (!IsMoving && (value != 0 || _dragAmount != value || _isScrolling))
                {
                    IsMoving = true;
                    OnMovingStarted?.Invoke();
                }
                if (IsMoving && (value == 0 || _dragAmount == value) && !_isScrolling)
                {
                    IsMoving = false;
                    OnMovingEnded?.Invoke();
                }
                if (_isScrolling)
                    return; // Ignore drag while scrolling
                _dragAmount = value;
                if (_dragAmount > _dragThreshold)
                {
                    _dragAmount = 0f;
                    if (!_isScrolling && !_scrollLocked)
                        StartCoroutine(ScrollDownRoutine());
                }
            }
        }

        public bool IsMoving { get; private set; }

        public event Action OnMovingStarted = delegate { };
        public event Action OnMovingEnded = delegate { };

        public event Action<int> OnMoveScreen = delegate { };

        public void OnBeginDrag(PointerEventData eventData) => DragAmount = 0f;


        public void OnDrag(PointerEventData eventData) => DragAmount += eventData.delta.y;


        public void OnEndDrag(PointerEventData eventData) => DragAmount = 0f;

        internal void LockMovement() => _locked = true;

        internal void UnlockMovement() => _locked = false;

        public void LockScrolling() => _scrollLocked = true;
        public void UnlockScrolling() => _scrollLocked = false;


        void Start()
        {
            OnMoveScreen(0);
        }

        private IEnumerator ScrollDownRoutine()
        {
            _isScrolling = true;
            float targetPosition = _stablePositionY + Screen.height;
            while (_stablePositionY < targetPosition)
            {
                _stablePositionY = Mathf.Lerp(_stablePositionY, targetPosition, Time.deltaTime * _scrollSpeed);
                if (_stablePositionY > targetPosition - _snapValue)
                    _stablePositionY = targetPosition;
                yield return null;
            }
            OnMoveScreen?.Invoke((int)(_stablePositionY / Screen.height));
            _isScrolling = false;
            DragAmount = 0f;
        }

        void Update()
        {
            _content.anchoredPosition = new Vector2(0, _stablePositionY + DragAmount);
        }
    }
}
