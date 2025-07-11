using System;
using System.Collections;
using System.Collections.Generic;
using Quackery.Effects;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Decks
{

    public class DrawPile : CardPile
    {
        private readonly CardFactory _cardFactory;


        public DrawPile(CardFactory cardFactory) : base(EnumCardPile.Draw, 0)
        {
            _cardFactory = cardFactory;
            var allItems = InventoryServices.GetAllItems();
            Populate(allItems);
            RegisterServices();
        }

        ~DrawPile()
        {
            UnregisterServices();
        }

        private void RegisterServices()
        {
            // DeckServices.AddNewToDraw = AddNewCard;
            // DeckServices.AddMultipleInstancesToDrawDeck = AddMultipleNew;
            // DeckServices.AddToDrawPile = AddNewCardsToDeck;
            DeckServices.DrawSpecificCards = DrawSpecificCards;
            DeckServices.DrawCategory = DrawCategoryCard;
            DeckServices.Draw = DrawMany;
        }



        private void UnregisterServices()
        {

            // DeckServices.AddMultipleInstancesToDrawDeck = delegate { };
            // DeckServices.AddToDrawPile = delegate { };
            // DeckServices.AddNewToDraw = (itemData, isPermanent, origin) => { };
            DeckServices.DrawSpecificCards = delegate { };
            DeckServices.DrawCategory = category => null;
            DeckServices.Draw = number => new List<Card>();
        }

        internal void Populate(List<Item> items)
        {
            foreach (var item in items)
                AddToDeck(item);
        }
        private void AddToDeck(Item item, Transform origin = null)
        {
            Card card = _cardFactory.Create(item);

            if (origin != null)
                card.StartCoroutine(DramaMoveRoutine(card));
            else AddAtTheBottom(card);

        }

        private IEnumerator DramaMoveRoutine(Card card)
        {

            DeckServices.MoveCardToEffect(card, true);
            yield return new WaitForSeconds(1f); // Wait for the effect to complete
            AddAtTheBottom(card);

        }

        public void DrawSpecificCards(List<ItemData> list)
        {
            if (list == null || list.Count == 0) return;

            List<Card> drawnCards = new();
            foreach (var itemData in list)
            {
                Card card = Cards.Find(c => c.Item.Data == itemData);
                if (card != null)
                {
                    drawnCards.Add(card);
                    RemoveCard(card);
                }
                else
                {
                    Item item = InventoryServices.AddNewItem(itemData);
                    card = _cardFactory.Create(item);

                    drawnCards.Add(card);
                }
            }

            foreach (var card in drawnCards)
                DeckServices.MoveToTable(card);

        }
        internal List<Card> DrawMany(int number)
        {

            var cards = new List<Card>();

            for (int i = 0; i < number; i++)
            {
                Card card = DrawOne();
                if (card == null)
                    break;
                cards.Add(card);
            }
            return cards;
        }


        internal Card DrawOne()
        {
            if (!DrawTopCard(out Card card))
            {
                DeckServices.MovePileType(EnumCardPile.Discard, EnumCardPile.Draw);
                DeckServices.Shuffle();
                if (!DrawTopCard(out card))
                {
                    Debug.LogWarning("No cards left to draw.");
                    return null;
                }
            }
            card.UpdateUI();
            card.Item.NumberOfDraws++;
            return card;
        }

        internal Card DrawCategoryCard(EnumItemCategory category)
        {
            return Cards.Find(c => c.Category == category);
        }


        private void AddNewCard(ItemData data, bool isPermanent, Transform origin)
        {
            if (isPermanent)

                AddToDeck(InventoryServices.AddNewItem(data), origin);
            else

                AddToDeck(new Item(data), origin);
        }



        // internal void AddMultipleNew(ItemData data, int numCards)
        // {
        //     for (int i = 0; i < numCards; i++)
        //     {
        //         Item item = InventoryServices.AddNewItem(data);
        //         AddToDeck(item);
        //     }
        // }
        // internal void AddNewCardsToDeck(List<ItemData> list)
        // {
        //     foreach (var itemData in list)
        //     {
        //         var item = InventoryServices.AddNewItem(itemData);
        //         AddToDeck(item);
        //     }
        // }
    }
}
