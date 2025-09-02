using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quackery.Inventories;
using System;
namespace Quackery.Decks
{
    public class AddCardOnTopOfDeck : MonoBehaviour
    {
        [SerializeField] private List<ItemData> _cards = new();
        private CardGameApp _cardGame;

        void Awake()
        {
            _cardGame = FindObjectOfType<CardGameApp>(true);
            _cardGame.OnOpened += DrawCards;
        }

        void OnDestroy()
        {
            _cardGame.OnOpened -= DrawCards;
        }

        private void DrawCards()
        {
            DeckServices.ForceOnNextDraw(_cards);
        }
    }
}
