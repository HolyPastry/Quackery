using System;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Quackery.Shops
{
    public abstract class ShopPost : ValidatedMonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField, Self] private AnimatedRect _animatedRect;
        private Vector2 _position;

        public ShopReward ShopReward { get; private set; }

        public event Action<ShopReward> OnPostClicked;

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
                return; // Ignore click if the pointer moved too muc
            OnPostClicked?.Invoke(ShopReward);
        }

        public void SlideIn()
        {
            gameObject.SetActive(true);
            _animatedRect.SlideIn(Direction.Bottom);
        }

        public virtual void SetupPost(ShopReward shopReward)
        {
            ShopReward = shopReward;
        }

        public void Hide()
        {
            _animatedRect.ZoomOut(true);
            gameObject.SetActive(false);
        }
    }
}
