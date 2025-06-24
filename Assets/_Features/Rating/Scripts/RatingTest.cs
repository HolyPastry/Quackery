using System.Collections;
using System.Collections.Generic;
using Quackery.Ratings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery
{
    public class RatingTest : MonoBehaviour
    {
        [SerializeField] private Button _plusButton;
        [SerializeField] private Button _minusButton;

        [SerializeField] private TextMeshProUGUI _ratingBonusText;

        void OnEnable()
        {
            _plusButton.onClick.AddListener(OnPlusButtonClicked);
            _minusButton.onClick.AddListener(OnMinusButtonClicked);
            RatingEvents.OnRatingChanged += UpdateUI;
        }

        void OnDisable()
        {
            _plusButton.onClick.RemoveListener(OnPlusButtonClicked);
            _minusButton.onClick.RemoveListener(OnMinusButtonClicked);
            RatingEvents.OnRatingChanged -= UpdateUI;
        }
        IEnumerator Start()
        {
            yield return FlowServices.WaitUntilReady();
            yield return RatingServices.WaitUntilReady();

            UpdateUI();
        }


        private void OnPlusButtonClicked()
        {
            RatingServices.Modify(1);
        }
        private void OnMinusButtonClicked()
        {
            RatingServices.Modify(-1);
        }
        private void UpdateUI()
        {
            _ratingBonusText.text = RatingServices.GetCardBonus().ToString();
        }


    }
}
