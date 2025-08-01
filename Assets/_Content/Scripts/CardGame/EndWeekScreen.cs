using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using KBCore.Refs;
using Quackery.Inventories;
using Quackery.Progression;
using Quackery.Shops;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery.Decks
{
    internal class EndWeekScreen : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _skipButton;


        [SerializeField] private App _endOfWeekApp;
        [SerializeField] private ShopManagerData _shopManagerData;
        [SerializeField] private List<Transform> _cardRoots = new();
        [SerializeField] private ClickableOverlay _clickableOverlayPrefab;
        private Card _selectedCard;
        private readonly List<Card> _cards = new();

        void OnEnable()
        {
            _skipButton.onClick.AddListener(CloseGame);
            _confirmButton.onClick.AddListener(ConfirmCard);
            Show();
        }



        void OnDisable()
        {
            _skipButton.onClick.RemoveAllListeners();
            _confirmButton.onClick.RemoveAllListeners();
        }

        internal void Show()
        {
            StartCoroutine(ShowCoroutine());
        }


        private IEnumerator ShowCoroutine()
        {
            _cards.Clear();
            _selectedCard = null;
            _confirmButton.interactable = false;

            int currentLevel = ProgressionServices.GetLevel();

            _shopManagerData.RollRarity(currentLevel, out EnumRarity rarity);

            for (int i = 0; i < 2; i++)
            {
                ItemData itemData = InventoryServices.GetRandomMatchingItem(
                    itemData => itemData.Category != EnumItemCategory.Curse &&
                    itemData.Category != EnumItemCategory.TempCurse &&
                    itemData.Rarity == rarity);

                if (itemData != null)
                {
                    Card card = DeckServices.CreateCard(itemData);
                    _cards.Add(card);

                    card.transform.SetParent(_cardRoots[i], false);
                    card.transform.localScale = Vector3.zero;
                    card.transform.localPosition = Vector3.zero;
                    card.transform.DOScale(1, 0.5f);
                    yield return new WaitForSeconds(0.3f);

                    var overlay = Instantiate(_clickableOverlayPrefab, card.transform);
                    overlay.transform.localPosition = Vector3.zero;
                    overlay.transform.localScale = Vector3.one;
                    overlay.Init(card);
                    overlay.OnClicked += OnCardClicked;
                }

            }

        }

        private void OnCardClicked(Card clickedCard)
        {
            foreach (var card in _cards)
            {
                card.SetOutline(card == clickedCard);
            }
            _selectedCard = clickedCard;
            _confirmButton.interactable = true;
        }

        private void CloseGame()
        {
            _confirmButton.interactable = false;
            foreach (var card in _cards)
            {
                if (_selectedCard == card)
                {
                    card.transform.DOMoveX(-Screen.width, 0.5f)
                        .OnComplete(() => Destroy(card.gameObject));
                }
                else
                {
                    card.transform.DOScale(0, 0.5f)
                        .OnComplete(() => Destroy(card.gameObject));
                }

            }
            _cards.Clear();
            _endOfWeekApp.Close();
        }

        private void ConfirmCard()
        {
            if (_selectedCard != null)
                DeckServices.AddNew(_selectedCard.Item.Data, EnumCardPile.Draw, EnumPlacement.AtTheBottom, EnumLifetime.Permanent);
            CloseGame();
        }
    }

}
