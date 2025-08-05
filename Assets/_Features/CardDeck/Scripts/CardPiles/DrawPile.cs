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

        private List<ItemData> _forcedOnNextDraw;

        public DrawPile() : base(EnumCardPile.Draw, 0)
        {
            RegisterServices();
        }

        ~DrawPile()
        {
            UnregisterServices();
        }

        public void Populate()
        {
            var allItems = InventoryServices.GetAllItems();
            Populate(allItems);
        }

        private void RegisterServices()
        {

            DeckServices.ForceOnNextDraw = ForceOnNextDraw;
            DeckServices.DrawCategory = DrawCategoryCard;
            DeckServices.Draw = DrawMany;
        }



        private void UnregisterServices()
        {
            DeckServices.ForceOnNextDraw = delegate { };
            DeckServices.DrawCategory = category => null;
            DeckServices.Draw = number => new List<Card>();
        }

        internal void Populate(List<Item> items)
        {
            foreach (var item in items)
                AddToDeck(item);
            Shuffle();
        }
        private void AddToDeck(Item item)
        {
            Card card = DeckServices.CreateCard(item.Data);
            AddAtTheBottom(card, isInstant: true);
        }

        private IEnumerator DramaMoveRoutine(Card card)
        {
            DeckServices.MoveCard(card, EnumCardPile.Effect, EnumPlacement.OnTop, 1);
            yield return new WaitForSeconds(1f); // Wait for the effect to complete
            AddAtTheBottom(card);

        }

        public void ForceOnNextDraw(List<ItemData> list)
        {
            _forcedOnNextDraw = new(list);

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
            if (!GetFromForceQueue(out Card card) &&
                !DrawTopCard(out card))
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
            EffectServices.Execute(EnumEffectTrigger.OnDraw, card);
            return card;
        }

        private bool GetFromForceQueue(out Card card)
        {
            if (_forcedOnNextDraw == null || _forcedOnNextDraw.Count == 0)
            {
                card = null;
                return false;
            }
            ItemData itemData = _forcedOnNextDraw[0];
            _forcedOnNextDraw.RemoveAt(0);
            card = DeckServices.CreateCard(itemData);
            return true;
        }

        internal Card DrawCategoryCard(EnumItemCategory category)
        {
            return Cards.Find(c => c.Category == category);
        }
    }
}
