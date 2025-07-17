using System;
using System.Collections;
using KBCore.Refs;
using Quackery.Clients;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery
{
    public class NormalClientButton : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private Button _button;
        [SerializeField, Child] private TextMeshProUGUI _queueText;
        private bool _initialized;

        void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClicked);
            if (_initialized)
                UpdateUI();
        }

        void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClicked);
        }

        IEnumerator Start()
        {
            yield return FlowServices.WaitUntilEndOfSetup();
            yield return ClientServices.WaitUntilReady();
            UpdateUI();
            _initialized = true;
        }

        private void UpdateUI()
        {
            int queueSize = ClientServices.GetQueueSize();
            _queueText.text = $"{queueSize} patients in queue";
        }

        private void OnButtonClicked()
        {
            ClientServices.StartNormalWeek();
        }


    }
}
