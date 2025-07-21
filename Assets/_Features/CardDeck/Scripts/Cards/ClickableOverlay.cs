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
        [SerializeField] private Card _parentCard;
        public event Action<Card> OnClicked = delegate { };
        public void OnPointerClick(PointerEventData eventData)
        {
            OnClicked.Invoke(_parentCard);
        }
        public void Init(Card card)
        {
            _parentCard = card;
        }
    }
}
