using System;

using DG.Tweening;
using KBCore.Refs;
using TMPro;
using UnityEngine;

namespace Quackery.Decks
{
    internal class AnimatedText : ValidatedMonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField, Self] private AnimatedRect _animatedRect;

        public AnimatedRect AnimatedRect => _animatedRect;


        public string Text
        {
            get => _text.text;
            set => _text.text = value;
        }
    }

}
