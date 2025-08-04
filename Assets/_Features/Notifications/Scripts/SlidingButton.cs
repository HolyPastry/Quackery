
using System;
using DG.Tweening;
using Holypastry.Bakery;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Quackery.Notifications
{
    public class SlidingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
    {
        [SerializeField] protected RectTransform _panel;
        [SerializeField] private bool _enableTap = true;
        [SerializeField] private bool _destroyOnFinish = true;
        [SerializeField] private float _screenRatioBeforeSliding = 0.3f;


        [ShowIf("_enableTap")]
        [SerializeField]
        private float _tapTimeOut = 0.5f;
        private bool _grabbed;
        private bool _sliding;
        private bool _grabbable = true;
        private float _velocity;
        private Vector2 _pointerDownPosition;
        private CountdownTimer _tapTimer;

        public event Action OnStartSliding = delegate { };
        public event Action OnFinishSliding = delegate { };

        public event Action OnTapped = delegate { };


        protected virtual void Awake()
        {
            if (_enableTap)
                _tapTimer = new CountdownTimer(_tapTimeOut);
        }

        void OnEnable()
        {
            Reset();
        }

        public void Reset()
        {
            _panel.localScale = Vector3.one;
            _panel.anchoredPosition = new Vector2(0, 0);
            _panel.gameObject.SetActive(true);
            _grabbable = true;
            _grabbed = false;
            _sliding = false;
        }

        void Update()
        {
            _tapTimer?.Tick(Time.deltaTime);

        }

        void LateUpdate()
        {
            if (_velocity == 0) return;
            var movementX = Mathf.Lerp(_panel.anchoredPosition.x,
                             _panel.anchoredPosition.x + _velocity,
                              Time.deltaTime * 10);
            movementX = Mathf.Clamp(movementX, -Screen.width, 0);
            _panel.anchoredPosition = new Vector2(movementX, _panel.anchoredPosition.y);

            if (!_sliding && _panel.anchoredPosition.x < -Screen.width * _screenRatioBeforeSliding)
            {

                _grabbable = false;
                _tapTimer?.Stop();
                _grabbed = false;
                _velocity = -200;
                _sliding = true;

            }

            if (_panel.anchoredPosition.x < -Screen.width * 0.4f)
            {
                OnFinishSliding?.Invoke();
                _velocity = 0;
                _panel.gameObject.SetActive(false);
                if (_destroyOnFinish)
                    Destroy(gameObject);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _grabbed = _grabbable;
            _pointerDownPosition = eventData.position;

            _tapTimer?.Start();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_grabbed) return;
            _grabbed = false;
            if (_sliding) return;
            if (_tapTimer?.IsRunning ?? false)
            {
                OnTapped?.Invoke();
                _tapTimer.Stop();
            }
            else
            {
                SlideBack();
            }

        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (_sliding) return;
            _velocity = 0;
            if (!_grabbed) return;

            _velocity = Mathf.Min(0, eventData.position.x - _pointerDownPosition.x);
            _pointerDownPosition = eventData.position;
        }

        private void SlideBack()
        {
            _panel.DOAnchorPosX(0, 0.1f).OnComplete(() =>
            {
                _sliding = false;
            });
        }
    }
}
