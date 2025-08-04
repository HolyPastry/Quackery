using System;
using System.Collections;
using DG.Tweening;
using Quackery.Decks;
using Quackery.GameMenu;
using Quackery.Inventories;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery.TetrisBill
{
    public class TetrisEndScreen : MonoBehaviour
    {
        [SerializeField] private ItemData _overdueCurse;
        [SerializeField] private Transform _cardRoot;
        [SerializeField] private Button _continueButton;
        [SerializeField] private TextMeshProUGUI _resultText;
        [SerializeField] private TextMeshProUGUI _debtWarningText;

        internal void Hide()
        {
            gameObject.SetActive(false);

        }

        void OnEnable()
        {
            _continueButton.onClick.AddListener(Continue);
        }
        void OnDisable()
        {
            _continueButton.onClick.RemoveListener(Continue);
        }

        private void Continue()
        {
            BillApp.ContinueAction();
        }

        internal void Show(int numCrossAdded, int MoneyLost)
        {
            _resultText.text = "";
            gameObject.SetActive(true);
            _continueButton.gameObject.SetActive(false);
            _debtWarningText.gameObject.SetActive(false);

            StartCoroutine(RealRoutine(numCrossAdded, MoneyLost));

        }

        internal IEnumerator RealRoutine(int numCrossAdded, int MoneyLost)
        {
            yield return new WaitForSeconds(0.1f);
            _resultText.text = $"<b>Well Done!</b>";
            yield return new WaitForSeconds(0.2f);

            _resultText.text += $"\nAmount Due: {MoneyScale.GetMoneyAmount()}";
            if (MoneyLost > 0)
            {
                PurseServices.Modify(-MoneyScale.GetMoneyAmount());
                DeckServices.AddNew(_overdueCurse,
                                      EnumCardPile.Draw,
                                      EnumPlacement.ShuffledIn,
                                      EnumLifetime.Permanent);
                yield return new WaitForSeconds(2f);
            }

            if (numCrossAdded > 0)
            {
                _resultText.text += $"\n<color=red>Debt Due: <b>{numCrossAdded}</b></color>";
                yield return new WaitForSeconds(0.5f);

                yield return new WaitForSeconds(0.8f);
                Card card = DeckServices.CreateCard(_overdueCurse);

                card.transform.SetParent(_cardRoot);
                card.transform.localPosition = Vector3.zero;
                card.transform.localScale = Vector3.zero;
                card.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
                yield return new WaitForSeconds(2f);

                yield return GameMenuController.AddToDeckRequest(new() { card });
                _debtWarningText.gameObject.SetActive(true);
                yield return new WaitForSeconds(0.4f);
            }

            _continueButton.gameObject.SetActive(true);
        }
    }
}

