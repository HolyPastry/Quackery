
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery.GameMenu
{
    public class GameMenuPanel : MonoBehaviour
    {
        [Header("Panel Settings")]
        [SerializeField] private bool _fullWidth = false;
        [SerializeField] private bool _fullHeight = false;

        [Header("Animation Settings")]
        [SerializeField] private float _animationDuration = 0.5f;
        [SerializeField] private Ease _easeIn = Ease.InCubic;
        [SerializeField] private Ease _easeOut = Ease.OutCubic;

        [Header("UI Elements")]
        [SerializeField] private LayoutGroup _layoutGroup;

        private RectTransform _rectTransform => transform as RectTransform;

        void Awake()
        {
            if (_fullWidth)
                _rectTransform.sizeDelta = new Vector2(Screen.width, _rectTransform.sizeDelta.y);
            if (_fullHeight)
                _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, Screen.height);
        }

        public void TeleportOffscreen()
        {
            _rectTransform.anchoredPosition = new Vector2(-Screen.width, _rectTransform.anchoredPosition.y);
        }
        public void SlideIn()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform);
            _layoutGroup.enabled = false;
            _layoutGroup.enabled = true;
            _rectTransform.DOAnchorPosX(0, _animationDuration)
                .SetEase(_easeIn);

        }

        internal void SlideOut()
        {
            _rectTransform.DOAnchorPosX(-Screen.width, _animationDuration)
                .SetEase(_easeOut);
        }
    }
}
