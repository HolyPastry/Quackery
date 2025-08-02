using System;
using System.Collections;
using System.Collections.Generic;
using Quackery.Decks;
using UnityEngine;

namespace Quackery.GameMenu
{
    public class SpawnAllCards : MonoBehaviour
    {
        void OnEnable()
        {
            SpawnAllCardsInDeck();
        }

        void OnDisable()
        {
            DestroyAllChildren();
        }

        private void DestroyAllChildren()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }

        private void SpawnAllCardsInDeck()
        {
            var allItems = InventoryServices.GetAllItems();
            foreach (var item in allItems)
            {
                var card = DeckServices.CreateCard(item.Data);
                card.transform.SetParent(transform, false);
            }
        }
    }
}
