
using Quackery.Clients;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery.Decks
{
    public class EvaluatedCardValueUI : CartValueUI
    {
        [SerializeField] private Image _valueGauge;

        [SerializeField] private TextMeshProUGUI _rewardText;
        [SerializeField] private AnimatedRect _rewardRect;

        [SerializeField] private float[] _valueThresholds = new float[] { 0.8f, 0.6f, 0.4f, 0.2f };
        [SerializeField] private Color[] _valueColors = new Color[] { Color.yellow, Color.cyan, Color.green, Color.blue, Color.red };

        private float _oldValue;


        protected override void UpdateCartValue(int value)
        {
            float bestValue = (float)BillServices.GetAmountDueToday() / ClientServices.NumClientsToday();

            float newValue = value / bestValue;
            if (_oldValue == -1) _oldValue = newValue;

            _cartValueText.text = $"<sprite name=Coin> {value}/{bestValue}";
            _valueGauge.fillAmount = (float)value / (bestValue * 0.8f);

            _valueGauge.color = _valueColors[^1];

            for (int i = 0; i < _valueThresholds.Length; i++)
            {
                if (newValue >= _valueThresholds[i])
                {
                    if (_oldValue < _valueThresholds[i])
                        TriggerThresholdReward(i);
                    _valueGauge.color = _valueColors[i];
                    break;
                }
            }

            _oldValue = newValue;


        }

        private void TriggerThresholdReward(int i)
        {
            _rewardText.text = i switch
            {
                3 => "Work Harder!",
                2 => "You're doing this",
                1 => "You're doing great!",
                0 => "Outstanding!",
                _ => "Outstanding!",
            };
            _rewardRect.FloatUp(100, 1f);
        }
    }
}
