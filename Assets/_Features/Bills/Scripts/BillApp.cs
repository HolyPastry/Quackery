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
        private bool _initialized;

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
            UpdateButtons();
            _initialized = true;
        }
        private void UpdateButtons(Bill bill) => UpdateButtons();

        private void UpdateButtons()
        {
            int numOverdueBills = BillServices.GetNumOverdueBills();
            ContinueButton.gameObject.SetActive(numOverdueBills < 4);
            GameOverButton.gameObject.SetActive(numOverdueBills >= 4);
        }

        private void OnGameOver()
        {
            Application.Quit();
        }

        private void OnContinue()
        {
            Hide();
        }
    }
}
