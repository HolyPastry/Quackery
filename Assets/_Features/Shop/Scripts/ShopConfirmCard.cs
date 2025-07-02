
using System;
using System.Collections;
using Quackery.Decks;
using TMPro;
using UnityEngine;
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
        [SerializeField] private GameObject _buttons;
        [SerializeField] private AnimatedRect _cardAnimated;
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

        public void Confirm()
        {
            //Debug.Log("Confirm");
            PlayConfirmRealization(_reward);
        }

        public void Cancel()
        {
            Hide();
            OnExited(false);
        }

        protected void PlayConfirmRealization(ShopReward reward)
        {
            _buttons.SetActive(false);
            if (reward is NewCardReward newCardReward)
            {
                StartCoroutine(AddCardRoutine(newCardReward));
            }
            else if (reward is QualityOfLifeReward qualityOfLifeReward)
            {
                Hide();
                OnExited(true);
            }

        }

        private IEnumerator AddCardRoutine(NewCardReward newCardReward)
        {
            _subscriptionPrice.text = "";
            _description.text = "";
            _title.text = "";
            yield return new WaitForSeconds(0.2f);
            _cardAnimated.RectTransform = _card.transform as RectTransform;
            _cardAnimated.SlideOut(Direction.Left)
                        .DoComplete(() => { });
            yield return _cardAnimated.WaitForAnimation();

            InventoryServices.AddNewItem(newCardReward.ItemData);
            Destroy(_card.gameObject);
            _description.text = "New Card Added!!";
            yield return new WaitForSeconds(2f);

            Hide();
            OnExited(true);



        }

        public override void Show(ShopReward reward)
        {
            _buttons.SetActive(true);
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

            string subscriptionText = "";
            if (data.Price > 0)
            {
                subscriptionText = "" + data.Price + "#Coin";
            }
            else
            {
                subscriptionText = "Free for a week";
            }
            if (data.Bill != null)
            {
                subscriptionText += $"\n#Bill {Sprites.Replace(data.Bill.MasterText)} ({data.Bill.Price} #Coin)";
            }
            _subscriptionPrice.text = Sprites.Replace(subscriptionText);
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
