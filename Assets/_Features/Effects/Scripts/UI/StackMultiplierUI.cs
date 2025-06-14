using System.Collections;
using System.Collections.Generic;
using KBCore.Refs;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Quackery
{
    public class StackMultiplierUI : ValidatedMonoBehaviour
    {
        [SerializeField, Child] private TextMeshProUGUI _text;

        public string MultiplierText => _multiplier.ToString();

        private int _multiplier;

        public void UpdateMultipler(int multiplier)
        {
            _text.text = $"x{multiplier}";
            _multiplier = multiplier;
        }

    }
}
