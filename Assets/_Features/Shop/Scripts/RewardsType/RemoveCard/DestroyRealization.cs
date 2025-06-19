using System.Collections;
using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;
using UnityEngine.Assertions;

namespace Quackery.Shops
{

    public class DestroyRealization : RewardRealization
    {
        [SerializeField] private Card _card;
        [SerializeField] private AnimatedRect _animatedCard;
        public override IEnumerator RealizationRoutine(ShopReward reward)
        {
            var cardReward = reward as RemoveCard;
            Assert.IsNotNull(cardReward, "DestroyRealization can only handle DestroyCardReward types.");
            _card.Item = cardReward.ItemToRemove;

            _animatedCard.ZoomIn();
            yield return new WaitForSeconds(1f);
            gameObject.SetActive(false);
            OnRealizationComplete.Invoke();
        }
    }
}
