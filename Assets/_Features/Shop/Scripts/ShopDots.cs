using System;
using System.Collections.Generic;
using Quackery.Shops;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery
{
    public class ShopDots : MonoBehaviour
    {
        [SerializeField] private ScrollRect _shopScrollRect;
        [SerializeField] private ShopApp _shopApp;
        [SerializeField] private List<Image> _dots;
        private int _numDots;


        private void OnEnable()
        {
            //_shopScrollRect.OnMoveScreen += HighlightDot;
            _shopApp.OnPostListUpdated += UpdateDots;

            _numDots = -1;


        }

        private void OnDisable()
        {
            //_shopScrollRect.OnMoveScreen -= HighlightDot;
            _shopApp.OnPostListUpdated -= UpdateDots;
        }

        private void UpdateDots(int numDots, int highlightedIndex)
        {
            _numDots = numDots;
            for (int i = 0; i < _dots.Count; i++)
            {
                _dots[i].gameObject.SetActive(i < numDots);
            }
            // HighlightDot(highlightedIndex);
        }

        private void HighlightDot(int index)
        {
            index %= _numDots; // Ensure index wraps around if it exceeds the number of dots
            if (index < 0 || index >= _dots.Count) return;
            for (int i = 0; i < _dots.Count; i++)
            {
                _dots[i].color = i == index ? Color.white : Color.gray;
            }
        }
    }
}
