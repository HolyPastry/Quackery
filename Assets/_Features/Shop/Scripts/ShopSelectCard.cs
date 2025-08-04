using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Quackery.Decks;
using Quackery.GameMenu;
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
        [SerializeField] private TextMeshProUGUI _realizationText;
        [SerializeField] private AnimatedRect _cardAnimated;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _cancelButton;

        [SerializeField] private GameObject _buttons;

        private readonly List<Card> _cards = new();
        private Card _selectedCard;



        void OnEnable()
        {
            _confirmButton.onClick.AddListener(Confirm);
            _cancelButton.onClick.AddListener(Cancel);
        }

        private void Confirm()
        {
            _buttons.SetActive(false);
            ShopApp.RemoveCardRequest(_reward.Price, _selectedCard.Item);
            OnExited(true);
            Hide();
        }


        void OnDisable()
        {
            _confirmButton.onClick.RemoveListener(Confirm);
            _cancelButton.onClick.RemoveListener(Cancel);
        }

        private void Cancel()
        {
            OnExited(false);
            Hide();
        }

        private void PopulateUI()
        {

            // Clear existing cards
            var cardToDestroy = _cards.FindAll(c => c != null);
            foreach (var card in cardToDestroy)
            {
                Destroy(card.gameObject);
            }
            _cards.Clear();

            var cards = DeckServices.GetMatchingCards(card =>
                            card.Category != Inventories.EnumItemCategory.Curse &&
                            card.Category != Inventories.EnumItemCategory.TempCurse, EnumCardPile.Draw);


            foreach (var card in cards)
            {
                var cardInstance = DeckServices.CreateCard(card.Item.Data);
                cardInstance.transform.localScale = Vector3.one * 0.75f;
                cardInstance.transform.SetParent(_cardParent, false);
                cardInstance.Item = card.Item;
                var overlay = Instantiate(_clickableOverlay, cardInstance.transform);
                overlay.Init(cardInstance);
                overlay.OnClicked += SelectCard;

                _cards.Add(cardInstance);
            }
        }

        private void SelectCard(Card card)
        {
            // Deselect all cards
            foreach (var c in _cards)
            {
                c.SetOutline(false);
            }
            card.SetOutline(true);
            _confirmButton.interactable = true;

            _selectedCard = card;


        }

        public override void Show(ShopReward reward)
        {

            base.Show(reward);
            _realizationText.gameObject.SetActive(false);
            _cardParent.gameObject.SetActive(true);
            _buttons.SetActive(true);

            _confirmButton.interactable = false;
            _confirmButton.GetComponentInChildren<TextMeshProUGUI>().text = Sprites.Replace($"#Money {reward.Price}");

            if (reward is RemoveCardReward)
                _explanationText.text = "Select a card to remove from your deck.";

            else if (reward is UpgradeCard)
                _explanationText.text = "Select a card to upgrade.";

            PopulateUI();
        }
    }
}
