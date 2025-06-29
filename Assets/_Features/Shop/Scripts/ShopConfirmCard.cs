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
        [SerializeField] private Card _card;
        [SerializeField] private TextMeshProUGUI _description;
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

            _card.Item = new Item(cardReward.ItemData);
            _description.text = cardReward.ItemData.ShortDescription;

        }
    }
}
