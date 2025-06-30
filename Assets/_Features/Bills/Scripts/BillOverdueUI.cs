using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery.Bills
{
    public class BillOverdueUI : MonoBehaviour
    {
        [SerializeField] private List<OverdueCross> _overdueCrosses;

        [SerializeField] private Image _skullInside;

        private Coroutine _updateUICoroutine;
        private Color _originalColor;
        private bool _isOverdue;
        private TweenerCore<Vector3, Vector3, VectorOptions> _skullTween;

        void Awake()
        {
            _originalColor = _skullInside.color;
        }
        private void OnEnable()
        {
            _updateUICoroutine ??= StartCoroutine(UpdateUICoroutine());
        }

        void OnDisable()
        {
            StopAllCoroutines();
            _updateUICoroutine = null;
        }

        IEnumerator Start()
        {
            yield return BillServices.WaitUntilReady();
            yield return CalendarServices.WaitUntilReady();

            _updateUICoroutine ??= StartCoroutine(UpdateUICoroutine());

        }

        private IEnumerator UpdateUICoroutine()
        {
            while (true)
            {

                int overdue = BillServices.GetNumOverdueBills();
                int dueToday = BillServices.GetNumBillDueToday();


                if (overdue + dueToday >= 4)
                {
                    if (!_isOverdue)
                    {
                        _isOverdue = true;
                        _skullInside.color = Color.red;
                        _skullTween?.Kill();
                        _skullTween = _skullInside.transform.DOScale(0.3f, 1).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
                    }
                }
                else
                {
                    _isOverdue = false;
                    _skullInside.color = _originalColor;
                    _skullTween?.Kill();
                }

                for (int i = 0; i < _overdueCrosses.Count; i++)
                {

                    if (i < overdue)
                    {
                        _overdueCrosses[i].SetState(OverdueCross.State.Overdue);
                    }
                    else if (i < dueToday)
                    {
                        _overdueCrosses[i].SetState(OverdueCross.State.DueToday);
                    }
                    else
                    {
                        _overdueCrosses[i].SetState(OverdueCross.State.NotDue);
                    }
                }
                yield return new WaitForSeconds(0.2f);
            }
        }




        internal IEnumerator ActOverdueBillRoutine()
        {
            if (_updateUICoroutine != null)
                StopCoroutine(_updateUICoroutine);

            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < _overdueCrosses.Count; i++)
            {
                if (_overdueCrosses[i].CurrentState == OverdueCross.State.Overdue ||
                   _overdueCrosses[i].CurrentState == OverdueCross.State.NotDue)
                    continue;
                _overdueCrosses[i].SetState(OverdueCross.State.Overdue);
                yield return new WaitForSeconds(0.5f);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}