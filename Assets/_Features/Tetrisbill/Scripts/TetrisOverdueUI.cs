using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Quackery.Bills;
using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery.TetrisBill
{
    public class TetrisOverdueUI : MonoBehaviour
    {
        [SerializeField] private List<OverdueCross> _overdueCrosses;

        [SerializeField] private Image _skullInside;
        [SerializeField] private GameObject _overduePanel;
        [SerializeField] private AnimatedRect _animatedCardRect;
        [SerializeField] private ItemData _overdueCurse;

        private Color _originalColor;
        private bool _isOverdue;
        private TweenerCore<Vector3, Vector3, VectorOptions> _skullTween;

        void Awake()
        {
            _originalColor = _skullInside.color;
        }

        public void Setup()
        {
            int numOverdueBill = BillServices.GetNumOverdueBills();
            foreach (var overdueCross in _overdueCrosses)
            {
                overdueCross.SetState(OverdueCross.State.NotDue);
            }
            for (int i = 0; i < numOverdueBill; i++)
            {
                if (i >= _overdueCrosses.Count) return;

                _overdueCrosses[i].SetState(OverdueCross.State.Overdue);

            }
        }


        internal IEnumerator ActOverdueBillRoutine()
        {

            int overdue = BillServices.GetNumOverdueBills();
            if (overdue == 0)
            {
                StopAllCoroutines();
                yield break;
            }
            _overduePanel.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < overdue; i++)
            {

                Card card = DeckServices.CreateCard(_overdueCurse);
                if (card == null)
                {
                    Debug.LogWarning("Failed to create card for overdue curse.");
                    continue;
                }
                card.transform.SetParent(_animatedCardRect.transform);
                card.transform.position = _overdueCrosses[i].transform.position;
                card.transform.localScale = Vector3.zero;
                card.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack);
                card.transform.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.OutBack);
                yield return new WaitForSeconds(1f);
                _animatedCardRect.RectTransform = card.transform as RectTransform;

                _animatedCardRect.SlideOut(Direction.Left)
                                .DoComplete(() => Destroy(card.gameObject));
                DeckServices.AddNew(_overdueCurse,
                                   EnumCardPile.Draw,
                                   EnumPlacement.ShuffledIn,
                                   EnumLifetime.Permanent);

            }
            yield return new WaitForSeconds(0.5f);
            StopAllCoroutines();
        }

        internal void TakeOneCross()
        {
            for (int i = _overdueCrosses.Count - 1; i >= 0; i--)
            {
                if (_overdueCrosses[i].CurrentState == OverdueCross.State.Overdue)
                {
                    _overdueCrosses[i].SetState(OverdueCross.State.NotDue);
                    return;
                }
            }
        }

        internal void AddOneCross()
        {
            for (int i = 0; i < _overdueCrosses.Count; i++)
            {
                if (_overdueCrosses[i].CurrentState == OverdueCross.State.NotDue)
                {
                    _overdueCrosses[i].SetState(OverdueCross.State.Overdue);
                    return;
                }
            }
        }
    }
}

