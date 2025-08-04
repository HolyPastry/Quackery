
using System;
using Ink.Runtime;
using Quackery.Decks;
using TMPro;
using UnityEngine;

namespace Quackery
{
    public class CartGauge : StackedGauges
    {
        [SerializeField] private TextMeshProUGUI _cartValueText;

        [SerializeField] private int _survivalGaugeWidth = 300;
        // private float _margin;

        private RectTransform rectTransform => transform as RectTransform;


        void OnEnable()
        {
            CartEvents.OnValueChanged += UpdateGauges;
            CartEvents.OnBonusChanged += UpdateBonus;
            CartEvents.OnModeChanged += OnGameModeChange;
            UpdateGauges();
            OnGameModeChange(CartServices.GetMode());

        }

        void OnDisable()
        {
            CartEvents.OnValueChanged -= UpdateGauges;
            CartEvents.OnModeChanged -= OnGameModeChange;
            CartEvents.OnBonusChanged -= UpdateBonus;

        }
        private void UpdateBonus(int deltaScore) => UpdateGauges();
        private void OnGameModeChange(CartMode mode)
        {
            var height = rectTransform.sizeDelta.y;
            int width = 0;
            switch (mode)
            {
                case CartMode.Survival:
                    width = _survivalGaugeWidth;
                    break;
                case CartMode.Normal:
                    width = Screen.width / 2;
                    break;
                case CartMode.SuperSaiyan:
                    width = Screen.width;
                    break;
            }
            rectTransform.sizeDelta = new Vector2(width, height);

            UpdateGauges();

        }

        public void Show()
        {
            gameObject.SetActive(true);
            _cartValueText.text = string.Empty;

        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void UpdateGauges()
        {
            var cartValue = CartServices.GetValue();
            var cartBonus = CartServices.GetBonus();

            float SurvivalValue = CartServices.GetMaxValue();

            float totalValue = cartValue + cartBonus;
            bool superMode = SurvivalValue == -1;
            if (superMode)
            {

                SurvivalValue = totalValue;
            }
            _gauges[0].fillAmount = cartValue / (SurvivalValue);
            _gauges[1].fillAmount = cartBonus / (SurvivalValue);

            if (_gauges[0].fillAmount > 0.05)
                _gaugeValues[0].text = $"{cartValue}";
            else
                _gaugeValues[0].text = "";

            if (_gauges[1].fillAmount > 0.05)
                _gaugeValues[1].text = $"+{cartBonus}";
            else
                _gaugeValues[1].text = "";

            if (superMode)
                _cartValueText.text = $"<sprite name=Money>{totalValue}";
            else
            {

                _cartValueText.text = $"<sprite name=Money>{totalValue}/{SurvivalValue}";
            }
        }

        internal void HideValue()
        {
            _cartValueText.text = string.Empty;
        }
    }
}
