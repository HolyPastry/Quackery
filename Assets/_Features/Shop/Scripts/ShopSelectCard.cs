using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
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
            => StartCoroutine(ConfirmRoutine());


        private IEnumerator ConfirmRoutine()
        {
            _buttons.SetActive(false);
            _cards.Remove(_selectedCard);

            _explanationText.text = "";
            _cardParent.gameObject.SetActive(false);
            _selectedCard.SetOutline(false);
            _selectedCard.transform.SetParent(_cardAnimated.RectTransform);
            _selectedCard.transform.DOLocalMove(Vector3.zero, 0.2f).SetEase(Ease.Linear);
            _selectedCard.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.Linear);
            yield return new WaitForSeconds(1f);
            _cardAnimated.RectTransform = _selectedCard.transform as RectTransform;


            if (_reward is RemoveCardReward)
            {
                InventoryServices.RemoveItem(_selectedCard.Item);
                _realizationText.text = $"Card Removed from your deck.";
                _realizationText.gameObject.SetActive(true);
            }
            else if (_reward is UpgradeCard upgradeCard)
            {
                //DeckServices.UpgradeCard(_selectedCard.Item, upgradeCard.UpgradeData);
                _realizationText.text = $"Upgraded";
                _realizationText.gameObject.SetActive(true);
            }
            _cardAnimated.ZoomOut(false).DoComplete(() => Destroy(_selectedCard.gameObject));

            yield return new WaitForSeconds(2f);
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

            // Create new cards
            foreach (var item in InventoryServices.GetAllItems())
            {
                var cardInstance = DeckServices.CreateCard(item.Data);
                cardInstance.transform.localScale = Vector3.one * 0.75f;
                cardInstance.transform.SetParent(_cardParent, false);
                cardInstance.Item = item;
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

            if (reward is RemoveCardReward)
                _explanationText.text = "Select a card to remove from your deck.";

            else if (reward is UpgradeCard)
                _explanationText.text = "Select a card to upgrade.";

            PopulateUI();
        }
    }
}
