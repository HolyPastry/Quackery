using System;
using Quackery.Decks;
using Quackery.Inventories;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Quackery.Shops
{
    internal class ShopConfirmCard : ConfirmationPanel
    {
        [SerializeField] private RectTransform _cardRoot;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private TextMeshProUGUI _subscriptionPrice;
        [SerializeField] private Button _applyButton;
        [SerializeField] private Button _cancelButton;

        void OnEnable()
        {
            _applyButton.onClick.AddListener(Confirm);
            _cancelButton.onClick.AddListener(Cancel);
        }
        void OnDisable()
        {
            _applyButton.onClick.RemoveListener(Confirm);
            _cancelButton.onClick.RemoveListener(Cancel);
        }

        public override void Show(ShopReward reward)
        {
            base.Show(reward);
            var cardReward = reward as AddCardReward;
            Assert.IsNotNull(cardReward, "ShopConfirmCard can only handle AddCardReward types.");

            Card card = DeckServices.CreateCard(cardReward.ItemData);
            card.transform.SetParent(_cardRoot, false);
            card.transform.localPosition = Vector3.zero;
            _subscriptionPrice.text = Sprites.Replace("Free for a week, then " + cardReward.ItemData.SubscriptionCost + "#Coin per week");

            _description.text = Sprites.Replace(cardReward.ItemData.LongDescription);

        }
    }
}
