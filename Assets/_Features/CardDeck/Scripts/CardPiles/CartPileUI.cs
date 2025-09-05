
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Quackery.Decks
{
    [RequireComponent(typeof(Image))]
    public class CartPile : CardPile, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
    {
        [SerializeField] private Image _highlightObject;
        [SerializeField] protected RewardPanel _rewardPanel;

        [SerializeField] private Color _highlightColor = Color.yellow;
        [SerializeField] private Color _defaultColor = Color.green;
        [SerializeField] private Color _backgroundColor = Color.blue;

        private bool _highlighted;


        protected override void OnEnable()
        {
            base.OnEnable();

            CartEvents.OnStacksHighlighted += HighlightStack;
            OnCardMovedIn += UpdateBackground;
            OnCardMovedOut += UpdateBackground;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            CartEvents.OnStacksHighlighted -= HighlightStack;
            OnCardMovedIn -= UpdateBackground;
            OnCardMovedOut -= UpdateBackground;
        }

        private void UpdateBackground()
        {
            var image = GetComponent<Image>();
            if (IsEmpty)
            {
                image.color = _defaultColor;
            }
            else
            {
                image.color = Color.clear;
            }
        }

        private void HighlightStack(List<CardPile> list)
        {
            _highlighted = list != null && list.Contains(this);

            if (_highlighted)
            {
                _highlightObject.gameObject.SetActive(true);
                _highlightObject.transform.SetAsLastSibling();

            }
            else
            {
                _highlightObject.gameObject.SetActive(false);
                _highlightObject.color = _defaultColor;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_highlighted) return;
            CartServices.HoverPile(this);
            _highlightObject.color = _highlightColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_highlighted) return;
            CartServices.UnhoverPile(this);
            _highlightObject.color = _defaultColor;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            if (!_highlighted) return;
            CartServices.HoverPile(this);
            DeckServices.StopPlayCardLoop();
        }

        internal Coroutine ShowRewardLabel(CardReward reward) =>
            StartCoroutine(_rewardPanel.ShowLabel(reward));


        internal Coroutine PunchRewardNumber(string numberString)
            => StartCoroutine(_rewardPanel.PunchNumberRoutine(numberString));

        internal void HideRewardLabel() => _rewardPanel.Hide();

    }
}
