using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Quackery.Decks
{
    public class CartPileUI : CardPileUI
    {
        [SerializeField] protected RewardPanel _rewardPanel;

        protected override void OnEnable()
        {
            base.OnEnable();
            CartEvents.OnCalculatingCartPile += CalculateCartPile;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            CartEvents.OnCalculatingCartPile -= CalculateCartPile;
        }

        private void CalculateCartPile(EnumCardPile pile, int index)
        {
            if (!IsItMe(pile, index)) return;
            var rewards = CartServices.GetPileRewards(pile, index);


            _rewardPanel.transform.SetAsLastSibling();

            StartCoroutine(ShowRewardsRoutine(rewards));
        }

        private IEnumerator ShowRewardsRoutine(List<CardReward> rewards)
        {
            foreach (var cardReward in rewards)
            {
                _rewardPanel.ShowReward(cardReward);
                CartServices.AddToCartValue(cardReward.Value);
                yield return new WaitForSeconds(1f);
            }
            CartServices.CompleteCartPileCalculation();
            _rewardPanel.Hide();
        }


    }
}
