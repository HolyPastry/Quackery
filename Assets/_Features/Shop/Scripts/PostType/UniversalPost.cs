using System;
using JetBrains.Annotations;
using Quackery.Decks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Quackery.Shops
{
    public class UniversalPost : ShopPost
    {
        [SerializeField] GameObject _paid;
        [SerializeField] GameObject _free;
        [SerializeField] TextMeshProUGUI _titleGUI;
        [SerializeField] TextMeshProUGUI _descriptionGUI;
        [SerializeField] TextMeshProUGUI _priceGUI;
        [SerializeField] TextMeshProUGUI _dealGUI;
        [SerializeField] TextMeshProUGUI _freeTextGUI;
        [SerializeField] Image _logo;
        [SerializeField] Transform _cardParent;
        [SerializeField] GameObject _followerPanel;
        [SerializeField] TextMeshProUGUI _followerTextGUI;
        [SerializeField] TextMeshProUGUI _ratingTextGUI;

        public override void SetupPost(ShopReward shopReward)
        {
            base.SetupPost(shopReward);

            _descriptionGUI.text = shopReward.Description;
            _titleGUI.text = shopReward.Title;

            if (shopReward.Price > 0)
            {
                _paid.SetActive(true);
                _free.SetActive(false);
                _priceGUI.text = $"<sprite name=Coin>{shopReward.Price}";
                _dealGUI.text = $"{UnityEngine.Random.Range(20, 99)}% off";

            }
            else
            {
                _paid.SetActive(false);
                _free.SetActive(true);
                _freeTextGUI.text = GenerateFreeText(shopReward);
            }

            if (SetupNewCard(shopReward)) return;
            if (SetupQualityOfLife(shopReward)) return;


        }

        private bool SetupQualityOfLife(ShopReward shopReward)
        {
            throw new NotImplementedException();
        }

        private bool SetupNewCard(ShopReward shopReward)
        {
            if (shopReward is not NewCardReward newCardReward) return false;

            Card card = DeckServices.CreateCard(newCardReward.ItemData);
            card.transform.SetParent(_cardParent, false);
            card.transform.localPosition = Vector3.zero;
            _followerPanel.SetActive(false);
            _ratingTextGUI.text = "";
            _logo.gameObject.SetActive(false);


            return true;
        }

        private string GenerateFreeText(ShopReward shopReward)
        {
            throw new NotImplementedException();
        }
    }
}
