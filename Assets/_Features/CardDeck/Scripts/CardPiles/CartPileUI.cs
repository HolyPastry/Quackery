using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Quackery.Decks
{
    public class CartPileUI : CardPile, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
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
            CartEvents.OnCartRewardCalculated += ShowReward;
            CartEvents.OnStacksHighlighted += HighlightStack;
            OnCardMovedIn += UpdateBackground;
            OnCardMovedOut += UpdateBackground;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            CartEvents.OnCartRewardCalculated -= ShowReward;
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

        private void ShowReward(int index, CardReward reward, int deltaScore, float duration)
        {
            if (!IsItMe(EnumCardPile.Cart, index)) return;
            _rewardPanel.ShowReward(reward, deltaScore);
        }

        private void HighlightStack(List<int> list)
        {
            _highlighted = list != null && list.Contains(PileIndex);

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
            CartServices.HoverPile(PileIndex);
            _highlightObject.color = _highlightColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_highlighted) return;
            CartServices.UnhoverPile(PileIndex);
            _highlightObject.color = _defaultColor;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            if (!_highlighted) return;
            CartServices.HoverPile(PileIndex);
            DeckServices.StopPlayCardLoop();
        }
    }
}
