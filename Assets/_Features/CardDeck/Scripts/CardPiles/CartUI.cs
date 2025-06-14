using System.Collections;
using System.Collections.Generic;
using Quackery.Decks;
using UnityEngine;

namespace Quackery
{
    public class CartUI : MonoBehaviour
    {
        private List<CardPileUI> cardPiles;
        void Awake()
        {
            GetComponentsInChildren(true, cardPiles);
            cardPiles.ForEach(pile => pile.gameObject.SetActive(false));
        }

        void OnEnable()
        {
            DeckEvents.OnCartSizeUpdated += OnCartSizeUpdated;
        }

        void OnDisable()
        {
            DeckEvents.OnCartSizeUpdated -= OnCartSizeUpdated;
        }

        private void OnCartSizeUpdated(int cartSize)
        {
            for (int i = 0; i < cardPiles.Count; i++)
            {
                cardPiles[i].gameObject.SetActive(i < cartSize);
            }
        }

    }
}
