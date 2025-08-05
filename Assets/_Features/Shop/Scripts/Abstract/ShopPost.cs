using System;
using DG.Tweening;
using UnityEngine;



namespace Quackery.Shops
{
    public class ShopPost : MonoBehaviour
    {


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


        public Action<ShopPost> OnBuyClicked = delegate { };

        //  public ShopRewardData Data { get; set; }







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
