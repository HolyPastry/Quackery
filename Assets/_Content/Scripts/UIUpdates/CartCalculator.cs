using System;
using System.Collections;
using System.Collections.Generic;
using Holypastry.Bakery.Quests;
using Quackery.Decks;
using TMPro;
using UnityEngine;

namespace Quackery
{
    public class CartCalculator : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _bonusGUI;
        [SerializeField] private TextMeshProUGUI _valueGUI;
        [SerializeField] private TextMeshProUGUI _totalValueGUI;



        void OnEnable()
        {
            CartEvents.OnValueChanged += UpdateValue;
            CartEvents.OnBonusChanged += UpdateBonus;
            CartEvents.OnTotalValueChanged += UpdateTotalValue;
            CartEvents.OnCalculationCompleted += HideGUIs;

            HideGUIs();
        }



        void OnDisable()
        {
            CartEvents.OnValueChanged -= UpdateValue;
            CartEvents.OnBonusChanged -= UpdateBonus;
            CartEvents.OnTotalValueChanged -= UpdateTotalValue;
            CartEvents.OnCalculationCompleted -= HideGUIs;
        }

        private void HideGUIs()
        {
            _bonusGUI.gameObject.SetActive(false);
            _valueGUI.gameObject.SetActive(false);
        }

        private void UpdateBonus(int obj)
        {
            var bonus = CartServices.GetBonus();
            StartCoroutine(UpdateTextGUI(_bonusGUI, bonus));
        }

        private void UpdateValue()
        {

            var value = CartServices.GetValue();
            StartCoroutine(UpdateTextGUI(_valueGUI, value));
        }

        private void UpdateTotalValue()
        {
            var totalValue = CartServices.GetTotalValue();
            StartCoroutine(UpdateTextGUI(_totalValueGUI, totalValue));
        }

        static IEnumerator UpdateTextGUI(TextMeshProUGUI gui, int newValue)
        {
            gui.gameObject.SetActive(true);
            yield return null;
            int oldValue = int.Parse(gui.text);
            int delta = newValue - oldValue;

            float startTime = Time.time;

            while (Time.time - startTime <= Tempo.WholeBeat)
            {
                int value = (int)Mathf.Lerp(oldValue, newValue, Time.time - startTime / Tempo.WholeBeat);
                gui.text = value.ToString();
                yield return null;
            }
            gui.text = newValue.ToString();
        }


    }
}
