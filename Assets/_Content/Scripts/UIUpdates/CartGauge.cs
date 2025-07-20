using Holypastry.Bakery;
using Quackery.Clients;
using Quackery.Decks;
using TMPro;
using UnityEngine;

namespace Quackery
{
    public class CartGauge : StackedGauges
    {
        [SerializeField] private TextMeshProUGUI _cartValueText;
        void OnEnable()
        {
            CartEvents.OnCartValueChanged += UpdateGauges;
            UpdateGauges();

        }

        void OnDisable()
        {
            CartEvents.OnCartValueChanged -= UpdateGauges;
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void UpdateGauges()
        {
            var cartValue = CartServices.GetCartValue();
            var cartBonus = CartServices.GetCartBonus();

            float SurvivalValue = (float)BillServices.GetAmountDueToday() / ClientServices.NumClientsToday();
            float totalValue = cartValue + cartBonus;
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

            _cartValueText.text = $"<sprite name=Coin> {totalValue} / {SurvivalValue}";
        }
    }
}
