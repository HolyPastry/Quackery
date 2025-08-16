
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using KBCore.Refs;
using Quackery.Effects;
using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Quackery
{
    public class StatusUI : ValidatedMonoBehaviour, ITooltipTarget, IPointerClickHandler
    {
        [SerializeField, Self] private AnimatedRect _animatedRect;
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _valueText;

        public Status Status { get; private set; }

        public List<Explanation> Explanations => Status.Explanations;

        public RectTransform RectTransform => transform as RectTransform;

        internal void UpdateStatus(Status status, int value, bool animate)
        {
            gameObject.SetActive(true);
            UpdateStatus(status, value);
            if (animate)
                _animatedRect.Punch();

        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        internal void UpdateStatus(Status status, int value)
        {

            Status = status;
            _icon.sprite = status.Icon;
            if (value == 0)
                _valueText.text = string.Empty;
            else
                _valueText.text = value.ToString();

            gameObject.SetActive(true);
        }

        internal void UpdateStatus(Status effect, int value, Vector3 origin)
        {
            gameObject.SetActive(true);
            _animatedRect.TeleportTo(origin);
            UpdateStatus(effect, value);
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

