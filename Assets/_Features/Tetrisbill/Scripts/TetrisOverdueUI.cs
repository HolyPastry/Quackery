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
        [SerializeField] private AnimatedRect _animatedCardRect;


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

        internal void Reset()
        {
            foreach (var overdueCross in _overdueCrosses)
            {
                overdueCross.SetState(OverdueCross.State.NotDue);
            }
        }
    }
}

