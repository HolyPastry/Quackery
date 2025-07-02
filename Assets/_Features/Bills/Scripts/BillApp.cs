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

        [SerializeField] private Button ContinueButton;
        [SerializeField] private Button GameOverButton;

        [SerializeField] private GameObject _warningPanel;

        [SerializeField] private BillOverdueUI _billOverdueUI;
        private bool _initialized;

        public bool IsGameOver { get; private set; }

        void OnEnable()
        {

            ContinueButton.onClick.AddListener(OnContinue);
            GameOverButton.onClick.AddListener(OnGameOver);
            BillEvents.OnBillUpdated += UpdateButtons;
            if (_initialized) UpdateButtons();
        }



        void OnDisable()
        {
            ContinueButton.onClick.RemoveListener(OnContinue);
            GameOverButton.onClick.RemoveListener(OnGameOver);
            BillEvents.OnBillUpdated -= UpdateButtons;
        }

        IEnumerator Start()
        {
            yield return FlowServices.WaitUntilEndOfSetup();
            yield return BillServices.WaitUntilReady();
            IsGameOver = false;
            UpdateButtons();
            _initialized = true;
        }

        private void UpdateButtons(Bill bill) => UpdateButtons();

        private void UpdateButtons()
        {
            int numOverdueBills = BillServices.GetNumOverdueBills();
            int numDueToday = BillServices.GetNumBillDueToday();
            ContinueButton.gameObject.SetActive(numOverdueBills + numDueToday < 4);
            GameOverButton.gameObject.SetActive(numOverdueBills + numDueToday >= 4);

            _warningPanel.SetActive(numOverdueBills + numDueToday >= 4);

        }

        private void OnGameOver()
        {
            IsGameOver = true;
            StartCoroutine(ContinueRoutine());
        }

        private void OnContinue()
        {
            StartCoroutine(ContinueRoutine());

        }

        private IEnumerator ContinueRoutine()
        {
            yield return _billOverdueUI.ActOverdueBillRoutine();

            BillServices.ResetBills();
            Hide();
        }


    }
}
