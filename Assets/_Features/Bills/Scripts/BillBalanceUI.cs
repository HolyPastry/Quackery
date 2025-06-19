using System;
using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Quackery
{
    public class BillBalanceUI : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI _balanceText;
        private bool _initialized;

        void OnEnable()
        {
            PurseEvents.OnPurseUpdated += UpdateBalance;
            if (_initialized)
                UpdateBalance();

        }
        void OnDisable()
        {
            PurseEvents.OnPurseUpdated -= UpdateBalance;
        }

        IEnumerator Start()
        {
            yield return FlowServices.WaitUntilReady();
            yield return PurseServices.WaitUntilReady();
            UpdateBalance();
            _initialized = true;
        }

        private void UpdateBalance(float obj) => UpdateBalance();


        private void UpdateBalance()
        {

            _balanceText.text = $"Balance: ${PurseServices.GetString()}";
        }
    }
}
