using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;

namespace Quackery.TetrisBill
{
    public class MoneyScale : MonoBehaviour
    {

        [SerializeField] private RectTransform _budgetHighlight;
        [SerializeField] private RectTransform _moneyHighlight;

        private int _budgetIndex = 0;

        public int MoneyAmount { get; private set; } = 0;

        public int BudgetIndex
        {
            get => _budgetIndex;
            set
            {
                _budgetIndex = value;
                _budgetHighlight.anchoredPosition = new Vector2(_budgetHighlight.anchoredPosition.x, _budgetIndex * TetrisGame.CellSize());
                TetrisGame.SetBudgetIndex(_budgetIndex);
            }
        }

        private List<TextMeshProUGUI> _textComponents = new();

        public static Func<int> GetMoneyAmount = () => 0;
        public static Action<int> SetMoneyAmount = (index) => { };

        void OnEnable()
        {
            GetMoneyAmount = () => MoneyAmount;
            SetMoneyAmount = (index) =>
            {
                for (int i = 0; i < _textComponents.Count - 1; i++)
                {

                    if (i + 1 > index || i + 1 > BudgetIndex)
                    {
                        MoneyAmount = int.Parse(_textComponents[i].text);
                        _moneyHighlight.anchoredPosition = new Vector2(_moneyHighlight.anchoredPosition.x, i * TetrisGame.CellSize());
                        return;
                    }
                }
            };
        }
        void OnDisable()
        {
            GetMoneyAmount = () => 0;
            SetMoneyAmount = (amount) => { };
        }
        IEnumerator Start()
        {
            yield return FlowServices.WaitUntilEndOfSetup();
            yield return BillServices.WaitUntilReady();
            yield return PurseServices.WaitUntilReady();

            int budget = PurseServices.GetAmount();
            GetComponentsInChildren(_textComponents);
            int amount = 200;
            int preAmount = 100;
            var textComponent = _textComponents[0];
            if (budget > 0 && budget < preAmount)
            {
                BudgetIndex = 0;

                SetText(textComponent, 0, true);
            }
            else
                SetText(textComponent, 0, false);

            for (int i = 1; i < _textComponents.Count; i++)
            {
                textComponent = _textComponents[i];
                if (budget > preAmount && budget < amount)
                {
                    BudgetIndex = i;

                    SetText(textComponent, preAmount, true);
                }
                else
                    SetText(textComponent, preAmount, false);
                int newAmount = amount + preAmount;
                preAmount = amount;
                amount = newAmount;
            }
            if (budget == 0)
                BudgetIndex = 0;

        }

        private void SetText(TextMeshProUGUI textComponent, int budget, bool isBudget)
        {
            textComponent.text = budget.ToString();
            textComponent.color = isBudget ? Color.black : Color.gray;
        }
    }
}
