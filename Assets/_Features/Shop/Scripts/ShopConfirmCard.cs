
using System;
using System.Collections;
using Quackery.Decks;
using Quackery.Followers;
using Quackery.QualityOfLife;
using Quackery.Ratings;
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
            else if (reward is ArtifactReward qualityOfLifeReward)
            {

                StartCoroutine(ApplyQualityOfLifeRoutine(qualityOfLifeReward));

            }

        }

        private IEnumerator ApplyQualityOfLifeRoutine(ArtifactReward qualityOfLifeReward)
        {
            _description.text = "\n";
            _subscriptionPrice.text = "";

            var data = qualityOfLifeReward.ArtifactData;

            if (data.Price > 0)
            {
                PurseServices.Modify(-data.Price);
                yield return new WaitForSeconds(1f);
            }


            if (data.FollowerBonus > 0)
            {
                _description.text += $"+{data.FollowerBonus} Followers. \n";
                FollowerServices.ModifyFollowers(data.FollowerBonus);
                yield return new WaitForSeconds(1f);

            }
            // if (data.RatingBonus > 0)
            // {
            //     _description.text += Sprites.Replace($"+{data.RatingBonus} #Rating. \n");
            //     RatingServices.Modify(data.RatingBonus);
            //     yield return new WaitForSeconds(01f);
            // }
            // if (data.CardBonus != null)
            // {
            //     _description.text += $"New Card added to Deck.\n";
            //     _cardAnimated.SlideOut(Direction.Left);

            //     DeckServices.AddNew(data.CardBonus,
            //                     EnumCardPile.Discard,
            //                     EnumPlacement.OnTop,
            //                     EnumLifetime.Permanent);
            //     yield return _cardAnimated.WaitForAnimation();

            // }

            if (data.Bill != null)
            {
                _description.text += Sprites.Replace($"new #Bill {Sprites.Replace(data.Bill.MasterText)}");
                BillServices.AddNewBill(data.Bill, true);
                yield return new WaitForSeconds(1f);
            }
            //QualityOfLifeServices.Acquire(data);
            yield return new WaitForSeconds(1f);
            Hide();
            OnExited(true);
        }

        private IEnumerator AddCardRoutine(NewCardReward newCardReward)
        {
            _subscriptionPrice.text = "";
            _description.text = "";
            _title.text = "";
            yield return Tempo.WaitForABeat;
            _cardAnimated.RectTransform = _card.transform as RectTransform;
            _cardAnimated.SlideOut(Direction.Left);


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
                case ArtifactReward qualityOfLifeReward:
                    ShowQualityOfLifeConfirmation(qualityOfLifeReward);
                    break;
                default:

                    break;
            }
        }

        private void ShowQualityOfLifeConfirmation(ArtifactReward qualityOfLifeReward)
        {
            base.Show(qualityOfLifeReward);

            var data = qualityOfLifeReward.ArtifactData;
            if (data.BonusItems != null)
            {
                // _card = DeckServices.CreateCard(data.CardBonus);
                _card.transform.SetParent(_cardRoot, false);
                _card.transform.localPosition = Vector3.zero;
                _cardAnimated.RectTransform = _card.transform as RectTransform;
            }
            else
            {
                _logo.gameObject.SetActive(true);
                _logo.sprite = qualityOfLifeReward.ArtifactData.ShopBanner;
            }

            // _title.text = Sprites.Replace(qualityOfLifeReward.ArtifactData.Title);
            // _description.text = Sprites.Replace(qualityOfLifeReward.ArtifactData.Description);
            // if (data.FollowerBonus > 0)
            //     _description.text += $" +{data.FollowerBonus} Followers. ";
            // if (data.RatingBonus > 0)
            //     _description.text += $" +{data.RatingBonus} Rating. ";

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
                // subscriptionText += $"\n#Bill {Sprites.Replace(data.Bill.MasterText)} ({data.Bill.Price} #Coin)";
            }
            _subscriptionPrice.text = Sprites.Replace(subscriptionText);
        }

        private void ShowNewCardConfirmation(NewCardReward cardReward)
        {
            base.Show(cardReward);

            _card = DeckServices.CreateCard(cardReward.ItemData);
            _card.transform.SetParent(_cardRoot, false);
            _card.transform.localPosition = Vector3.zero;
            _subscriptionPrice.text = Sprites.Replace("Free for a week,\n then " + cardReward.ItemData.ShopPrice + " #Coin per week");

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
