using System.Collections;
using System.Collections.Generic;
using Quackery.Decks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery.Shops
{
    public class ShopSelectCard : ConfirmationPanel
    {

        [SerializeField] private ClickableOverlay _clickableOverlay;
        [SerializeField] private Transform _cardParent;

        [SerializeField] private TextMeshProUGUI _explanationText;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _cancelButton;
        private bool _initialized;

        private List<Card> _cards = new();

        void OnEnable()
        {

            _confirmButton.onClick.AddListener(Confirm);
            _cancelButton.onClick.AddListener(Cancel);
        }

        void OnDisable()
        {
            _confirmButton.onClick.RemoveListener(Confirm);
            _cancelButton.onClick.RemoveListener(Cancel);
        }
        private void PopulateUI()
        {

            // Clear existing cards
            foreach (var card in _cards)
            {
                Destroy(card.gameObject);
            }
            _cards.Clear();

            // Create new cards
            foreach (var item in InventoryServices.GetAllItems())
            {
                var cardInstance = DeckServices.CreateCard(item.Data);
                cardInstance.transform.SetParent(_cardParent, false);

                cardInstance.Item = item;
                var overlay = Instantiate(_clickableOverlay, cardInstance.transform);
                overlay.Init(cardInstance);
                overlay.OnClicked += SelectCard;

                _cards.Add(cardInstance);
            }

            // Set explanation text
            //_explanationText.text = "Select a card to Destroy.";
        }

        private void SelectCard(Card card)
        {
            // Deselect all cards
            foreach (var c in _cards)
            {
                c.SetOutline(false);
            }
            card.SetOutline(true);

            if (_reward is RemoveCardReward removeCard)
                removeCard.ItemToRemove = card.Item;

            else if (_reward is UpgradeCard upgradeCard)
                upgradeCard.ItemToUpgrade = card.Item;
        }

        public override void Show(ShopReward reward)
        {
            base.Show(reward);

            if (reward is RemoveCardReward)
                _explanationText.text = "Select a card to remove from your deck.";

            else if (reward is UpgradeCard)
                _explanationText.text = "Select a card to upgrade.";

            PopulateUI();
        }
    }
}
