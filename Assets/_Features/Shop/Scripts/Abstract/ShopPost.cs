using System;
using DG.Tweening;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;


namespace Quackery.Shops
{
    public abstract class ShopPost : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        // [SerializeField, Self] private AnimatedRect _animatedRect;
        private Vector2 _position;

        public ShopReward ShopReward { get; private set; }
        public Vector2 AnchoredPosition
        {
            get => _rectTransform.anchoredPosition;
            set => _rectTransform.anchoredPosition = value;
        }
        public Vector2 SizeDelta
        {
            get => _rectTransform.sizeDelta;
            set => _rectTransform.sizeDelta = value;
        }

        private RectTransform _rectTransform => transform as RectTransform;

        public event Action<ShopReward> OnPostClicked = delegate { };

        public Action<ShopPost> OnBuyClicked = delegate { };

        //  public ShopRewardData Data { get; set; }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //_animatedRect.ScaleUp();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // _animatedRect.ScaleDown();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _position = eventData.position;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (Vector2.Distance(_position, eventData.position) > 10f)
                return; // Ignore click if the pointer moved too much
            // OnPostClicked?.Invoke(ShopReward);
        }

        public void SlideIn()
        {
            gameObject.SetActive(true);
            //   _animatedRect.SlideIn(Direction.Bottom);
        }

        public virtual void SetupPost(ShopReward shopReward)
        {
            ShopReward = shopReward;
        }

        public void Hide()
        {
            //    _animatedRect.ZoomOut(true);
            gameObject.SetActive(false);
        }

        internal void MoveTo(Vector2 vector2)
        {
            _rectTransform.DOAnchorPos(vector2, 0.5f, true);
        }

        internal void MoveUp()
        {
            var currentPosY = _rectTransform.anchoredPosition.y;
            var newPosY = currentPosY + Screen.height;
            _rectTransform.DOAnchorPosY(newPosY, 0.5f, true);
        }
    }
}
