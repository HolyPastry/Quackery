
using System;
using System.Collections;
using DG.Tweening;
using KBCore.Refs;
using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace Quackery
{
    public class EffectUI : ValidatedMonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _valueText;

        [SerializeField] private RectTransform _longBg;
        [SerializeField, Self] private AnimatedRect _animatedRect;

        [SerializeField] private float _extend = -58f;
        [SerializeField] private float _retracted = 0f;
        public Effect Effect { get; private set; }

        bool _useValue = false;

        internal void UpdateStatus(Effect status, bool animate)
        {
            gameObject.SetActive(true);
            _useValue = status.Data.UseValue;
            UpdateStatus(status);

            if (animate)
                StartCoroutine(MakeIconFlyToPosition(status.Origin));
            else
            if (_useValue)
                _longBg.anchoredPosition = new Vector2(_longBg.anchoredPosition.x, _extend);
        }

        public void Hide()
        {
            _longBg.anchoredPosition = new Vector2(_longBg.anchoredPosition.x, _retracted);
            gameObject.SetActive(false);
        }

        internal void UpdateStatus(Effect effect)
        {


            if (effect.Data == null)
            {
                Debug.LogWarning("StatusData is null. Cannot update StatusUI.");
                return;
            }
            _longBg.anchoredPosition = new Vector2(_longBg.anchoredPosition.x, _retracted);

            Effect = effect;
            _icon.sprite = effect.Data.Icon;
            _valueText.text = effect.Value.ToString();

            gameObject.SetActive(true);
        }

        private IEnumerator MakeIconFlyToPosition(Vector2 originPosition)
        {
            yield return null;
            _animatedRect.SlideIn(originPosition);
            if (!_useValue) yield break;

            yield return new WaitForSeconds(0.5f);
            _longBg.DOAnchorPosY(_extend, 0.2f).SetEase(Ease.OutBack);
        }
    }
}

