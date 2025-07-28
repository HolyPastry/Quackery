

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Quackery.Artifacts;
using Quackery.Decks;
using Quackery.Followers;
using Quackery.Inventories;
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
        [SerializeField] BillShopWidget _billShopWidget;
        [SerializeField] Button _buyButton;
        [SerializeField] Button _selectButton;

        [SerializeField] private LayoutGroup _leftColumnGroup;
        private ShopReward _shopReward;

        private RectTransform rectTransform => transform as RectTransform;

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

        void OnEnable()
        {
            if (_buyButton != null)
                _buyButton.onClick.AddListener(OnBuyButtonClicked);

            if (_selectButton != null)
                _selectButton.onClick.AddListener(OnSelectButtonClicked);
        }



        void OnDisable()
        {
            if (_buyButton != null)
                _buyButton.onClick.RemoveAllListeners();
            if (_selectButton != null)
                _selectButton.onClick.RemoveAllListeners();
        }
        private void OnSelectButtonClicked() => ShopApp.ShowConfirmation(this);

        private void OnBuyButtonClicked() => StartCoroutine(GiveRewardRoutine());

        private IEnumerator GiveRewardRoutine()
        {

            _paid.transform.DOPunchScale(Vector3.one * 1.1f, 0.5f, 0).OnComplete(() =>
            {
                _paid.SetActive(false);
            });
            yield return new WaitForSeconds(0.3f);
            PurseServices.Modify(-_shopReward.Price);

            yield return new WaitForSeconds(0.6f);

            if (_shopReward is ArtifactReward artifactReward)
            {
                yield return StartCoroutine(ArtifactRewardRoutine(artifactReward.ArtifactData));
            }
            else if (_shopReward is NewCardReward newCardReward)
            {
                yield return StartCoroutine(CardRewardRoutine(newCardReward.ItemData));
            }
            yield return new WaitForSeconds(1f);
            OnBuyClicked?.Invoke(this);
        }

        private IEnumerator CardRewardRoutine(ItemData itemData)
        {
            DeckServices.AddNew(
                    itemData,
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
            yield return new WaitForSeconds(0.5f);
        }

        private IEnumerator ArtifactRewardRoutine(ArtifactData artifactData)
        {
            // QualityOfLifeServices.Acquire(artifactData);


            if (artifactData.FollowerBonus > 0)
            {
                _followerPanel.transform.DOScale(Vector3.zero, 0.5f);
                FollowerServices.ModifyFollowers(artifactData.FollowerBonus);
                yield return new WaitForSeconds(2f);
            }
            if (artifactData.Bill != null)
            {

                _billShopWidget.transform.DOScale(Vector3.zero, 0.5f);
                BillServices.AddNewBill(artifactData.Bill, false);
                yield return new WaitForSeconds(1f);
            }

            _cardParent.gameObject.SetActive(artifactData.BonusItems.Count > 0);

            foreach (var itemData in artifactData.BonusItems)
            {
                SpawnCard(itemData);
                yield return new WaitForSeconds(0.5f);
            }
            foreach (var itemData in artifactData.BonusItems)
            {
                DeckServices.DestroyCardType(itemData);
            }

        }

        private void SpawnCard(ItemData itemData)
        {
            DeckServices.AddNew(
                               itemData,
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

        public override void SetupPost(ShopReward shopReward)
        {
            _shopReward = shopReward;
            rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
            base.SetupPost(shopReward);

            _titleGUI.text = shopReward.Title;
            _descriptionGUI.text = shopReward.Description;

            _billShopWidget.Hide();

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
               SetupArtifact(shopReward) ||
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
            // _ratingTextGUI.text = "";
            _logo.gameObject.SetActive(true);
            _logo.sprite = cardRemovalReward.Logo;
            return true;

        }

        private bool SetupArtifact(ShopReward shopReward)
        {

            if (shopReward is not ArtifactReward artifactReward) return false;
            ArtifactData artifactData = artifactReward.ArtifactData;
            if (artifactData.FollowerBonus > 0)
            {
                _followerTextGUI.text = $"+{artifactData.FollowerBonus}";
                _followerPanel.SetActive(true);
            }
            else
            {
                _followerPanel.SetActive(false);
            }

            if (artifactData.Bill != null)
            {
                _billShopWidget.Show(artifactData.Bill);
            }
            else
            {
                _billShopWidget.Hide();
            }

            // if (qualityOfLifeData.RatingBonus > 0)
            //     _ratingTextGUI.text = $"+{qualityOfLifeData.RatingBonus}";

            // else
            //     _ratingTextGUI.text = "";

            if (artifactData.BonusItems != null)
            {
                // _logo.gameObject.SetActive(false);
                // Card card = DeckServices.CreateCard(artifactData.CardBonus);
                // card.transform.SetParent(_cardParent, false);
                // _cardParent.gameObject.SetActive(true);
                // card.transform.localPosition = Vector3.zero;
            }
            else
            {
                _logo.gameObject.SetActive(true);
                _cardParent.gameObject.SetActive(false);
                _logo.sprite = artifactData.ShopBanner;
            }

            return true;
        }

        private bool SetupNewCard(ShopReward shopReward)
        {
            if (shopReward is not NewCardReward newCardReward) return false;

            Card card = DeckServices.CreateCard(newCardReward.ItemData);
            card.transform.SetParent(_cardParent, false);
            _cardParent.gameObject.SetActive(true);
            card.transform.localPosition = Vector3.zero;
            _followerPanel.SetActive(false);
            //  _ratingTextGUI.text = "";
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
