
using System;
using Bakery.Dialogs;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery.ChatApp
{
    internal class ChoiceButton : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private Button _button;
        [SerializeField, Child] private TMPro.TextMeshProUGUI _text;

        public event Action<int> OnChoiceSelected;
        public void SetChoice(DialogChoice choice, int index)
        {
            _text.text = choice.Text;
            _button.onClick.AddListener(() => OnChoiceSelected?.Invoke(index));
        }

    }
}
