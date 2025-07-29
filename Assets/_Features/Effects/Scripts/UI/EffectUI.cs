
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using KBCore.Refs;
using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Quackery
{
    public class EffectUI : ValidatedMonoBehaviour, ITooltipTarget, IPointerClickHandler
    {
        [SerializeField, Self] private AnimatedRect _animatedRect;
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _valueText;

        public Effect Effect { get; private set; }

        public List<Explanation> Explanations => Effect.Data.Explanations ?? new List<Explanation>();

        public RectTransform RectTransform => transform as RectTransform;

        internal void UpdateStatus(Effect status, bool animate)
        {
            gameObject.SetActive(true);
            UpdateStatus(status);
            if (animate)
                _animatedRect.Punch();

            // if (animate)
            //     StartCoroutine(MakeIconFlyToPosition(status.Origin));
            // else
            // if (_useValue)
            //     _longBg.anchoredPosition = new Vector2(_longBg.anchoredPosition.x, _extend);
        }

        public void Hide()
        {
            // _longBg.anchoredPosition = new Vector2(_longBg.anchoredPosition.x, _retracted);
            gameObject.SetActive(false);
        }

        internal void UpdateStatus(Effect effect)
        {
            if (effect.Data == null)
            {
                Debug.LogWarning("StatusData is null. Cannot update StatusUI.");
                return;
            }
            // _longBg.anchoredPosition = new Vector2(_longBg.anchoredPosition.x, _retracted);

            Effect = effect;
            _icon.sprite = effect.Data.Icon;
            if (effect.Value == 0)
                _valueText.text = string.Empty;
            else
            {
                if (effect.Data.CanBeNegative)
                    _valueText.text = effect.Value.ToString();
                else
                    _valueText.text = Mathf.Abs(effect.Value).ToString();
            }

            gameObject.SetActive(true);
        }

        internal void UpdateStatus(Effect effect, Vector3 origin)
        {
            gameObject.SetActive(true);
            _animatedRect.TeleportTo(origin);
            UpdateStatus(effect);
            _animatedRect.SlideToZero();

        }

        public void OnPointerClick(PointerEventData eventData)
        {
            transform.SetAsLastSibling();
            Tooltips.ShowTooltipRequest?.Invoke(this);
            eventData.Use();
        }

    }
}

