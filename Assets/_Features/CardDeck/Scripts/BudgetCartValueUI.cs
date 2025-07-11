
using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Quackery.Clients;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery.Decks
{
    public class BudgetCartValueUI : CartValueUI
    {
        [SerializeField] private Image _budgetGauge;
        private Color _originalColor;
        private TweenerCore<Color, Color, ColorOptions> _colorTween;

        void Awake()
        {
            _originalColor = _budgetGauge.color;
        }

        protected override void UpdateCartValue()
        {
            int budget = ClientServices.GetBudget();
            int value = CartServices.GetCartValue();
            _cartValueText.text = $"<sprite name=Coin> {value}/{budget}";
            _budgetGauge.fillAmount = (float)value / budget;
            if (value > budget)
            {
                _colorTween?.Kill();

                _budgetGauge.color = Color.red;

            }
            else if (value > budget * 0.7f)
            {
                if (_colorTween != null && _colorTween.IsActive())
                    return; // Already tweening
                _colorTween = _budgetGauge.DOColor(Color.red, 0.5f).SetLoops(-1, LoopType.Yoyo);
            }
            else
            {
                _colorTween?.Kill();
                _budgetGauge.color = _originalColor;
            }


        }



    }
}
