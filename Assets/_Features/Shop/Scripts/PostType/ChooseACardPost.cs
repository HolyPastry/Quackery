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

        [SerializeField] private Button _SelectButton;

        [SerializeField] private TextMeshProUGUI _titleGUI;
        [SerializeField] private TextMeshProUGUI _descriptionGUI;
        [SerializeField] private TextMeshProUGUI _priceGUI;
        [SerializeField] private TextMeshProUGUI _helpTextGUI;
        [SerializeField] private Image _brandLogo;
        private RemoveCardReward _removeCardReward;

        void OnEnable()
        {
            _SelectButton.onClick.AddListener(() => ShopApp.ShowConfirmation(this));
            PurseEvents.OnPurseUpdated += UpdateSelectButton;
        }

        void OnDisable()
        {
            _SelectButton.onClick.RemoveAllListeners();
            PurseEvents.OnPurseUpdated -= UpdateSelectButton;
        }

        private void UpdateSelectButton(float obj)
        {
            if (_removeCardReward != null)
                _SelectButton.interactable = PurseServices.CanAfford(_removeCardReward.Price);
        }

        public override void SetupPost(ShopReward shopReward)
        {
            base.SetupPost(shopReward);
            _removeCardReward = shopReward as RemoveCardReward;
            _titleGUI.text = _removeCardReward.Title;
            _descriptionGUI.text = _removeCardReward.Description;
            _priceGUI.text = $"<sprite name=Money>{_removeCardReward.Price}";
            _brandLogo.sprite = _removeCardReward.Logo;

            _SelectButton.interactable = PurseServices.CanAfford(_removeCardReward.Price);
        }

        internal void HideButton()
        {
            _SelectButton.gameObject.SetActive(false);
            _helpTextGUI.gameObject.SetActive(false);

        }
    }
}
