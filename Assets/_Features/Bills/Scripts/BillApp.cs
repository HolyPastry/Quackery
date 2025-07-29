using System;
using System.Collections;
using System.Collections.Generic;
using Quackery.Bills;
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

        }



        void OnDisable()
        {

            GameOverAction = delegate { };
            ContinueAction = delegate { };

        }

        private void OnGameOver()
        {
            IsGameOver = true;
            Close();
        }

        private void OnContinue()
        {
            BillServices.ResetBills();

            Close();

        }




    }
}
