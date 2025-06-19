using System.Collections;

using DG.Tweening;
using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;
using UnityEngine.Assertions;

namespace Quackery.Shops
{
    public class AddCardRealization : RewardRealization
    {

        [SerializeField] private Card _card;
        [SerializeField] private AnimatedRect _animatedCard;
        [SerializeField] private Transform _textTransform;

        public override IEnumerator RealizationRoutine(ShopReward reward)
        {
            var cardReward = reward as AddCardReward;
            Assert.IsNotNull(cardReward, "AddCardRealization can only handle AddCardReward types.");
            _card.Item = new Item(cardReward.ItemData);

            _textTransform.DOPunchScale(Vector3.one * 1.1f, 1f, 1, 1);
            yield return new WaitForSeconds(0.5f);
            _animatedCard.SlideOut(Direction.Left);

            yield return _animatedCard.WaitForAnimation();
            yield return new WaitForSeconds(1f);
            gameObject.SetActive(false);
            OnRealizationComplete.Invoke();

        }
    }
}
