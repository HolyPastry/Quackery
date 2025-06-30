using System;
using System.Collections;
using System.Collections.Generic;
using KBCore.Refs;
using TMPro;
using UnityEngine;

namespace Quackery
{
    public class QuoteGenerator : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private TextMeshProUGUI _quoteTextGUI;
        [SerializeField] private List<string> _quotes;

        void OnEnable()
        {
            _quoteTextGUI.text = GetRandomQuote();
        }

        private string GetRandomQuote()
        {
            if (_quotes == null || _quotes.Count == 0)
            {
                return "No quotes available.";
            }

            int randomIndex = UnityEngine.Random.Range(0, _quotes.Count);
            return _quotes[randomIndex];
        }
    }
}
