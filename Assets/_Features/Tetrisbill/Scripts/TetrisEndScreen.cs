using System;
using System.Collections;
using DG.Tweening;
using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery.TetrisBill
{
    public class TetrisEndScreen : MonoBehaviour
    {
        [SerializeField] private ItemData _overdueCurse;
        [SerializeField] private Transform _cardRoot;
        [SerializeField] private Button _continueButton;

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

        internal void Show(int numCrossAdded)
        {
            gameObject.SetActive(true);
            _continueButton.gameObject.SetActive(false);
            if (numCrossAdded > 0)
            {
                BillServices.SetNumOverdueBills(numCrossAdded);
                StartCoroutine(ActOverdueBillRoutine());
            }
            else
            {
                _continueButton.gameObject.SetActive(true);
            }

        }

        internal IEnumerator ActOverdueBillRoutine()
        {

            yield return new WaitForSeconds(0.5f);

            Card card = DeckServices.CreateCard(_overdueCurse);

            card.transform.SetParent(_cardRoot);
            card.transform.position = Vector3.zero;
            card.transform.localScale = Vector3.zero;
            card.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(1f);
            card.transform.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(1f);

            DeckServices.AddNew(_overdueCurse,
                               EnumCardPile.Draw,
                               EnumPlacement.ShuffledIn,
                               EnumLifetime.Permanent);

            _continueButton.gameObject.SetActive(true);
        }
    }
}

