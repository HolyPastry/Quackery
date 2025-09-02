using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Holypastry.Bakery;
using UnityEngine;

namespace Quackery
{
    public class ExplanationParser
    {

        private readonly DataCollection<Explanation> _collection;

        public ExplanationParser() => _collection = new("Explanations");


        public void Parse(string textIn, out string parsedText, out List<Explanation> explanations)
        {

            explanations = new();
            parsedText = textIn;

            foreach (var explanation in _collection.Data)
            {
                Regex regex = new(explanation.name);
                if (!regex.IsMatch(parsedText)) continue;

                explanations.Add(explanation);
                parsedText = regex.Replace(parsedText, $"<b>{explanation.Name}</b>");

            }
        }
    }
}
