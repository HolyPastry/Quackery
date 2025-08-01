using System;
using System.Collections;
using System.Collections.Generic;
using KBCore.Refs;
using Quackery.Decks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Quackery
{
    public class ClickableOverlay : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private bool _showTooltip = false;
        [SerializeField] private Card _card;
        public event Action<Card> OnClicked = delegate { };
        public void OnPointerClick(PointerEventData eventData)
        {
            if (_card == null)
            {
                _card = GetComponentInChildren<Card>(true);
            }
            if (_card == null)
            {
                return;
            }
            OnClicked.Invoke(_card);
            if (_showTooltip)
                Tooltips.ShowTooltipRequest(_card);

        }
        public void Init(Card card)
        {
            _card = card;
        }
    }
}
