using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Quackery.Decks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery.Shops
{
    public class ChooseACardPost : ShopPost
    {
        [SerializeField] private List<Transform> _cardParents;
        [SerializeField] private ClickableOverlay _clickableOverlayPrefab;
        [SerializeField] private Transform _selectedCardParent;
        [SerializeField] private GameObject _helperTextObject;
        [SerializeField] private GameObject _hiddablePanel;

        //[SerializeField] private  _cardPrefab;
        [SerializeField] private Button _SelectButton;

        [SerializeField] private TextMeshProUGUI _buttonText;
        private List<Card> _cards = new List<Card>();
        private Card _selectedCard;

        void OnEnable()
        {
            _SelectButton.onClick.AddListener(OnSelectButtonClicked);
        }

        void OnDisable()
        {
            _SelectButton.onClick.RemoveListener(OnSelectButtonClicked);
        }

        private void OnSelectButtonClicked()
        {
            _SelectButton.interactable = false;
            DeckServices.AddNew(_selectedCard.Item.Data,
                 EnumCardPile.Draw,
                  EnumPlacement.ShuffledIn,
                   EnumLifetime.Permanent);
            StartCoroutine(SelectCardRoutine());

        }

        private IEnumerator SelectCardRoutine()
        {
            bool moveComplete = false;
            _selectedCard.transform.SetParent(_selectedCardParent);
            _hiddablePanel.SetActive(false);
            _selectedCard.transform.DOScale(1, 1f);
            _selectedCard.transform.DOLocalMove(Vector3.zero, 1f)
                .OnComplete(() => moveComplete = true);
            yield return new WaitUntil(() => moveComplete);
            moveComplete = false;

            _helperTextObject.SetActive(true);

            var rectTransform = _selectedCard.GetComponent<RectTransform>();
            rectTransform.DOAnchorPosX(-Screen.width, 1f)
                .OnComplete(() => moveComplete = true);
            yield return new WaitUntil(() => moveComplete);
            yield return new WaitForSeconds(1f);
            ShopApp.RemovePostRequest(this);
        }

        public override void SetupPost(ShopReward shopReward)
        {
            base.SetupPost(shopReward);
            _helperTextObject.SetActive(false);

            var randomCards = InventoryServices.GetRandomItems(_cardParents.Count);
            for (int i = 0; i < _cardParents.Count; i++)
            {
                Card card = DeckServices.CreateCard(randomCards[i]);
                card.transform.SetParent(_cardParents[i], false);
                card.transform.localScale = Vector3.one;
                card.transform.localPosition = Vector3.zero;
                var clickableOverlay = Instantiate(_clickableOverlayPrefab, card.transform);
                clickableOverlay.Init(card);
                clickableOverlay.OnClicked += OnCardClicked;
                _cards.Add(card);
            }
            _SelectButton.interactable = false;
            _buttonText.text = "Select a card";

        }

        private void OnCardClicked(Card card)
        {
            _SelectButton.interactable = true;
            _cards.ForEach(c => c.SetOutline(false));
            card.SetOutline(true);
            _selectedCard = card;
            _buttonText.text = $"Confirm";
        }
    }
}
