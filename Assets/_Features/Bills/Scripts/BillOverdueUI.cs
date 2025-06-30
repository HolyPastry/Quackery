using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery.Bills
{
    public class BillOverdueUI : MonoBehaviour
    {
        [SerializeField] private List<OverdueCross> _overdueCrosses;

        private Coroutine _updateUICoroutine;
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