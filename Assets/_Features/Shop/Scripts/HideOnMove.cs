
using DG.Tweening;
using UnityEngine;

namespace Quackery
{
    public class HideOnMove : MonoBehaviour
    {
        [SerializeField] private ShopScrollRect _shopScrollRect;
        [SerializeField] private Direction _direction = Direction.Top;

        private RectTransform rectTransform => transform as RectTransform;

        void OnEnable()
        {
            _shopScrollRect.OnMovingStarted += Hide;
            _shopScrollRect.OnMovingEnded += Show;
        }
        void OnDisable()
        {
            _shopScrollRect.OnMovingStarted -= Hide;
            _shopScrollRect.OnMovingEnded -= Show;
        }

        private void Hide()
        {
            float width = rectTransform.rect.width;
            float height = rectTransform.rect.height;
            switch (_direction)
            {
                case Direction.Left:
                    rectTransform.DOAnchorPosX(-width, 0.5f).SetEase(Ease.OutBack);
                    break;
                case Direction.Right:
                    rectTransform.DOAnchorPosX(width, 0.5f).SetEase(Ease.OutBack);
                    break;
                case Direction.Top:
                    rectTransform.DOAnchorPosY(height, 0.5f).SetEase(Ease.OutBack);
                    break;
                case Direction.Bottom:
                    rectTransform.DOAnchorPosY(-height, 0.5f).SetEase(Ease.OutBack);
                    break;
            }
        }

        private void Show()
        {
            rectTransform.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.InBack);
        }
    }
}
