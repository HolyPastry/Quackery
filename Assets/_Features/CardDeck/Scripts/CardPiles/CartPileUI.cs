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
            CartEvents.OnCalculatingCartPile += CalculateCartPile;
            CartEvents.OnStacksHighlighted += HighlightStack;

        }



        protected override void OnDisable()
        {
            base.OnDisable();
            CartEvents.OnCalculatingCartPile -= CalculateCartPile;
            CartEvents.OnStacksHighlighted -= HighlightStack;
        }

        private void CalculateCartPile(int index)
        {
            if (!IsItMe(EnumCardPile.Cart, index)) return;
            var rewards = CartServices.GetPileRewards(index);

            _rewardPanel.transform.SetAsLastSibling();

            StartCoroutine(ShowRewardsRoutine(rewards));
        }

        private IEnumerator ShowRewardsRoutine(List<CardReward> rewards)
        {
            yield return new WaitForSeconds(1f);
            foreach (var cardReward in rewards)
            {
                _rewardPanel.ShowReward(cardReward);
                CartServices.AddToCartValue(cardReward.Value);
                yield return new WaitForSeconds(2f);
            }
            CartServices.CompleteCartPileCalculation();
            _rewardPanel.Hide();
        }

        private void HighlightStack(List<int> list)
        {
            _highlighted = list != null && list.Contains(PileIndex);

            _highlightObject.SetActive(_highlighted);

        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_highlighted) return;
            CartServices.SelectPile(PileIndex);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_highlighted) return;
            CartServices.DeselectPile(PileIndex);
        }
    }
}
