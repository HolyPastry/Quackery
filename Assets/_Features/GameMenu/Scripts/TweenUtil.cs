using DG.Tweening;
using UnityEngine;

namespace Quackery.GameMenu
{
    public class TweenUtil : MonoBehaviour
    {

        public static void SlideOut(RectTransform rectTransform)
        {
            rectTransform.DOAnchorPosX(-rectTransform.rect.width, 0.5f);
        }

        public static void SlideIn(RectTransform rectTransform)
        {
            rectTransform.DOAnchorPosX(0, 0.5f);
        }
        public static void TeleportOut(RectTransform rectTransform)
        {
            rectTransform.anchoredPosition = new Vector2(-rectTransform.rect.width, rectTransform.anchoredPosition.y);
        }

        public static void TeleportIn(RectTransform rectTransform)
        {
            rectTransform.anchoredPosition = Vector2.zero;
        }
    }
}
