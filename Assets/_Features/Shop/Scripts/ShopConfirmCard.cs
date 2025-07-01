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
        [SerializeField] private Image _logo;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private TextMeshProUGUI _subscriptionPrice;
        [SerializeField] private Button _applyButton;
        [SerializeField] private Button _cancelButton;
        private Card _card;

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
            _logo.gameObject.SetActive(false);
            if (_card != null)
                Destroy(_card.gameObject);
            switch (reward)
            {
                case NewCardReward newCardReward:
                    ShowNewCardConfirmation(newCardReward);
                    break;
                case QualityOfLifeReward qualityOfLifeReward:
                    ShowQualityOfLifeConfirmation(qualityOfLifeReward);
                    break;
                default:

                    break;
            }
        }

        private void ShowQualityOfLifeConfirmation(QualityOfLifeReward qualityOfLifeReward)
        {
            base.Show(qualityOfLifeReward);

            var data = qualityOfLifeReward.QualityOfLifeData;
            if (data.CardBonus != null)
            {
                _card = DeckServices.CreateCard(data.CardBonus);
                _card.transform.SetParent(_cardRoot, false);
                _card.transform.localPosition = Vector3.zero;
            }
            else
            {
                _logo.gameObject.SetActive(true);
                _logo.sprite = qualityOfLifeReward.QualityOfLifeData.ShopBanner;
            }

            _title.text = Sprites.Replace(qualityOfLifeReward.QualityOfLifeData.Title);
            _description.text = Sprites.Replace(qualityOfLifeReward.QualityOfLifeData.Description);
            if (data.FollowerBonus > 0)
                _description.text += $" +{data.FollowerBonus} Followers. ";
            if (data.RatingBonus > 0)
                _description.text += $" +{data.RatingBonus} Rating. ";
            if (data.Price > 0)
            {
                _subscriptionPrice.text = "" + data.Price + "<sprite name=Coin>";
            }
            else
            {
                _subscriptionPrice.text = "Free for a week";
            }
            if (data.Bill != null)
            {
                _subscriptionPrice.text += $"\nNew bill: {Sprites.Replace(data.Bill.MasterText)} ({data.Bill.Price} <sprite name=Coin>)";
            }
        }

        private void ShowNewCardConfirmation(NewCardReward cardReward)
        {
            base.Show(cardReward);

            _card = DeckServices.CreateCard(cardReward.ItemData);
            _card.transform.SetParent(_cardRoot, false);
            _card.transform.localPosition = Vector3.zero;
            _subscriptionPrice.text = Sprites.Replace("Free for a week,\n then " + cardReward.ItemData.SubscriptionCost + " #Coin per week");

            string text = cardReward.ItemData.LongDescription;
            foreach (var line in cardReward.ItemData.Explanations)
            {
                text += "\n" + line;
            }
            _description.text = Sprites.Replace(text);
            _title.text = Sprites.Replace(cardReward.ItemData.MasterText);
        }
    }
}
