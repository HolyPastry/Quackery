using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;

namespace Quackery.TetrisBill
{
    public class MoneyScale : MonoBehaviour
    {

        [SerializeField] private RectTransform _rowHighlight;
        [SerializeField] private RectTransform _moneyHighlight;

        private int _budgetIndex = 0;

        public int BudgetAmount { get; private set; } = 0;

        public int BudgetIndex
        {
            get => _budgetIndex;
            set
            {
                _budgetIndex = value;
                _rowHighlight.anchoredPosition = new Vector2(_rowHighlight.anchoredPosition.x, _budgetIndex * TetrisGame.CellSize());
                TetrisGame.SetBudgetIndex(_budgetIndex);
            }
        }

        private List<TextMeshProUGUI> _textComponents = new();

        public static Func<int> GetBudgetAmount = () => 0;
        public static Action<int> SetMoneyAmount = (amount) => { };

        void OnEnable()
        {
            GetBudgetAmount = () => BudgetAmount;
            SetMoneyAmount = (amount) =>
            {
                for (int i = 0; i < _textComponents.Count - 1; i++)
                {

                    if (i + 1 > amount)
                    {
                        BudgetAmount = int.Parse(_textComponents[i].text);
                        _moneyHighlight.anchoredPosition = new Vector2(_moneyHighlight.anchoredPosition.x, i * TetrisGame.CellSize());
                        return;
                    }
                }
            };
        }
        void OnDisable()
        {
            GetBudgetAmount = () => 0;
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
        }

        private void SetText(TextMeshProUGUI textComponent, int budget, bool isBudget)
        {
            textComponent.text = budget.ToString();
            textComponent.color = isBudget ? Color.black : Color.gray;
        }
    }
}
