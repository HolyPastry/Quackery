using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Quackery.Decks
{
    public class CartPileUI : CardPileUI, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private GameObject _highlightObject;
        [SerializeField] protected RewardPanel _rewardPanel;
        private bool _highlighted;


        protected override void OnEnable()
        {
            base.OnEnable();
            CartEvents.OnCartRewardCalculated += ShowReward;
            CartEvents.OnStacksHighlighted += HighlightStack;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            CartEvents.OnCartRewardCalculated -= ShowReward;
            CartEvents.OnStacksHighlighted -= HighlightStack;
        }

        private void ShowReward(int index, CardReward reward, float duration)
        {
            if (!IsItMe(EnumCardPile.Cart, index)) return;
            _rewardPanel.ShowReward(reward);
        }

        private void HighlightStack(List<int> list)
        {
            _highlighted = list != null && list.Contains(PileIndex);

            _highlightObject.transform.SetAsLastSibling();
            _highlightObject.SetActive(_highlighted);

        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_highlighted) return;
            CartServices.HoverPile(PileIndex);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_highlighted) return;
            CartServices.UnhoverPile(PileIndex);
        }
    }
}
