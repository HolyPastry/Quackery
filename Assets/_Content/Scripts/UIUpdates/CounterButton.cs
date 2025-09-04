
using System;
using KBCore.Refs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery
{
    public class CounterButton : ValidatedMonoBehaviour
    {
        [SerializeField, Child] private Button _button;
        [SerializeField] private TextMeshProUGUI _numberText;

        private int _counter = -1;

        public int Counter
        {
            get => _counter;
            set
            {
                _counter = value;
                _numberText.text = _counter.ToString();
                _button.interactable = _counter > 0;
            }
        }

        public bool Interactable
        {
            get => _button.interactable;
            set => _button.interactable = value && _counter > 0;
        }

        public Action OnClicked = delegate { };

        void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClicked);
        }

        void OnDisable()
        {
            _button.onClick.RemoveAllListeners();
        }

        private void OnButtonClicked()
        {
            Counter--;
            OnClicked();
        }
    }
}
