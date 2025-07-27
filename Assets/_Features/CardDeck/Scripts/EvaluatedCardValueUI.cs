
using System;
using DG.Tweening;
using Quackery.Clients;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Quackery.Decks
{


    public class EvaluatedCardValueUI : CartValueUI
    {
        [SerializeField] private Image _valueGauge;

        [SerializeField] private TextMeshProUGUI _rewardText;
        [SerializeField] private AnimatedRect _rewardRect;

        private float _oldValue;

        protected override void UpdateCartValue()
        {
            float bestValue = (float)BillServices.GetAmountDueToday() / ClientServices.NumClientsToday();

            var baseValue = CartServices.GetValue();
            var bonusValue = CartServices.GetBonus();

            var totalValue = baseValue + bonusValue;

            float newValue = totalValue / bestValue;
            //     CartEvaluation evaluation = CartServices.GetCartEvaluation();

            if (_oldValue == -1) _oldValue = newValue;

            _cartValueText.text = $"<sprite name=Coin> {baseValue} <size=80%><color=#000000>+{bonusValue}</color></size>";
            _valueGauge.fillAmount = (1 / 0.8f) * newValue;

            // if (newValue > _oldValue)
            //     PlayRealization(evaluation);

            _oldValue = newValue;


        }

        // private void PlayRealization(CartEvaluation evaluation)
        // {
        //     _valueGauge.color = evaluation.Color;

        //     TriggerThresholdReward(evaluation.Index);

        //     // _cartValueText.transform.DOPunchScale(Vector3.one * 0.2f, 0.5f, 1, 1f)
        //     //     .SetEase(Ease.OutElastic);
        // }

        private void TriggerThresholdReward(int i)
        {
            _rewardText.text = i switch
            {
                4 => "You can do better!",
                3 => "Work Harder!",
                2 => "You're doing this",
                1 => "You're doing great!",
                0 => "You're beautiful!",
                _ => "Outstanding!",
            };
            _rewardRect.FloatUp(100, 1f);
        }
    }
}
