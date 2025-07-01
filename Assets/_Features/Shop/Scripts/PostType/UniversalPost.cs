using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Quackery.Decks;
using Quackery.QualityOfLife;
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

        [SerializeField] private LayoutGroup _leftColumnGroup;

        private static readonly List<string> _freeTexts = new()
        {
            "It's on the house!",
            "Try it for free!",
            "No cost, just rewards!",
            "A gift from us to you!",
            "Enjoy this for free!",
            "No strings attached!",
            "Experience it at no cost!",
            "Free for a limited time!",
            "No charge, just benefits!",
            "A complimentary offer!",
            "Enjoy it on us!",
            "No initial payment required!",
            "Experience it without spending!",
            "A free opportunity!",
            "Exclusive Offer",
            "Exclusive Deal",
            "Limited Time Offer",
            "Special Promotion",
            "You're in luck!",
            "You've been selected!",
            "A special gift for you!",
        };
        public override void SetupPost(ShopReward shopReward)
        {

            base.SetupPost(shopReward);

            _titleGUI.text = shopReward.Title;
            _descriptionGUI.text = shopReward.Description;

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

            if (SetupNewCard(shopReward) ||
               SetupQualityOfLife(shopReward) ||
                SetupCardRemoval(shopReward))
            {
                _leftColumnGroup.enabled = false;
                _leftColumnGroup.enabled = true;
                LayoutRebuilder.ForceRebuildLayoutImmediate(_leftColumnGroup.GetComponent<RectTransform>());
            }
            else
                Debug.LogWarning($"ShopReward {shopReward.GetType()} is not supported in UniversalPost");


        }

        private bool SetupCardRemoval(ShopReward shopReward)
        {
            if (shopReward is not RemoveCardReward cardRemovalReward) return false;

            _followerPanel.SetActive(false);
            _ratingTextGUI.text = "";
            _logo.gameObject.SetActive(true);
            _logo.sprite = cardRemovalReward.Logo;
            return true;

        }

        private bool SetupQualityOfLife(ShopReward shopReward)
        {

            if (shopReward is not QualityOfLifeReward qualityOfLifeReward) return false;
            QualityOfLifeData qualityOfLifeData = qualityOfLifeReward.QualityOfLifeData;
            if (qualityOfLifeData.FollowerBonus > 0)
            {
                _followerTextGUI.text = $"+{qualityOfLifeData.FollowerBonus}";
                _followerPanel.SetActive(true);
            }
            else
            {
                _followerPanel.SetActive(false);
            }

            if (qualityOfLifeData.RatingBonus > 0)
                _ratingTextGUI.text = $"+{qualityOfLifeData.RatingBonus}";

            else
                _ratingTextGUI.text = "";

            if (qualityOfLifeData.CardBonus != null)
            {
                _logo.gameObject.SetActive(false);
                Card card = DeckServices.CreateCard(qualityOfLifeData.CardBonus);
                card.transform.SetParent(_cardParent, false);
                card.transform.localPosition = Vector3.zero;
            }
            else
            {
                _logo.gameObject.SetActive(true);
                _logo.sprite = qualityOfLifeData.ShopBanner;
            }

            return true;
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
            int randomIndex = UnityEngine.Random.Range(0, _freeTexts.Count);
            return _freeTexts[randomIndex];
        }
    }
}
