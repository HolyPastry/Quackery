

using System;
using System.Collections;
using DG.Tweening;
using Quackery.Decks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Quackery.Shops
{
    public class CardPost : ShopPost
    {
        [SerializeField] ShopUIData _shopUIData;
        [SerializeField] TextMeshProUGUI _priceGUI;
        [SerializeField] Image _brandLogo;
        [SerializeField] Image _backgroundImage;
        [SerializeField] Image _highlightImage;
        [SerializeField] Button _buyButton;
        [SerializeField] Transform _cardParent;

        private NewCardReward _cardShopReward;

        private RectTransform rectTransform => transform as RectTransform;

        void OnEnable()
        {
            PurseEvents.OnPurseUpdated += UpdateBuyButton;
            _buyButton.onClick.AddListener(() => StartCoroutine(CardRewardRoutine()));
        }

        void OnDisable()
        {
            PurseEvents.OnPurseUpdated -= UpdateBuyButton;
            _buyButton.onClick.RemoveAllListeners();

        }

        private void UpdateBuyButton(float obj)
        {
            _buyButton.interactable = PurseServices.CanAfford(_cardShopReward.Price);
        }

        private IEnumerator CardRewardRoutine()
        {
            _buyButton.gameObject.SetActive(false);
            yield return ShopApp.SpendMoneyRequest(_cardShopReward.Price);

            DeckServices.AddNew(
                    _cardShopReward.ItemData,
                    EnumCardPile.Draw,
                    EnumPlacement.ShuffledIn,
                    EnumLifetime.Permanent);

            Card card = _cardParent.GetComponentInChildren<Card>();
            AudioSource cardAudio = _cardParent.GetComponentInChildren<AudioSource>();
            if (cardAudio != null)
                cardAudio.Play();
            if (card != null)
                (card.transform as RectTransform).DOAnchorPosX(-Screen.width, 0.5f)
                    .OnComplete(() =>
                    {
                        Destroy(card.gameObject);
                    });
        }

        private bool SetupNewCard()
        {
            Card card = DeckServices.CreateCard(_cardShopReward.ItemData);
            card.transform.SetParent(_cardParent, false);
            _cardParent.gameObject.SetActive(true);
            card.transform.localPosition = Vector3.zero;

            return true;
        }

        public override void SetupPost(ShopReward shopReward)
        {
            _buyButton.gameObject.SetActive(true);
            _cardShopReward = shopReward as NewCardReward;
            rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);

            _priceGUI.text = $"<sprite name=Money>{shopReward.Price}";
            _buyButton.interactable = PurseServices.CanAfford(shopReward.Price);

            if (_shopUIData.TryGetCardUIData(_cardShopReward.ItemData.Category, out var cardData))
            {
                _brandLogo.sprite = cardData.logo;
                _brandLogo.color = Color.white;
                _backgroundImage.color = cardData.backgroundColor;
                _highlightImage.sprite = cardData.highlightImage;
                _highlightImage.color = Color.white;
            }
            else
            {
                Debug.LogWarning($"No UI data found for item category: {_cardShopReward.ItemData.Category}");
                _brandLogo.sprite = null;
                _brandLogo.color = Color.clear;
                _backgroundImage.color = Color.clear;
                _highlightImage.sprite = null;
                _highlightImage.color = Color.clear;
            }

            SetupNewCard();

        }



    }
}
