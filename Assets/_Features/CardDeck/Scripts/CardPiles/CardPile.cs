
using System;
using System.Collections.Generic;

using Quackery.Inventories;


namespace Quackery.Decks
{
    public class CardPile
    {
        public EnumCardPile Type;
        public int Index;
        public List<Card> Cards = new();
        public void Shuffle() => Cards.Shuffle();
        public bool IsEmpty => Cards.Count == 0;

        public Card TopCard => PeekTopCard();

        public EnumItemCategory Category => IsEmpty ? EnumItemCategory.Any : TopCard.Category;

        public int Count => Cards.Count;

        public bool Playable = true;
        public bool Enabled = true;

        public CardPile(EnumCardPile discardPile, int index)
        {
            Type = discardPile;
            Index = index;
        }

        private bool ValidityCheck()
        {
            if (Cards.Count == 0)
            {
                //Debug.LogWarning($"{Type} Deck is empty");
                return false;
            }
            return true;
        }

        public Card PeekTopCard()
        {
            if (!ValidityCheck()) return null;
            return Cards[0];
        }
        public Card PeekBottomCard()
        {
            if (!ValidityCheck()) return null;
            return Cards[^1];
        }

        public bool DrawTopCards(int numberCards, out List<Card> cards)
        {
            cards = new List<Card>();
            if (!ValidityCheck() || numberCards <= 0 || numberCards > Cards.Count) return false;

            for (int i = 0; i < numberCards; i++)
            {
                cards.Add(Cards[0]);
                Cards.RemoveAt(0);
            }
            return true;
        }

        public bool DrawBottomCards(int numberCards, out List<Card> cards)
        {
            cards = new List<Card>();
            if (!ValidityCheck() || numberCards <= 0 || numberCards > Cards.Count) return false;

            for (int i = 0; i < numberCards; i++)
            {
                cards.Add(Cards[^1]);
                Cards.RemoveAt(Cards.Count - 1);
            }
            return true;
        }

        public bool DrawTopCard(out Card card)
        {
            card = null;
            if (!ValidityCheck()) return false;
            card = Cards[0];
            Cards.RemoveAt(0);
            return true;
        }
        public bool DrawBottomCard(out Card card)
        {
            card = null;
            if (!ValidityCheck()) return false;
            card = Cards[^1];
            Cards.RemoveAt(Cards.Count - 1);
            return true;
        }

        public void Clear()
        {
            Cards.Clear();
        }

        public bool RemoveCard(Card card)
        {
            return Cards.Remove(card);
        }

        public void AddOnTop(Card card, bool isInstant = false)
        {
            if (card == null) return;
            Cards.Insert(0, card);
            DeckEvents.OnCardMovedTo(card, Type, Index, true, isInstant);
        }
        public void AddAtTheBottom(Card card, bool isInstant = false)
        {
            if (card == null) return;
            Cards.Add(card);
            DeckEvents.OnCardMovedTo(card, Type, Index, false, isInstant);
        }

        public void MergeOnTop(CardPile pile)
        {
            if (pile == null || pile.IsEmpty) return;

            foreach (var card in pile.Cards)
            {
                if (card != null)
                {
                    AddOnTop(card);
                    //DeckEvents.OnCardMovedTo(card, Type, Index, true);
                }
            }
            pile.Clear();
        }

        internal void MergeBelow(CardPile pile)
        {
            if (pile == null || pile.IsEmpty) return;

            foreach (var card in pile.Cards.ToArray())
            {
                if (card != null)
                {
                    AddAtTheBottom(card);
                    // DeckEvents.OnCardMovedTo(card, Type, Index, false);
                }
            }
            pile.Clear();
        }

        internal List<CardReward> CalculateCartRewards(List<CardPile> otherPiles)
        {
            if (IsEmpty || !Enabled) return new();
            var allCards = new List<Card>(Cards);
            allCards.Remove(TopCard);
            return TopCard.CalculateCardReward(allCards, otherPiles);

        }

        internal void RestoreCategory()
        {
            foreach (var card in Cards)
            {
                if (card != null)
                {
                    card.RemoveCategoryOverride();
                }
            }
        }

        internal void OverrideStackCategory(EnumItemCategory category)
        {
            for (int i = 0; i < Cards.Count; i++)
            {
                Cards[i].OverrideCategory(category);
            }

        }


        public bool Contains(Card card)
        {
            return Cards.Contains(card);
        }

        internal void UpdateUI()
        {
            if (IsEmpty || !Enabled) return;
            TopCard.UpdateUI();

        }

        internal void Add(Card card, EnumPlacement placement)
        {
            switch (placement)
            {
                case EnumPlacement.OnTop:
                    AddOnTop(card);
                    break;
                case EnumPlacement.AtTheBottom:
                    AddAtTheBottom(card);
                    break;
                case EnumPlacement.ShuffledIn:
                    AddShuffledIn(card);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(placement), placement, null);
            }
        }

        private void AddShuffledIn(Card card)
        {
            int index = UnityEngine.Random.Range(0, Cards.Count);
            Cards.Insert(index, card);
            DeckEvents.OnCardMovedTo(card, Type, Index, true, false);
        }
    }
}
