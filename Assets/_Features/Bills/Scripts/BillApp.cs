using System;
using System.Collections;
using System.Collections.Generic;
using Quackery.Bills;
using Quackery.TetrisBill;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery
{
    public class BillApp : App
    {

        public bool IsGameOver { get; private set; }
        public static Action GameOverAction = delegate { };
        public static Action ContinueAction = delegate { };

        void OnEnable()
        {


            GameOverAction = OnGameOver;
            ContinueAction = OnContinue;
            OnClosed += ResetBills;

        }



        void OnDisable()
        {

            GameOverAction = delegate { };
            ContinueAction = delegate { };

            OnClosed -= ResetBills;

        }

        private void ResetBills()
        {
            BillServices.ResetBills();
            TetrisController.ResetRequest();

        }

        private void OnGameOver()
        {
            IsGameOver = true;
            Close();
        }

        private void OnContinue()
        {

            Close();
        }




    }
}
